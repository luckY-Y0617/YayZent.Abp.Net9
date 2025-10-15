using Volo.Abp.Domain.Services;
using YayZent.Framework.Blog.Domain.Entities;

namespace YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;

public interface IBlogPostDomainService: IDomainService
{
    Task<BlogPostAggregateRoot> CreateBlogPostAsync(string title, string blogContent, string author, 
        string? summary, Guid categoryId, List<Guid>? tagIds);

    Task<BlogPostAggregateRoot> UpdateBlogPostAsync(BlogPostAggregateRoot blogPost, string title, string author,
        string? summary, Guid categoryId, List<Guid>? tagIds);

}