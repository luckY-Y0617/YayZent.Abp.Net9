using Volo.Abp.Domain.Services;
using YayZent.Framework.Blog.Domain.Entities;

namespace YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;

public interface ICategoryDomainService: IDomainService
{
    Task<CategoryAggregateRoot> CreateOrGetCategoryAsync(string categoryName, int sequenceNumber);

    Task<CategoryAggregateRoot> GetAsync(Guid id);
}