using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.ObjectMapping;
using YayZent.Framework.Core.Options;
using YayZent.Framework.Rbac.Application.Contracts.Cache;
using YayZent.Framework.Rbac.Application.Contracts.Dtos;
using YayZent.Framework.Rbac.Application.Contracts.IService.User;
using YayZent.Framework.Rbac.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Rbac.Domain.Entities;
using YayZent.Framework.Rbac.Domain.Entities.ValueObjects;
using YayZent.Framework.Rbac.Domain.Repositories;
using YayZent.Framework.Rbac.Domain.Shared.Consts;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Rbac.Application.Services.User;

public class UserInternalService: IUserInternalService
{
    private readonly JwtOptions _jwtOptions;
    private readonly IUserRepository _userRepository;
    private readonly IObjectMapper _objectMapper;
    private readonly IUserDomainService _userDomainService;
    private readonly ISqlSugarRepository<UserRoleEntity> _userRoleRepository;
    private readonly IDistributedCache<UserInfoCacheItem, UserInfoCacheKey> _userCache;

    public UserInternalService(IUserRepository userRepository, ISqlSugarRepository<UserRoleEntity> userRoleRepository, IObjectMapper objectMapper, IUserDomainService userDomainService,
        IDistributedCache<UserInfoCacheItem, UserInfoCacheKey> userCache, IOptions<JwtOptions> jwtOptions)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _userDomainService = userDomainService;
        _objectMapper = objectMapper;
        _userCache = userCache;
        _jwtOptions = jwtOptions.Value;
    }
    
    public async Task<UserRoleMenuDto> GetUserInfoAsync(Guid userId)
    {
        var user = await _userRepository.GetUserAllInfoAsync(userId);
        return MapUserToUserRoleMenuDto(user);
    }

    private UserRoleMenuDto MapUserToUserRoleMenuDto(UserAggregateRoot user)
    {
        user.EncryPassword = new EncryPasswordValueObject(string.Empty, string.Empty);

        var userRoleMenuDto = new UserRoleMenuDto();
        if (UserConst.Admin.Equals(user.UserName))
        {
            userRoleMenuDto.User = _objectMapper.Map<UserAggregateRoot, UserDto>(user);
            userRoleMenuDto.RoleCodes.Add(UserConst.AdminRoleCode);
            userRoleMenuDto.PermissionCodes.Add(UserConst.AdminPermissionCode);
            return userRoleMenuDto;
        }

        foreach (var role in user.Roles ?? Enumerable.Empty<RoleAggregateRoot>())
        {
            userRoleMenuDto.RoleCodes.Add(role.RoleCode);

            foreach (var menu in role.Menus ?? Enumerable.Empty<MenuAggregateRoot>())
            {
                if (!string.IsNullOrEmpty(menu.PermissionCode))
                {
                    userRoleMenuDto.PermissionCodes.Add(menu.PermissionCode);
                }

                userRoleMenuDto.Menus.Add(_objectMapper.Map<MenuAggregateRoot, MenuDto>(menu));
            }

            role.Menus = new List<MenuAggregateRoot>();
            userRoleMenuDto.Roles.Add(_objectMapper.Map<RoleAggregateRoot, RoleDto>(role));
        }
        
        user.Roles = new List<RoleAggregateRoot>();
        userRoleMenuDto.User = _objectMapper.Map<UserAggregateRoot, UserDto>(user);
        userRoleMenuDto.Menus = userRoleMenuDto.Menus.OrderByDescending(x => x.OrderNum).ToHashSet();
        return userRoleMenuDto;
    }

    public async Task<List<UserRoleMenuDto>> GetInfoListAsync(List<Guid> userIds)
    {
        var rs = new List<UserRoleMenuDto>();
        foreach (var userId in userIds)
        {
            rs.Add(await GetUserInfoAsync(userId));
        }
        return rs;
    }

    public async Task<string> RetrivePassword(string phone, string oldPassword, string newPassword)
    {
        var user = await _userRepository.FindAsync(x => x.Phone == phone);
        if (user == null)
        {
            throw new UserFriendlyException("该手机号未注册");
        }
        
        user.UpdatePassword(oldPassword ,newPassword);
        await _userRepository.UpdateAsync(user);
        return user.UserName;
    }

    public async Task<UserDto> GetUserAsync(Guid userId)
    {
        var user = await _userDomainService.GetAsync(userId);
        if (user == null)
        {
            throw new UserFriendlyException("User not found");
        }
        return _objectMapper.Map<UserAggregateRoot, UserDto>(user);
    }

    public async Task<UserDto> ValidateAndGetUserAsync(string userName, string password)
    {
        var user = await _userDomainService.LoginValidationAsync(userName, password);

        return _objectMapper.Map<UserAggregateRoot, UserDto>(user);
    }


    public async Task<bool> IsEmailRegisteredAsync(string email)
    {
        return await _userRepository.AnyAsync(x => x.Email == email);
    }

    public async Task<bool> IsPhoneNumberRegisteredAsync(string phone)
    {
        return await _userRepository.AnyAsync(x => x.Phone == phone);
    }

    public async Task InsertNewUserByPhoneAsync(string userName, string password, string phone)
    {
        try
        {
            var user = await _userDomainService.CreateUserByPhoneAsync(userName, password, phone);
            await _userDomainService.SetDefaultRolesAsync(user.Id);

            await _userRepository.InsertAsync(user);
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException(ex.Message);
        }
    }
    
    public async Task InsertNewUserByEmailAsync(string userName, string password, string email)
    {
        try
        {
            var user = await _userDomainService.CreateUserByEmailAsync(userName, password, email);
            var userRoleEntitys = await _userDomainService.SetDefaultRolesAsync(user.Id);
            
            await _userRoleRepository.InsertManyAsync(userRoleEntitys);

            await _userRepository.InsertAsync(user);
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException(ex.Message);
        }
    }


    private async Task<UserRoleMenuDto> GetInfoByCacheAsync(Guid userId)
    {
        UserRoleMenuDto? userInfo = null;
        var tokenExpiresSeconds = _jwtOptions.Expiration;
        var cacheUserInfo = await _userCache.GetOrAddAsync(new UserInfoCacheKey(userId), async () =>
        {
            var user = await _userRepository.GetUserAllInfoAsync(userId);
            var userRoleMenu = MapUserToUserRoleMenuDto(user);
            return new UserInfoCacheItem(userRoleMenu);
        }, () => new DistributedCacheEntryOptions{ AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokenExpiresSeconds)});

        return cacheUserInfo?.Info!;
    }
}