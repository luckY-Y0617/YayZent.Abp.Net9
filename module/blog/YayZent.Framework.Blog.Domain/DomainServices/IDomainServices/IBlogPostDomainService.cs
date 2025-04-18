using Volo.Abp.Domain.Services;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Blog.Domain.Shared.Dtos;

namespace YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;

public interface IBlogPostDomainService: IDomainService
{
    Task<BlogPostAggregateRoot> CreateBlogPostAsync(CreateBlogPostParameterDto param);
}