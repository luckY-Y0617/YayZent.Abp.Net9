using Volo.Abp.Domain.Services;
using YayZent.Framework.Rbac.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Rbac.Domain.Entities;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Rbac.Domain.DomainServices;

public class RoleDomainService: DomainService, IRoleDomainService
{
    private readonly ISqlSugarRepository<RoleAggregateRoot> _roleRepository;
    private readonly ISqlSugarRepository<RoleMenuEntity> _roleMenuRepository;
    private readonly ISqlSugarRepository<RoleDeptEntity> _roleDeptRepository;

    public RoleDomainService(ISqlSugarRepository<RoleAggregateRoot> roleRepository,
        ISqlSugarRepository<RoleMenuEntity> roleMenuRepository, ISqlSugarRepository<RoleDeptEntity> roleDeptRepository)
    {
        _roleRepository = roleRepository;
        _roleMenuRepository = roleMenuRepository;
        _roleDeptRepository = roleDeptRepository;
    }

    public async Task<List<RoleMenuEntity>> SetRoleMenusAsync(List<Guid> roleIds, List<Guid> menuIds)
    {
        await _roleMenuRepository.DeleteAsync(x => roleIds.Contains(x.RoleId));
        
        var roleMenus = roleIds
            .SelectMany(x => menuIds.Select(u => new RoleMenuEntity() { RoleId = x, MenuId = u }))
            .ToList();

        return roleMenus;
    }
    
    public async Task<List<RoleMenuEntity>> SetRoleDeptsAsync(List<Guid> roleIds, List<Guid> deptIds)
    {
        await _roleDeptRepository.DeleteAsync(x => roleIds.Contains(x.RoleId));
        
        var roleMenus = roleIds
            .SelectMany(x => deptIds.Select(u => new RoleMenuEntity() { RoleId = x, MenuId = u }))
            .ToList();

        return roleMenus;
    }
}