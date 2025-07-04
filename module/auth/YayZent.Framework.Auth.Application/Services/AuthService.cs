using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using YayZent.Framework.Auth.Application.Contracts.Captcha;
using YayZent.Framework.Auth.Application.Contracts.Dtos.Auth;
using YayZent.Framework.Auth.Application.Contracts.Enums;
using YayZent.Framework.Auth.Application.Contracts.Token;
using YayZent.Framework.Auth.Domain.Entities;
using YayZent.Framework.Auth.Domain.Shared.Consts;
using YayZent.Framework.Core.Helper;
using YayZent.Framework.Ddd.Application.Contracts.Dtos;
using YayZent.Framework.Rbac.Application.Contracts.Cache;
using YayZent.Framework.SqlSugarCore.Abstractions;
using YayZent.Framework.Rbac.Application.Contracts.IService.User;
using YayZent.Framework.Rbac.Domain.Shared.Consts;
using YayZent.Framework.Rbac.Domain.Shared.Etos;
using YayZent.Framework.Rbac.Domain.Shared.Options;

namespace YayZent.Framework.Auth.Application.Services;

public class AuthService: ApplicationService
{
    private ILocalEventBus LocalEventBus => LazyServiceProvider.LazyGetRequiredService<ILocalEventBus>();
    private readonly IDistributedCache<UserInfoCacheItem, UserInfoCacheKey> _userCache;
    private readonly RbacOptions _rbacOptions;
    private readonly ICurrentUser _currentUser;
    private readonly ISqlSugarRepository<RefreshTokenAggregateRoot> _refreshTokenRepository;
    private readonly IUserInternalService _userInternalService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LoginLogService _loginLogService;
    private readonly IObjectMapper _objectMapper;
    private readonly ICaptchaService _captchaService;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ITokenProvider _tokenProvider;

    public AuthService(
        IDistributedCache<UserInfoCacheItem, UserInfoCacheKey> userCache, 
        IHttpContextAccessor httpContextAccessor,  LoginLogService loginLogService, ITokenProvider tokenProvider,
        IObjectMapper objectMapper,  ICurrentUser currentUser, ISqlSugarRepository<RefreshTokenAggregateRoot> refreshTokenRepository,
        IOptions<RbacOptions> rbacOptions, IUserInternalService userInternalService, ICaptchaService captchaService, IGuidGenerator guidGenerator)
    {
        _userCache = userCache;
        _httpContextAccessor = httpContextAccessor;
        _loginLogService = loginLogService;
        _objectMapper = objectMapper;
        _currentUser = currentUser;
        _refreshTokenRepository = refreshTokenRepository;
        _userCache = userCache;
        _rbacOptions = rbacOptions.Value;
        _userInternalService = userInternalService;
        _captchaService = captchaService;
        _guidGenerator = guidGenerator;
        _tokenProvider = tokenProvider;
    }

    private Guid GetCurrentUserId()
    {
        return _currentUser.Id ?? throw new UserFriendlyException("用户未登录");
    }
    
    private async Task<string> GetTokenByUserIdAsync(Guid userId, Action<UserClaimsContext>? configure = null)
    {
        var userInfo = await GetClaimsAsync(userId);
        configure?.Invoke(userInfo);
        return await _tokenProvider.CreateAccessTokenAsync(userInfo.Claims);
    }

    private async Task<string> GetRefreshTokenByUserIdAsync(Guid userId)
    {
        return await _tokenProvider.CreateRefreshTokenAsync(userId);
    }
    
