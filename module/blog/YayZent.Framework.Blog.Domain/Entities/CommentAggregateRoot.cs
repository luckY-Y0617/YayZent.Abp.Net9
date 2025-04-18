using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace YayZent.Framework.Blog.Domain.Entities;

[SugarTable("Comment")]
public class CommentEntity : Entity<Guid>, IAuditedObject
{
    [SugarColumn(IsPrimaryKey = true)]
    public override Guid Id { get; protected set; }
    
    public string? Content { get; set; }

    public DateTime CreationTime { get; set; } = DateTime.Now;
    public Guid? CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }
}