using SqlSugar;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Guids;

namespace YayZent.Framework.Blog.Domain.Entities;

[SugarTable("Tag")]
[SugarIndex("idx_tagname", nameof(TagName), OrderByType.Asc, IsUnique = true)]
public class TagAggregateRoot : AuditedAggregateRoot<Guid>
{
    [SugarColumn(IsNullable = false)]
    public string TagName { get; set; }
    
    /// <summary>
    /// 在所有Category中的显示序号，越小越靠前
    /// </summary>
    public int SequenceNumber { get; set; } = 0;
    
    public Guid CategoryId { get; set; }
    
    [Navigate(typeof(BlogPostAggregateRoot), nameof(BlogPostTagEntity.TagId), nameof(BlogPostTagEntity.BlogPostId))]
    public List<BlogPostAggregateRoot>? BlogPosts { get; set; }
    
    public TagAggregateRoot()  {}

    public TagAggregateRoot(IGuidGenerator guidGenerator, string tagName) : base(guidGenerator.Create())
    {
        TagName = tagName;
    }
}