    private async Task<UserClaimsContext> GetClaimsAsync(Guid userId)
    {
        var dto = (await _userCache.GetAsync(new UserInfoCacheKey(userId)))?.Info;
        if (dto == null)
        {
            dto = await _userInternalService.GetUserInfoAsync(userId);

            await _userCache.SetAsync(
                new UserInfoCacheKey(userId),
                new UserInfoCacheItem(dto),
                new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) }
            );
        }

        var claims = new List<Claim>();

        if (!dto.User.State) throw new UserFriendlyException(UserConst.StateIsState);
        if (dto.RoleCodes.Count == 0) throw new UserFriendlyException(UserConst.NoRole);
        if (dto.PermissionCodes.Count == 0) throw new UserFriendlyException(UserConst.NoPermission);

        claims.Add(new Claim(AbpClaimTypes.UserId, dto.User.Id.ToString()));
        claims.Add(new Claim(AbpClaimTypes.UserName, dto.User.UserName));

        if (dto.User.DeptId != null)
        {
            claims.Add(new Claim(TokenTypeConst.DeptId, dto.User.DeptId.Value.ToString()));
        }

        if (!string.IsNullOrEmpty(dto.User.Phone))
        {
            claims.Add(new Claim(TokenTypeConst.PhoneNumber, dto.User.Phone));
        }

        if (UserConst.Admin.Equals(dto.User.UserName))
        {
            claims.Add(new Claim(TokenTypeConst.Permission, UserConst.AdminPermissionCode));
            claims.Add(new Claim(TokenTypeConst.Roles, UserConst.AdminRoleCode));
        }
        else
        {
            foreach (var p in dto.PermissionCodes)
            {
                claims.Add(new Claim(TokenTypeConst.Permission, p));
            }

            foreach (var r in dto.RoleCodes)
            {
                claims.Add(new Claim(TokenTypeConst.Roles, r));
            }
        }

        return new UserClaimsContext { Claims = claims };
    }
    
    /// <summary>
    /// 获取图形验证码
    /// </summary>
    /// <returns></returns>
    public async Task<ApiResponse<ImageCaptchaOutputDto>> GetImageCaptchaAsync()
    {
        var uuid = _guidGenerator.Create();
        var captchaBytes = await _captchaService.GenerateImageCaptchaAsync(uuid);
        var enableCaptcha = _rbacOptions.EnableCaptcha;

        return ApiResponse<ImageCaptchaOutputDto>.Ok(new ImageCaptchaOutputDto()
        {
            Img = captchaBytes,
            Uuid = uuid,
            IsEnableCaptcha = enableCaptcha
        });
    }

    /// <summary>
    /// 获取注册用的邮箱验证码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<ApiResponse> PostEmailCaptchaForRegisterAsync(EmailCaptchaForRegisterInputDto input)
    {
        try
        {
            _captchaService.ValidateImageCaptcha(input.Uuid, input.Code);

            if (await _userInternalService.IsEmailRegisteredAsync(input.Email))
            {
                throw new UserFriendlyException("该邮箱已被注册");
            }

            await _captchaService.SendEmailCaptchaAsync(ValidationEmailTypeEnum.Register, input.Email);
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail(ex.Message);
        }

        return ApiResponse.Ok();
    }

    /// <summary>
    /// 获取注册用的手机验证码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<object> PostPhoneCaptchaForRegisterAsync(PhoneCaptchaForRegisterInputDto input)
    {
        _captchaService.ValidateImageCaptcha(input.Uuid, input.Code);

        if (await _userInternalService.IsPhoneNumberRegisteredAsync(input.Phone))
        {
            throw new UserFriendlyException("该手机号已被注册");
        }
        
        var code = await _captchaService.SendPhoneCaptchaAsync(ValidationPhoneTypeEnum.Register, input.Phone);

        return new { code };
    }
    
    /// <summary>
    /// 通过邮箱注册用户
    /// </summary>
    /// <param name="input"></param>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<ApiResponse> PostRegisterAsync(RegisterInputDto input)
    {
        try
        {
            if (_rbacOptions.EnableRegister == false)
            {
                throw new UserFriendlyException("暂未开放注册功能");
            }

            if (input.Email is null)
            {
                throw new UserFriendlyException("邮箱号不能为空");
            }

            if (input.UserName.StartsWith("ls_"))
            {
                throw new UserFriendlyException("用户名不能以ls开头");
            }

            if (_rbacOptions.EnableCaptcha)
            {
                await _captchaService.ValidateEmailCaptchaAsync(ValidationEmailTypeEnum.Register, input.Email,
                    input.Code);
            }

            await _userInternalService.InsertNewUserByEmailAsync(input.UserName, input.Password, input.Email);
            
            return ApiResponse.Ok();
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail(ex.Message);
        }
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<ApiResponse<LoginOutputDto>> PostLoginAsync(LoginInputDto input)
    {
        try
        {
            if (string.IsNullOrEmpty(input.UserName) || string.IsNullOrEmpty(input.Password))
            {
                throw new UserFriendlyException("请输入用户名或密码");
            }

            var user = await _userInternalService.ValidateAndGetUserAsync(input.UserName, input.Password);
            var accessToken = await GetTokenByUserIdAsync(user.Id);
            var refreshToken = await GetRefreshTokenByUserIdAsync(user.Id);

            if (_httpContextAccessor.HttpContext != null)
            {
                // 1. 设置 HttpOnly Cookie 存储 RefreshToken
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // 生产环境必须启用，要求HTTPS
                    SameSite = SameSiteMode.Strict, // 根据需求调整（Strict 或 Lax 或 None）
                    Expires = DateTimeOffset.UtcNow.AddDays(7) // 根据RefreshToken有效期设置
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

                // 2. 记录登录日志
                var logEntity = await _loginLogService.CreateLoginLogAsyncFromHttpContext(_httpContextAccessor.HttpContext);
                var loginEto = _objectMapper.Map<LoginLogAggregateRoot, LoginEventArgs>(logEntity);
                loginEto.UserName = user.UserName;
                loginEto.UserId = user.Id;
                await LocalEventBus.PublishAsync(loginEto);
            }
            
            // 3. 返回 AccessToken 给前端
            return ApiResponse<LoginOutputDto>.Ok(new LoginOutputDto() { Token = accessToken, UserName = user.UserName, UserId = user.Id });
        }
        catch (Exception ex)
        {
            return ApiResponse<LoginOutputDto>.FailWithData(ex.Message);
        }

    }

    /// <summary>
    /// 获取refreshtoken
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<ApiResponse<object>> PostRefreshToken()
    {
        try
        {
            var refreshTokenFromCookie = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshTokenFromCookie))
            {
                throw new UserFriendlyException("缺少refreshToken");
            }

            var hashRefreshToken = HashHelper.ComputeSha256Hash(refreshTokenFromCookie, HashHelper.HashEncoding.Hex);

            var refreshTokenEntity = await _refreshTokenRepository
                .GetFirstAsync(x => x.Token == hashRefreshToken && !x.IsActive && x.ExpiresAt > DateTimeOffset.UtcNow);

            if (refreshTokenEntity == null)
            {
                throw new Exception("Refresh failed");
            }
            
            var userId = refreshTokenEntity.UserId;
            var user = await _userInternalService.GetUserAsync(userId);

            refreshTokenEntity.Activate();
            await _refreshTokenRepository.UpdateAsync(refreshTokenEntity);

            var accessToken = await GetTokenByUserIdAsync(userId);
            var refreshToken = await GetRefreshTokenByUserIdAsync(userId);

            if (_httpContextAccessor.HttpContext != null)
            {
                // 1. 设置 HttpOnly Cookie 存储 RefreshToken
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // 生产环境必须启用，要求HTTPS
                    SameSite = SameSiteMode.Strict, // 根据需求调整（Strict 或 Lax 或 None）
                    Expires = DateTimeOffset.UtcNow.AddDays(7) // 根据RefreshToken有效期设置
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

                // 2. 记录登录日志
                var logEntity = await _loginLogService.CreateLoginLogAsyncFromHttpContext(_httpContextAccessor.HttpContext);
                var loginEto = _objectMapper.Map<LoginLogAggregateRoot, LoginEventArgs>(logEntity);
                loginEto.UserName = user.UserName;
                loginEto.UserId = user.Id;
                await LocalEventBus.PublishAsync(loginEto);
            }
            return ApiResponse<object>.Ok(new {accessToken});
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.FailWithData(ex.Message);
        }
    }


    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<string> PostRetrivePasswordAsync(RetrievePasswordRequestDto request)
    {
        await _captchaService.ValidatePhoneCaptchaAsync(ValidationPhoneTypeEnum.RetrievePassword, request.Phone, request.Code);

        return await _userInternalService.RetrivePassword(request.Phone, request.OldPassword, request.Password);
    }

    /// <summary>
    /// 登出
    /// </summary>
    /// <returns></returns>
    public async Task<ApiResponse> PostLoginOutAsync()
    {
        try
        {
            var userId = GetCurrentUserId();

            await _userCache.RemoveAsync(new UserInfoCacheKey(userId));
            await _refreshTokenRepository.DeleteAsync(x => x.UserId == userId);
            return ApiResponse.Ok();
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail(ex.Message);
        }
    }
}