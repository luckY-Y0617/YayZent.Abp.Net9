using SqlSugar;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using YayZent.Framework.Core.Interfaces;

namespace YayZent.Framework.Rbac.Domain.Entities;

[SugarTable("Dept")]
public class DeptAggregateRoot: AuditedAggregateRoot<Guid>, ISoftDelete, IOrderNum, IState
{
    /// <summary>
    /// 部门名称
    /// </summary>
    public string DeptName { get; set; } = string.Empty;
    
    /// <summary>
    /// 部门编码
    /// </summary>
    public string DeptCode { get; set; } = string.Empty;
    
    /// <summary>
    /// 负责人
    /// </summary>
    public string? Leader { get; set; }
    
    /// <summary>
    /// 父级部门Id
    /// </summary>
    public Guid ParentDeptId { get; set; }
    
    /// <summary>
    /// 部门备注
    /// </summary>
    public string? Remark { get; set; } 
    
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
    public bool State { get; set; }
    
    public DeptAggregateRoot() {}

    public DeptAggregateRoot(Guid id): base(id) { }

    public DeptAggregateRoot(Guid id, Guid parentDeptId) : this(id)
    {
        ParentDeptId = parentDeptId;
    }
}