using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;
using YayZent.Framework.Auth.Domain.Shared.Authorization;
using YayZent.Framework.Blog.Application.Contracts.Dtos;
using YayZent.Framework.Blog.Application.Contracts.Dtos.BlogPost;
using YayZent.Framework.Blog.Application.Contracts.IServices;
using YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Blog.Domain.Repositories;
using YayZent.Framework.Core.Rendering.Markdown;
using YayZent.Framework.Ddd.Application.Contracts.Dtos;

namespace YayZent.Framework.Blog.Application.Services;

public class BlogPostService : ApplicationService, IBlogPostService
{
    private readonly IObjectMapper _objectMapper;
    private readonly IBlogPostDomainService _blogPostDomainService;
    private readonly ITagDomainService _tagDomainService;
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IMarkdownRenderService _markdownRenderer;

    public BlogPostService(IBlogPostDomainService blogPostDomainService, ITagDomainService tagDomainService, 
        IBlogPostRepository blogPostRepository, IMarkdownRenderService markdownRenderer,  IObjectMapper objectMapper)
    {
        _blogPostDomainService = blogPostDomainService;
        _tagDomainService = tagDomainService;
        _blogPostRepository = blogPostRepository;
        _markdownRenderer = markdownRenderer;
        _objectMapper = objectMapper;
    }


    [Authorize]
    [Permission("blog:add")]
    public async Task<ApiResponse<CreateBlogPostOutputDto>> CreateAsync([FromForm] CreateBlogPostInputDto input)
    {
        string? imageName = input.Image?.FileName;
        await using var imageStream = input.Image?.OpenReadStream();  
        
        var tagList = input.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        
        var tagIds = await _tagDomainService.CreateTagsAsync(tagList);

        // 调用 domain service 创建 BlogPost
        BlogPostAggregateRoot blogPost = await _blogPostDomainService.CreateBlogPostAsync(input.Title, input.BlogContent,
            input.Author, input.Summary, imageStream, imageName, input.Category, tagIds);

        var result = new CreateBlogPostOutputDto()
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
        };
        
        await _blogPostRepository.DbContext.InsertNav(blogPost)
            .Include(x => x.BlogFile) // 插入导航 BlogFile
            .Include(x => x.Tags)
            .Include(x => x.Catergory)
            .ExecuteCommandAsync();

        return ApiResponse<CreateBlogPostOutputDto>.Ok(result);
    }


    public async Task<ApiResponse<PagedResultDto<BlogPostListOutputDto>>> GetBlogPostListAsync(BlogPostListInputDto input)
    {
        DateTime? startTime = null;
        DateTime? endTime = null;
        
        if (!string.IsNullOrEmpty(input.YearMonth))
        {
            startTime = DateTime.ParseExact(input.YearMonth, "yyyy-MM", null);
            endTime = startTime?.AddMonths(1).AddTicks(-1);
        }
        
        var totalCountRef = new RefAsync<int>();
        
        var items = await _blogPostRepository.DbQueryable
            .Includes(x => x.BlogFile)
            .Includes(x => x.Tags)
            .Includes(x => x.Catergory)
            .WhereIF(input.CategoryId != Guid.Empty, x => x.CategoryId == input.CategoryId)
            .WhereIF(startTime.HasValue, x => x.CreationTime >= startTime && x.CreationTime <= endTime)
            .OrderBy(x => x.CreationTime, OrderByType.Desc)
            .ToPageListAsync(input.CurrentPage, input.MaxResultCount, totalCountRef);

        var rs = new PagedResultDto<BlogPostListOutputDto>(totalCountRef,
            _objectMapper.Map<List<BlogPostAggregateRoot>, List<BlogPostListOutputDto>>(items));

        return ApiResponse<PagedResultDto<BlogPostListOutputDto>>.Ok(rs);
    }

    public async Task<ApiResponse<GetBlogPostDetailOutputDto>> GetBlogPostDetailAsync(GetBlogPostDetailInputDto input)
    {
        var blogPostDetail = new GetBlogPostDetailOutputDto();

        try
        {
            var blogPost = await _blogPostRepository.DbQueryable.Includes(x => x.BlogFile)
                .FirstAsync(x => x.Id == input.BlogPostId);
            var blogFile = blogPost.BlogFile;
            blogPost.AddViews();
            await _blogPostRepository.UpdateAsync(blogPost);
            
            if (blogFile == null)
            {
                throw new UserFriendlyException("Blog content not found");
            }

            blogPostDetail.Content = _markdownRenderer.ToHtml(blogFile.FileContent);
            blogPostDetail.Title = blogPost.Title;
            blogPostDetail.Author = blogPost.Author;
            blogPostDetail.CreationTime = blogPost.CreationTime.ToString("yyyy-MM-dd HH:mm:ss");
            blogPostDetail.RevisonTime = blogPost.LastModificationTime?.ToString("yyyy-MM-dd HH:mm:ss");

        }
        catch (Exception ex)
        {
            return ApiResponse<GetBlogPostDetailOutputDto>.FailWithData();
        }
        
        return ApiResponse<GetBlogPostDetailOutputDto>.Ok(blogPostDetail);
    }

    public async Task<ApiResponse<List<GetSortedBlogPostsOutputDto>?>> GetSortedBlogPostsAsync(GetSortedBlogPostsInputDto input)
    {
        var query = _blogPostRepository.DbQueryable;

        // 按照是否排序决定查询顺序
        if (input.IsSorted)
        {
            query = query.OrderBy(x => x.CreationTime, OrderByType.Desc);
        }
        else
        {
            query = query.OrderBy(x => SqlFunc.GetRandom());
        }
        
        var items = await query
            .Select(x => new GetSortedBlogPostsOutputDto{ Id = x.Id, Title = x.Title})
            .Take(input.Quantity)
            .ToListAsync();
        
        return ApiResponse<List<GetSortedBlogPostsOutputDto>?>.Ok(items);
    }
}