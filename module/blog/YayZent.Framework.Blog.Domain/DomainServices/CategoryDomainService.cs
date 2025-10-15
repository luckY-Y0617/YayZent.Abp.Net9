using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Users;
using YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Blog.Domain.DomainServices;

public class CategoryDomainService: DomainService, ICategoryDomainService
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentUser _currentUser;
    private readonly ISqlSugarRepository<CategoryAggregateRoot> _categoryRepository;

    public CategoryDomainService(IGuidGenerator guidGenerator, ICurrentUser currentUser,
        ISqlSugarRepository<CategoryAggregateRoot> categoryRepository)
    {
        _guidGenerator = guidGenerator;
        _currentUser = currentUser;
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryAggregateRoot> GetAsync(Guid id)
    {
        return await _categoryRepository.GetFirstAsync(x => x.Id == id);
    }

    public async Task<CategoryAggregateRoot> CreateOrGetCategoryAsync(string categoryName, int sequenceNumber)
    {
        var category = await _categoryRepository.DbQueryable.Where(x => x.CategoryName == categoryName).FirstAsync();

        if (category != null)
        {
            return category;
        }
        
        category = new CategoryAggregateRoot(_guidGenerator.Create(), categoryName, sequenceNumber);
        await _categoryRepository.InsertAsync(category);
        
        return category;
    }
}