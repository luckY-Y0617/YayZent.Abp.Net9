using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace YayZent.Framework.Blog.Domain.Entities;

public class BlogAggregateRoot : AggregateRoot<Guid>, IAuditedObject, ISoftDelete
{
    public DateTime CreationTime { get; }
    public Guid? CreatorId { get; }
    public DateTime? LastModificationTime { get; }
    public Guid? LastModifierId { get; }
    public bool IsDeleted { get; }
}