using SqlSugar;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Guids;

namespace YayZent.Framework.Blog.Domain.Entities;

[SugarTable("BlogPostTag")]
public class BlogPostTagEntity : Entity<Guid>
{
    public Guid BlogPostId { get; protected set; } 
    public Guid TagId { get; protected set; } 
    
    public BlogPostTagEntity() {}

    public BlogPostTagEntity(IGuidGenerator guidGenerator,Guid blogPostId, Guid tagId) : base(guidGenerator.Create())
    {
        BlogPostId = blogPostId;
        TagId = tagId;
    }
}