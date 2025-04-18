using SqlSugar;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace YayZent.Framework.Blog.Domain.Entities;

[SugarTable("Category")]
public class CatergoryAggregateRoot: AuditedAggregateRoot<Guid>, ISoftDelete
{
    /// <summary>
    /// category
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 在所有Category中的显示序号，越小越靠前
    /// </summary>
    public int SequenceNumber { get; set; } = 0;

    /// <summary>
    /// 软删除
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 创建时间
    /// </summary>
    public override DateTime CreationTime { get; set; } = DateTime.Now;


    public CatergoryAggregateRoot() {}

    public CatergoryAggregateRoot(Guid id, string categoryName, int sequenceNumber) : base(id)
    {
        CategoryName = categoryName;
        SequenceNumber = sequenceNumber;
    }
    
    public void ChangeSequenceNumber(int newSeq)
    {
        SequenceNumber = newSeq;
    }
}