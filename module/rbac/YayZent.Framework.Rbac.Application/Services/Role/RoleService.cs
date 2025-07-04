using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;
using YayZent.Framework.Ddd.Application;
using YayZent.Framework.Rbac.Application.Contracts.Dtos.Role;
using YayZent.Framework.Rbac.Domain.DomainServices;
using YayZent.Framework.Rbac.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Rbac.Domain.Entities;
using YayZent.Framework.Rbac.Domain.Shared.Consts;
using YayZent.Framework.Rbac.Domain.Shared.Enums;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Rbac.Application.Services.Role;

public class RoleService: ApplicationService
{
    public readonly IObjectMapper _ObjectMapper;
    private readonly IRoleDomainService _roleDomainService;
    private readonly ISqlSugarRepository<RoleAggregateRoot, Guid> _roleRepository;
    private readonly ISqlSugarRepository<UserRoleEntity, Guid> _userRoleRepository;
    private readonly ISqlSugarRepository<RoleDeptEntity, Guid> _roleDeptRepository;
    private readonly ISqlSugarRepository<RoleMenuEntity, Guid> _roleMenuRepository;
    private readonly ISqlSugarRepository<MenuAggregateRoot, Guid> _menuRepository;

    public RoleService(
        IObjectMapper objectMapper,
        IRoleDomainService roleDomainService,
        ISqlSugarRepository<RoleAggregateRoot, Guid> roleRepository,
        ISqlSugarRepository<UserRoleEntity, Guid> userRoleRepository,
        ISqlSugarRepository<RoleMenuEntity, Guid> roleMenuRepository,
        ISqlSugarRepository<RoleDeptEntity, Guid> roleDeptRepository,
        ISqlSugarRepository<MenuAggregateRoot, Guid> menuRepository) 
    {
        _ObjectMapper = objectMapper;
        _roleDomainService = roleDomainService;
        _roleRepository = roleRepository;
        _roleMenuRepository = roleMenuRepository;
        _userRoleRepository = userRoleRepository;
        _roleDeptRepository = roleDeptRepository;
        
        _menuRepository = menuRepository;
    }

    public async Task InitializeAdminRoleAsync()
    {
        var adminRole = await _roleRepository.GetFirstAsync(x => x.RoleCode == UserConst.AdminRoleCode);
        if (adminRole == null)
        {
            throw new UserFriendlyException("Admin role not found");
        }
        var menus = await _menuRepository.DbQueryable.Select(x => x.Id).ToListAsync();
        if (menus == null)
        {
            throw new UserFriendlyException("No menus found");
        }
        
        var roleMenus =  await _roleDomainService.SetRoleMenusAsync([adminRole.Id], menus);
        await _roleMenuRepository.InsertManyAsync(roleMenus);
    }
    
    public async Task InitializeDefaultRoleAsync()
    {
        var defaultRole = await _roleRepository.GetFirstAsync(x => x.RoleCode == UserConst.DefaultRoleCode);
        if (defaultRole == null)
        {
            throw new UserFriendlyException("default role not found");
        }
        var menus = await _menuRepository.DbQueryable
            .Where(x => x.MenuAccessLevel != MenuAccessLevelEnum.AdminOnly)
            .Select(x => x.Id)
            .ToListAsync();
        if (menus == null)
        {
            throw new UserFriendlyException("No menus found");
        }
        
        var roleMenus =  await _roleDomainService.SetRoleMenusAsync([defaultRole.Id], menus);
        await _roleMenuRepository.InsertManyAsync(roleMenus);
    }

    public async Task UpdateDataScopeAsync(UpdateDataScopeInputDto input)
    {
        //如果 DataScope 是 CUSTOM，就删除角色的原有部门权限,插入新的部门权限
        if (input.DataScope == DataScopeEnum.Custom)
        {
            await _roleDeptRepository.DeleteAsync(x => x.Id == input.RoleId);
            var newRoleDeptEntities = input.DeptIds.Select(x => new RoleDeptEntity(){DeptId = x, RoleId = input.RoleId});
            await _roleDeptRepository.InsertManyAsync(newRoleDeptEntities);
        }
        
        var entity = new RoleAggregateRoot(input.RoleId) {DateScope = input.DataScope};
        await _roleRepository.DbContext.Updateable(entity).UpdateColumns(x => x.DateScope).ExecuteCommandAsync();
    }


    public async Task<PagedResultDto<RoleGetListOutputDto>> GetListAsync(RoleGetListInputDto input)
    {
        RefAsync<int> total = 0;
        
        var entities = await _roleRepository.DbQueryable
            .WhereIF(!string.IsNullOrEmpty(input.RoleCode), x => x.RoleCode.Contains(input.RoleCode))
            .WhereIF(input.State is not null, x => x.State == input.State)
            .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);

        var rs = _ObjectMapper.Map<List<RoleAggregateRoot>, List<RoleGetListOutputDto>>(entities);
        
        return new PagedResultDto<RoleGetListOutputDto>(total, rs);
    }


    public  async Task<RoleGetOutputDto> CreateAsync(RoleCreateDto request)
    {
        var isExist = await _roleRepository.AnyAsync(x => x.RoleCode == request.RoleCode || x.RoleName == request.RoleName);

        if (isExist)
        {
            throw new UserFriendlyException(RoleConst.Exist);
        }

        var entity = _ObjectMapper.Map<RoleCreateDto, RoleAggregateRoot>(request);
        await _roleRepository.InsertAsync(entity);
        return _ObjectMapper.Map<RoleAggregateRoot, RoleGetOutputDto>(entity);
    }

    public async Task<RoleGetOutputDto> UpdateAsync(Guid roleId, RoleUpdateDto request)
    {
        var role = await _roleRepository.GetAsync(roleId);
        
        var isExist = await _roleRepository.AnyAsync(x => x.RoleCode == request.RoleCode || x.RoleName == request.RoleName);

        if (isExist)
        {
            throw new UserFriendlyException(RoleConst.Exist);
        }
        
        _ObjectMapper.Map(request, role);
        await _roleRepository.UpdateAsync(role);
        await _roleDomainService.SetRoleMenusAsync(new List<Guid>{roleId}, request.DeptIds);
        
        return _ObjectMapper.Map<RoleAggregateRoot, RoleGetOutputDto>(role);
    }

    [Route("role/{roleId}/state={state}")]
    public async Task<RoleGetOutputDto> UpdateStateAsync([FromRoute] Guid id, [FromRoute] bool state)
    {
        var entity = await _roleRepository.GetAsync(id);
        if (entity == null)
        {
            throw new UserFriendlyException("Role not found");
        }
        entity.State = state;
        await _roleRepository.UpdateAsync(entity);
        return _ObjectMapper.Map<RoleAggregateRoot, RoleGetOutputDto>(entity);
    }
    
}