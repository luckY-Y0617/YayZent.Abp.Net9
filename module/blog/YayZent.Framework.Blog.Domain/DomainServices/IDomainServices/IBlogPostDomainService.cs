using Volo.Abp.Domain.Services;
using YayZent.Framework.Blog.Domain.Entities;

namespace YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;

public interface IBlogPostDomainService: IDomainService
{
    Task<BlogPostAggregateRoot> CreateBlogPostAsync(string title, string blogContent, string author, 
        string? summary, Stream? image, string? imageName, string categoryName, List<Guid>? tagIds);
}