using Volo.Abp.ObjectMapping;
using YayZent.Framework.Blog.Application.Contracts.Dtos.Category;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Ddd.Application;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Blog.Application.Services;

public class CategoryService: CustomCrudAppService<CategoryAggregateRoot, CategoryGetOutputDto, CategoryGetListOutputDto, 
    Guid, CategoryGetListInputDto, CatergoryCreateDto, CategoryUpdateDto>
{
    private readonly IObjectMapper _objectMapper;
    private readonly ISqlSugarRepository<CategoryAggregateRoot, Guid> _categoryRepository;
    private readonly ISqlSugarRepository<BlogPostAggregateRoot, Guid> _blogPostRepository;

    public CategoryService(
        IObjectMapper objectMapper,
        ISqlSugarRepository<CategoryAggregateRoot, Guid> categoryRepository,
        ISqlSugarRepository<BlogPostAggregateRoot, Guid> blogPostRepository) : base(
        categoryRepository)
    {
        _objectMapper = objectMapper;
        _categoryRepository = categoryRepository;
        _blogPostRepository = blogPostRepository;
    }

    public async Task<List<MenuOutputDto>> GetMenu()
    {
        var categoryList = await _categoryRepository.DbQueryable.Where(x => x.SequenceNumber > 0).ToListAsync();
        
        var categoryIds = categoryList.Select(x => x.Id).ToList();

        var blogDict = (await _blogPostRepository.DbQueryable
            .Where(x => categoryIds.Contains(x.CategoryId))
            .ToListAsync())
            .GroupBy(x => x.CategoryId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var rs = categoryList.Select(x => new MenuOutputDto()
        {
            CategoryName = x.CategoryName,
            Children = blogDict.TryGetValue(x.Id, out var blogs) 
                ? _objectMapper.Map<List<BlogPostAggregateRoot>, List<MenuItem>>(blogs) 
                : new List<MenuItem>()
        }).ToList();
        
        return rs;
    }

}