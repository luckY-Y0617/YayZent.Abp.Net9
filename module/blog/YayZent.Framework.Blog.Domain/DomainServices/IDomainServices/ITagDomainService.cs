using Volo.Abp.Domain.Services;
using YayZent.Framework.Blog.Domain.Entities;

namespace YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;

public interface ITagDomainService: IDomainService
{
    Task<List<TagAggregateRoot>?> GetTagListByIdsAsync(List<Guid>? tagIds);

    Task<List<Guid>?> CreateTagsAsync(List<string>? tags);
}