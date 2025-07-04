using SqlSugar;
using Volo.Abp;
using Volo.Abp.Guids;
using Volo.Abp.ObjectMapping;
using YayZent.Framework.Blog.Application.Contracts.Dtos.Category;
using YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Ddd.Application;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Blog.Application.Services;

public class CategoryService: CustomCrudAppService<CatergoryAggregateRoot, CategoryGetOutputDto, CategoryGetListOutputDto, 
    Guid, CategoryGetListInputDto, CatergoryCreateDto, CategoryInputDto>
{
    private readonly IObjectMapper _objectMapper;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ITagDomainService _tagDomainService;
    private readonly ISqlSugarRepository<TagAggregateRoot> _tagRepository;
    private readonly ISqlSugarRepository<CategoryTagEntity> _categoryTagRepository;
    private readonly ISqlSugarRepository<CatergoryAggregateRoot, Guid> _categoryRepository;
    private readonly ISqlSugarRepository<BlogPostAggregateRoot, Guid> _blogPostRepository;

    public CategoryService(
        IObjectMapper objectMapper,
        IGuidGenerator guidGenerator,
        ITagDomainService tagDomainService,
        ISqlSugarRepository<TagAggregateRoot> tagRepository,
        ISqlSugarRepository<CategoryTagEntity> categoryTagRepository,
        ISqlSugarRepository<CatergoryAggregateRoot, Guid> categoryRepository,
        ISqlSugarRepository<BlogPostAggregateRoot, Guid> blogPostRepository) : base(
        categoryRepository)
    {
        _objectMapper = objectMapper;
        _guidGenerator = guidGenerator;
        _tagDomainService = tagDomainService;
        _tagRepository = tagRepository;
        _categoryTagRepository = categoryTagRepository;
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

    public override async Task<CategoryGetOutputDto> CreateAsync(CatergoryCreateDto input)
    {
        var oldCategory = await _categoryRepository.DbQueryable.FirstAsync(x => x.CategoryName == input.CategoryName);

        if (oldCategory is not null)
        {
            throw new BusinessException("Category already exists");
        }

        var category = new CatergoryAggregateRoot(_guidGenerator.Create(), input.CategoryName);
        await _categoryRepository.InsertAsync(category);

        var tagIds = await _tagDomainService.CreateTagsAsync(input.TagNames);
        
        var categoryTags = tagIds.Select(x => new CategoryTagEntity(_guidGenerator.Create(), category.Id, x)).ToList();

        await _categoryTagRepository.InsertManyAsync(categoryTags);

        return new CategoryGetOutputDto(){CategoryId = category.Id};
    }

}