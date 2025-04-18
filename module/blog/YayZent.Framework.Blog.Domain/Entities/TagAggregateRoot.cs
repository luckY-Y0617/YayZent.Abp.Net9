using SqlSugar;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Guids;

namespace YayZent.Framework.Blog.Domain.Entities;

[SugarTable("Tag")]
[SugarIndex("idx_tagname", nameof(TagName), OrderByType.Asc, IsUnique = true)]
public class TagAggregateRoot : Entity<Guid>
{
    [SugarColumn(IsNullable = false)]
    public string TagName { get; set; }
    
    [Navigate(typeof(BlogPostAggregateRoot), nameof(BlogPostTagEntity.TagId), nameof(BlogPostTagEntity.BlogPostId))]
    public List<BlogPostAggregateRoot>? BlogPosts { get; set; }
    
    public TagAggregateRoot()  {}

    public TagAggregateRoot(IGuidGenerator guidGenerator, string tagName) : base(guidGenerator.Create())
    {
        TagName = tagName;
    }
}