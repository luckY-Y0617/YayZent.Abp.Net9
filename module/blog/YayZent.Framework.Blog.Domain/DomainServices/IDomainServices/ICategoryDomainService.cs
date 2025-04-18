using Volo.Abp.Domain.Services;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Blog.Domain.Shared.Dtos.Category;

namespace YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;

public interface ICategoryDomainService: IDomainService
{
    Task<CatergoryAggregateRoot> CreateOrGetCategoryAsync(CreateCategoryParameterDto param);
}