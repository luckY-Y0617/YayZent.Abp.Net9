using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using YayZent.Framework.Blog.Application.Contracts.Dtos;
using YayZent.Framework.Blog.Application.Contracts.Dtos.BlogPost;
using YayZent.Framework.Blog.Application.Contracts.IServices;
using YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Blog.Domain.Repositories;
using YayZent.Framework.Blog.Domain.Shared.Dtos;
using YayZent.Framework.Core.Rendering.Markdown;
using YayZent.Framework.Ddd.Application.Contracts.Dtos;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Blog.Application.Services;

[RemoteService]
public class BlogPostService : ApplicationService, IBlogPostService
{
    private readonly IBlogPostDomainService _blogPostDomainService;
    private readonly ITagDomainService _tagDomainService;
    private readonly ITagRepository _tagRepository;
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly ISqlSugarRepository<BlogFileEntity> _blogFileRepository;
    private readonly IMarkdownRenderService _markdownRenderer;
    

    public BlogPostService(IBlogPostDomainService blogPostDomainService, ITagDomainService tagDomainService,ITagRepository tagRepository, IBlogPostRepository blogPostRepository,
        ISqlSugarRepository<BlogFileEntity> blogFileRepository,IMarkdownRenderService markdownRenderer)
    {
        _blogPostDomainService = blogPostDomainService;
        _tagDomainService = tagDomainService;
        _tagRepository = tagRepository;
        _blogPostRepository = blogPostRepository;
        _blogFileRepository = blogFileRepository;
        _markdownRenderer = markdownRenderer;
    }

    public async Task<ApiResponse<CreateBlogPostResponse>> CreateAsync([FromForm] CreateBlogPostRequest request)
    {
        string? imageName = request.Image?.FileName;
        await using var imageStream = request.Image?.OpenReadStream();  
        
        var tagList = request.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        
        var tagIds = await _tagDomainService.CreateTagsAsync(tagList);

        // 将 input 转换为 domain 层参数 DTO
        var domainParam = new CreateBlogPostParameterDto
        {
            Author = request.Author,
            Title = request.Title,
            Summary = request.Summary,
            Category = request.Category,
            TagIds = tagIds,
            BlogContent = request.BlogContent,         
            Image = imageStream,
            ImageName = imageName
        };

        // 调用 domain service 创建 BlogPost
        BlogPostAggregateRoot blogPost = await _blogPostDomainService.CreateBlogPostAsync(domainParam);

        var result = new CreateBlogPostResponse()
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
        };
        
        await _blogPostRepository.DbContext.InsertNav(blogPost)
            .Include(x => x.BlogFile) // 插入导航 BlogFile
            .Include(x => x.Tags)
            .Include(x => x.Catergory)
            .ExecuteCommandAsync();

        return ApiResponse<CreateBlogPostResponse>.Ok(result);
    }

    public async Task<ApiResponse<GetBlogPostListResponse>> GetBlogPostListAsync(GetBlogPostListRequest request)
    {
        DateTime? startTime = null;
        DateTime? endTime = null;

        if (!string.IsNullOrEmpty(request.YearMonth))
        {
            startTime = DateTime.ParseExact(request.YearMonth, "yyyy-MM", null);
            endTime = startTime?.AddMonths(1).AddTicks(-1);
        }
        
        var totalCountRef = new RefAsync<int>();
        
        var items = await _blogPostRepository.DbQueryable
            .Includes(x => x.BlogFile)
            .Includes(x => x.Tags)
            .Includes(x => x.Catergory)
            .WhereIF(request.CategoryId != Guid.Empty, x => x.CategoryId == request.CategoryId)
            .WhereIF(startTime.HasValue, x => x.CreationTime >= startTime && x.CreationTime <= endTime)
            .OrderBy(x => x.CreationTime, OrderByType.Desc)
            .ToPageListAsync(request.CurrentPage, request.MaxResultCount, totalCountRef);

        var rs = new GetBlogPostListResponse()
        {
            TotalCount = totalCountRef.Value,
            
            BlogPostList = items.Select(x => new GetBlogPostListResponse.BlogPostDetail
            {
                Id = x.Id,
                Title = x.Title,
                Summary = x.Summary,
                Category = x.Catergory?.CategoryName,
                ImageUrl = x.BlogFile?.ImageUploadUrl,
                CreationTime = x.CreationTime
            }).ToList()
        };

        return ApiResponse<GetBlogPostListResponse>.Ok(rs);
    }

    public async Task<ApiResponse<GetBlogPostDetailResponse>> GetBlogPostContentAsync(GetBlogPostDetailRequest request)
    {
        var blogPostDetail = new GetBlogPostDetailResponse();

        try
        {
            var blogPost = await _blogPostRepository.DbQueryable.Includes(x => x.BlogFile)
                .FirstAsync(x => x.Id == request.BlogPostId);
            var blogFile = blogPost.BlogFile;

            if (blogFile == null)
            {
                throw new UserFriendlyException("Blog content not found");
            }

            blogPostDetail.Content = _markdownRenderer.ToHtml(blogFile.FileContent);
            blogPostDetail.Title = blogPost.Title;
            blogPostDetail.Author = blogPost.Author;
            blogPostDetail.CreationTime = blogPost.CreationTime;
            blogPostDetail.RevisonTime = blogPost.LastModificationTime;

        }
        catch (Exception ex)
        {
            return ApiResponse<GetBlogPostDetailResponse>.FailWithData();
        }
        
        return ApiResponse<GetBlogPostDetailResponse>.Ok(blogPostDetail);
    }
}