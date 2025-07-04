using SqlSugar;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using YayZent.Framework.Core.Interfaces;
using YayZent.Framework.Rbac.Domain.Shared.Enums;

namespace YayZent.Framework.Rbac.Domain.Entities;

[SugarTable("Role")]
public class RoleAggregateRoot: AuditedAggregateRoot<Guid>, ISoftDelete, IOrderNum, IState
{
    /// <summary>
    /// 角色名
    /// </summary>
    public string RoleName { get; set; } = string.Empty;
    
    /// <summary>
    /// 角色编码
    /// </summary>
    public string RoleCode { get; set; } = string.Empty;
    
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
    
    /// <summary>
    /// 角色数据范围
    /// </summary>
    public DataScopeEnum DateScope { get; set; } = DataScopeEnum.All;
    
    /// <summary>
    /// 软删除
    /// </summary>
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// 排序
    /// </summary>
    public int OrderNum { get; set; }
    
    /// <summary>
    /// 状态
    /// </summary>
    public bool State { get; set; } = true;
    
    [Navigate(typeof(RoleMenuEntity), nameof(RoleMenuEntity.RoleId), nameof(RoleMenuEntity.MenuId))]
    public List<MenuAggregateRoot>? Menus { get; set; }
    
    [Navigate(typeof(RoleDeptEntity), nameof(RoleDeptEntity.RoleId), nameof(RoleDeptEntity.DeptId))]
    public List<DeptAggregateRoot>? Depts { get; set; }
    
    public RoleAggregateRoot(){}

    public RoleAggregateRoot(Guid id): base(id){}
    
    public RoleAggregateRoot(Guid id, string roleName, string roleCode, DataScopeEnum dateScope):base(id)
    {
        RoleName = roleName;
        RoleCode = roleCode;
        DateScope = dateScope;
    }
}