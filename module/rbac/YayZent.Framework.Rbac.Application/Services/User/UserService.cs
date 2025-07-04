using Volo.Abp.Application.Services;
using YayZent.Framework.Rbac.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Rbac.Domain.Entities;
using YayZent.Framework.Rbac.Domain.Repositories;
using YayZent.Framework.Rbac.Domain.Shared.Consts;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Rbac.Application.Services.User;

public class UserService: ApplicationService
{
    private readonly IUserRepository _userRepository;
    private readonly ISqlSugarRepository<RoleAggregateRoot> _roleRepository;
    private readonly IRoleDomainService _roleDomainService;
    private readonly IUserDomainService _userDomainService;
    private readonly ISqlSugarRepository<UserRoleEntity> _userRoleRepository;


    public UserService(IUserRepository userRepository, ISqlSugarRepository<RoleAggregateRoot> roleRepository,
        ISqlSugarRepository<UserRoleEntity> userRoleRepository, IRoleDomainService roleDomainService, IUserDomainService userDomainService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _roleDomainService = roleDomainService;
        _userDomainService = userDomainService;
    }

    public async Task<bool> InitializeAdminUser()
    {
        var adminUser = await _userRepository.DbQueryable.Where(x => x.UserName == "admin").FirstAsync();

        var userRoleEntities = await _userDomainService.SetAdminRoleAsync(adminUser.Id);
        await _userRoleRepository.InsertManyAsync(userRoleEntities);
        return true;
    }
    
    public async Task<bool> InitializeDefaultUser()
    {
        var defaultUser = await _userRepository.DbQueryable.Where(x => x.UserName == "default").FirstAsync();

        var userRoleEntities = await _userDomainService.SetDefaultRolesAsync(defaultUser.Id);
        await _userRoleRepository.InsertManyAsync(userRoleEntities);
        return true;
    }
}