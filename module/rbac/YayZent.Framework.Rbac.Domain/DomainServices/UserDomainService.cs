using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.ObjectMapping;
using YayZent.Framework.Rbac.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Rbac.Domain.Entities;
using YayZent.Framework.Rbac.Domain.Repositories;
using YayZent.Framework.SqlSugarCore.Abstractions;
using YayZent.Framework.Rbac.Domain.Shared.Consts;
using YayZent.Framework.Rbac.Domain.Shared.Etos;

namespace YayZent.Framework.Rbac.Domain.DomainServices;

/// <summary>
/// “不要在应用服务中写业务逻辑”
/// 不要用仓储替代领域模型的业务行为”
/// 业务规则属于领域层，数据存取属于仓储，流程控制属于应用服务
/// </summary>
public class UserDomainService: DomainService, IUserDomainService
{
    private readonly IUserRepository _userRepository;
    private readonly ISqlSugarRepository<RoleAggregateRoot> _roleRepository;
    private readonly ISqlSugarRepository<UserPostEntity> _userPostRepository;
    private readonly ISqlSugarRepository<UserRoleEntity> _userRoleRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ILocalEventBus _localEventBus;
    private readonly IObjectMapper _objectMapper;

    public UserDomainService(IUserRepository userRepository, IObjectMapper objectMapper,
        ISqlSugarRepository<RoleAggregateRoot> roleRepository, ISqlSugarRepository<UserRoleEntity> userRoleRepository,
        ISqlSugarRepository<UserPostEntity> userPostRepository, IGuidGenerator guidGenerator, ILocalEventBus localEventBus)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userPostRepository = userPostRepository;
        _userRoleRepository = userRoleRepository;
        _guidGenerator = guidGenerator;
        _localEventBus = localEventBus;
        _objectMapper = objectMapper;
    }

    public async Task<UserAggregateRoot> GetAsync(Guid id)
    {
        return await _userRepository.GetFirstAsync(x => x.Id == id);
    }

    public async Task<UserAggregateRoot> CreateUserByPhoneAsync(string userName, string password, string phone)
    {
        if (await _userRepository.AnyAsync(x => x.Phone == phone))
        {
            throw new UserFriendlyException(UserConst.PhoneRepeat);
        }

        if (await _userRepository.AnyAsync(x => x.UserName == userName))
        {
            throw new UserFriendlyException(UserConst.Exist);
        }
        
        var user = UserAggregateRoot.RegisterByPhone(userName, password, phone);
        
        await _localEventBus.PublishAsync(new UserCreateEventArgs());

        return user;
    }
    
    public async Task<UserAggregateRoot> CreateUserByEmailAsync(string userName, string password, string email)
    {
        if (await _userRepository.AnyAsync(x => x.Email == email))
        {
            throw new UserFriendlyException(UserConst.EmailRepeat);
        }

        if (await _userRepository.AnyAsync(x => x.UserName == userName))
        {
            throw new UserFriendlyException(UserConst.Exist);
        }
        
        var user = UserAggregateRoot.RegisterByEmail(userName, password, email);
        
        await _localEventBus.PublishAsync(new UserCreateEventArgs());

        return user;
    }

    public async Task<List<UserRoleEntity>> SetUserRolesAsync(List<Guid> userIds, List<Guid> roleIds)
    {
        await _userRoleRepository.DeleteAsync(x => userIds.Contains(x.UserId));

        var userRoles = userIds
            .SelectMany(x => roleIds.Select(u => new UserRoleEntity(){ UserId = x, RoleId = u}))
            .ToList();
        return userRoles;
    }

    public async Task<List<UserPostEntity>> SetUserPostsAsync(List<Guid> userIds, List<Guid> postIds)
    {
        await _userPostRepository.DeleteAsync(x => userIds.Contains(x.UserId));
        
        var userPosts = userIds
            .SelectMany(x => postIds.Select(u => new UserPostEntity(){ UserId = x, PostId = u}))
            .ToList();
        return userPosts;
    }

    public async Task<List<UserRoleEntity>> SetDefaultRolesAsync(Guid userId)
    {
        var defaultRole = await _roleRepository.GetFirstAsync(x => x.RoleCode == UserConst.DefaultRoleCode);
        if (defaultRole == null)
        {
            throw new UserFriendlyException("Default role is not set");
        }
        var rs = await SetUserRolesAsync([userId], [defaultRole.Id]);
        return rs;
    }

    public async Task<List<UserRoleEntity>> SetAdminRoleAsync(Guid userId)
    {
        var adminRole = await _roleRepository.GetFirstAsync(x => x.RoleCode == UserConst.AdminRoleCode);
        if (adminRole == null)
        {
            throw new UserFriendlyException("Admin role is not set");
        }
        var rs = await SetUserRolesAsync([userId], [adminRole.Id]);
        return rs;
    }


    public async Task<UserAggregateRoot> LoginValidationAsync(string username, string password)
    {
        
        var user = await _userRepository.GetFirstAsync(x => x.UserName == username && x.State == true);

         if (user == null)
         {
             throw new UserFriendlyException(UserConst.LoginUserNoExist);
         }
        
         if (!user.IsPasswordMatch(password))
         {
             throw new UserFriendlyException(UserConst.LoginError);
         }
        
        return user;
    }


}