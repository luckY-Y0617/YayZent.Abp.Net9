using SqlSugar;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using YayZent.Framework.Core.Interfaces;

namespace YayZent.Framework.Rbac.Domain.Entities;

[SugarTable("Post")]
public class PostAggregateRoot: AuditedAggregateRoot<Guid>, ISoftDelete, IOrderNum, IState
{
    /// <summary>
    /// 岗位编码
    /// </summary>
    public string PostCode { get; set; } = string.Empty;
    
    /// <summary>
    /// 岗位名称
    /// </summary>
    public string PostName { get; set; } = string.Empty;
    
    /// <summary>
    /// 描述
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
    
    public PostAggregateRoot() {}

    public PostAggregateRoot(Guid id, string postCode, string postName, int orderNum)
    {
        Id = id;
        PostCode = postCode;
        PostName = postName;
        OrderNum = orderNum;
    }
}