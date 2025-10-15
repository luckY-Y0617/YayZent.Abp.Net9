using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Users;
using YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Blog.Domain.Repositories;
using YayZent.Framework.Core.Helper;
using YayZent.Framework.Core.File.Abstractions;
using YayZent.Framework.Core.File.Enums;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Blog.Domain.DomainServices;

// 业务是为用户提供价值的规则和流程，技术只是实现手段。
public class BlogPostDomainService: DomainService, IBlogPostDomainService
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentUser _currentUser;
    private readonly IFileClientResolver _fileClientResolver;
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly ISqlSugarRepository<BlogFileEntity> _blogFileRepository;
    private readonly ISqlSugarRepository<BlogPostTagEntity> _blogPostTagRepository;
    private readonly ITagDomainService _tagDomainService;
    private readonly ICategoryDomainService _categoryDomainService;

    public BlogPostDomainService(
        IGuidGenerator guidGenerator,
        IBlogPostRepository blogPostRepository,
        ICurrentUser currentUser,
        IFileClientResolver fileClientResolver,
        ISqlSugarRepository<BlogFileEntity> blogFileRepository,
        ITagRepository tagRepository,
        ISqlSugarRepository<BlogPostTagEntity> blogPostTagRepository,
        ITagDomainService tagDomainService,
        ICategoryDomainService categoryDomainService)
    {
        _guidGenerator = guidGenerator;
        _blogPostRepository = blogPostRepository;
        _currentUser = currentUser;
        _fileClientResolver = fileClientResolver;
        _blogFileRepository = blogFileRepository;
        _blogPostTagRepository = blogPostTagRepository;
        _tagDomainService = tagDomainService;
        _categoryDomainService = categoryDomainService;
    }
    
    public async Task<BlogPostAggregateRoot> CreateBlogPostAsync(string title, string blogContent, string author, 
        string? summary, Guid categoryId, List<Guid>? tagIds)
    {
        Guid blogPostId = _guidGenerator.Create();
        
        BlogPostAggregateRoot blogPost = new BlogPostAggregateRoot(blogPostId, author, title, summary);

        BlogFileEntity blogFile = new BlogFileEntity(_guidGenerator.Create(), blogContent);
        blogPost.SetFile(blogFile); 
        var category = await _categoryDomainService.GetAsync(categoryId);
        blogPost.SetCategory(category);
        
        var tagList = await _tagDomainService.GetTagListByIdsAsync(tagIds);
        blogPost.SetTags(tagList);
        await CreateBlogPostTagAsync(blogPostId, tagList);

        return blogPost;
    }

    public async Task<BlogPostAggregateRoot> UpdateBlogPostAsync(BlogPostAggregateRoot blogPost, string title, string author,
        string? summary, Guid categoryId, List<Guid>? tagIds)
    {
        blogPost.Title = title;
        blogPost.Author = author;
        blogPost.Summary = summary;
        
        var category = await _categoryDomainService.GetAsync(categoryId);
        blogPost.SetCategory(category);
        
        var tagList = await _tagDomainService.GetTagListByIdsAsync(tagIds);
        blogPost.SetTags(tagList);
        await CreateBlogPostTagAsync(blogPost.Id, tagList);

        return blogPost;
    }


    private async Task CreateBlogPostTagAsync(Guid blogPostId, List<TagAggregateRoot>? tags)
    {
        if (tags == null)
        {
            return;
        }
        
        var blogPostTagEntities = tags
            .Select(tag => new BlogPostTagEntity(_guidGenerator, tag.Id, blogPostId))
            .ToList();

        await _blogPostTagRepository.InsertManyAsync(blogPostTagEntities);
    }

}