using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Users;
using YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Blog.Domain.Shared.Dtos.Category;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Blog.Domain.DomainServices;

public class CategoryDomainService: DomainService, ICategoryDomainService
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentUser _currentUser;
    private readonly ISqlSugarRepository<CatergoryAggregateRoot> _categoryRepository;

    public CategoryDomainService(IGuidGenerator guidGenerator, ICurrentUser currentUser,
        ISqlSugarRepository<CatergoryAggregateRoot> categoryRepository)
    {
        _guidGenerator = guidGenerator;
        _currentUser = currentUser;
        _categoryRepository = categoryRepository;
    }

    public async Task<CatergoryAggregateRoot> CreateOrGetCategoryAsync(CreateCategoryParameterDto param)
    {
        var category = await _categoryRepository.DbQueryable.Where(x => x.CategoryName == param.CategoryName).FirstAsync();

        if (category != null)
        {
            return category;
        }
        
        category = new CatergoryAggregateRoot(_guidGenerator.Create(), param.CategoryName, param.SequenceNumber);
        await _categoryRepository.InsertAsync(category);
        
        return category;
    }
}