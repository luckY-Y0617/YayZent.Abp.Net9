using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.EventBus.Local;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using YayZent.Framework.Auth.Domain.Shared.Authorization;
using YayZent.Framework.Blog.Application.Contracts.Dtos;
using YayZent.Framework.Blog.Application.Contracts.Dtos.BlogPost;
using YayZent.Framework.Blog.Application.Contracts.IServices;
using YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Blog.Domain.Repositories;
using YayZent.Framework.Blog.Domain.Shared.Etos;
using YayZent.Framework.Core.File.Storage;
using YayZent.Framework.Core.Helper;
using YayZent.Framework.Core.Rendering.Markdown;
using YayZent.Framework.Ddd.Application.Contracts.Dtos;

namespace YayZent.Framework.Blog.Application.Services;

public class BlogPostService : ApplicationService, IBlogPostService
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUser _currentUser;
    private readonly IObjectMapper _objectMapper;
    private readonly IBlogPostDomainService _blogPostDomainService;
    private readonly ITagDomainService _tagDomainService;
    private readonly IBlogPostRepository _blogPostRepository;
    private ILocalEventBus LocalEventBus => LazyServiceProvider.LazyGetRequiredService<ILocalEventBus>();

    public BlogPostService(IBlogPostDomainService blogPostDomainService, ITagDomainService tagDomainService, ICurrentUser currentUser,
        IBlogPostRepository blogPostRepository, IObjectMapper objectMapper, IFileStorageService fileStorageService)
    {
        _blogPostDomainService = blogPostDomainService;
        _tagDomainService = tagDomainService;
        _blogPostRepository = blogPostRepository;
        _objectMapper = objectMapper;
        _currentUser = currentUser;
        _fileStorageService = fileStorageService;
    }


    [Authorize]
    [Permission("blog:add")]
    public async Task<ApiResponse<CreateBlogPostOutputDto>> CreateAsync([FromForm] CreateBlogPostInputDto input)
    {
        await using var imageStream = input.Image?.OpenReadStream();
        var imagePath = await _fileStorageService.SaveTempFileAsync(imageStream);
        // Stream contentStream = StreamHelper.StringToStream(input.BlogContent);
        // var filePath = await _fileStorageService.SaveTempFileAsync(contentStream);
        
        var tagIds = JsonConvert.DeserializeObject<List<Guid>>(input.TagIds);
        
        // 调用 domain service 创建 BlogPost
        BlogPostAggregateRoot blogPost = await _blogPostDomainService.CreateBlogPostAsync(input.Title, input.BlogContent,
            input.Author, input.Summary, input.CategoryId, tagIds);

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

        await LocalEventBus.PublishAsync(new BlogCreatedEventArgs() {BlogFileId = blogPost.BlogFile.Id, ContentPath = null, ImagePath = imagePath});

        return ApiResponse<CreateBlogPostOutputDto>.Ok(result);
    }

    [HttpPost]
    [Authorize]
    [Permission("blog:add")]
    public async Task<ApiResponse> UpdateAsync([FromForm]UpdateBlogInputDto input)
    {
        string? imagePath = null;
        
        if (input.Image != null)
        {
            await using var imageStream = input.Image?.OpenReadStream();
            imagePath = await _fileStorageService.SaveTempFileAsync(imageStream);
        }
        
        Stream contentStream = StreamHelper.StringToStream(input.BlogContent);
        var filePath = await _fileStorageService.SaveTempFileAsync(contentStream);
        
        var blog  = await _blogPostRepository.DbQueryable
            .Includes(x => x.BlogFile)
            .Includes(x => x.Catergory)
            .Includes(x => x.Tags)
            .FirstAsync(x => x.Id == input.BlogId);
        
        var tagIds = JsonConvert.DeserializeObject<List<Guid>>(input.TagIds);
        
        blog.BlogFile.FileContent = input.BlogContent;
        await _blogPostDomainService.UpdateBlogPostAsync(blog, input.Title, input.Author, input.Summary, input.CategoryId, tagIds);

        await _blogPostRepository.DbContext
            .UpdateNav(blog)
            .Include(x => x.Tags, new UpdateNavOptions() { ManyToManyIsUpdateA = true })
            .Include(x => x.Catergory)
            .Include(x => x.BlogFile)
            .ExecuteCommandAsync();

        await LocalEventBus.PublishAsync(new BlogUpdatedEventArgs()
            { BlogFileId = blog.BlogFileId, ImagePath = imagePath, ContentPath = filePath });
        
        return ApiResponse.Ok();
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

        var query = _blogPostRepository.DbQueryable
            .Includes(x => x.BlogFile)
            .Includes(x => x.Tags)
            .Includes(x => x.Catergory)
            .WhereIF(input.CategoryId != Guid.Empty, x => x.CategoryId == input.CategoryId)
            .WhereIF(startTime.HasValue, x => x.CreationTime >= startTime && x.CreationTime <= endTime);

        // 加上 TagId 筛选（多对多）
        if (input.TagId.HasValue && input.TagId != Guid.Empty)
        {
            query = query.Where(x => x.Tags.Any(t => t.Id == input.TagId.Value));
        }

        var items = await query
            .OrderBy(x => x.CreationTime, OrderByType.Desc)
            .ToPageListAsync(input.CurrentPage, input.MaxResultCount, totalCountRef);

        var rs = new PagedResultDto<BlogPostListOutputDto>(
            totalCountRef,
            _objectMapper.Map<List<BlogPostAggregateRoot>, List<BlogPostListOutputDto>>(items)
        );

        return ApiResponse<PagedResultDto<BlogPostListOutputDto>>.Ok(rs);
    }

    [Authorize]
    public async Task<ApiResponse<PagedResultDto<BlogPostListOutputDto>>> GetUserBlogsAsync(
        BlogPostListInputDto input)
    {
        var totalCountRef = new RefAsync<int>();
        
        var items = await _blogPostRepository.DbQueryable
            .Includes(x => x.BlogFile)   
            .Includes(x => x.Tags)
            .Includes(x => x.Catergory)
            .Where(x => x.CreatorId == _currentUser.Id)
            .OrderBy(x => x.CreationTime, OrderByType.Desc)
            .ToPageListAsync(input.CurrentPage, input.MaxResultCount, totalCountRef);
        
        var rs = new PagedResultDto<BlogPostListOutputDto>(totalCountRef,
            _objectMapper.Map<List<BlogPostAggregateRoot>, List<BlogPostListOutputDto>>(items));

        return ApiResponse<PagedResultDto<BlogPostListOutputDto>>.Ok(rs);
    }

    [Authorize]
    public async Task<ApiResponse<GetBlogForUpdateOutputDto>> GetBlogForUpdateAsync(GetBlogPostDetailInputDto input)
    {
        var blogContent = new GetBlogForUpdateOutputDto();

        try
        {
            var blogPost = await _blogPostRepository.DbQueryable
                .Includes(x => x.BlogFile)
                .Includes(x => x.Catergory)
                .Includes(x => x.Tags)
                .FirstAsync(x => x.Id == input.BlogId);

            if (blogPost == null)
            {
                throw new UserFriendlyException("Blog content not found");
            }

            blogContent.Title = blogPost.Title;
            blogContent.CategoryId = blogPost.Catergory?.Id;
            blogContent.TagIds = blogPost.Tags?.Select(x => x.Id).ToList();
            blogContent.CoverImgUrl = blogPost.BlogFile?.ImageUploadUrl;
            blogContent.Summary = blogPost.Summary;
            blogContent.Content = blogPost.BlogFile?.FileContent;
        }
        catch (Exception ex)
        {
            return ApiResponse<GetBlogForUpdateOutputDto>.FailWithData();
        }
        return ApiResponse<GetBlogForUpdateOutputDto>.Ok(blogContent);
    } 

    public async Task<ApiResponse<GetBlogPostDetailOutputDto>> GetBlogPostDetailAsync(GetBlogPostDetailInputDto input)
    {
        var blogPostDetail = new GetBlogPostDetailOutputDto();

        try
        {
            var blogPost = await _blogPostRepository.DbQueryable
                .Includes(x => x.BlogFile)
                .FirstAsync(x => x.Id == input.BlogId);
            var blogFile = blogPost.BlogFile;
            blogPost.AddViews();
            await _blogPostRepository.UpdateAsync(blogPost);
            
            if (blogFile == null)
            {
                throw new UserFriendlyException("Blog content not found");
            }

            blogPostDetail.Content = blogFile.FileContent;
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

    [HttpDelete]
    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var blog = await _blogPostRepository.GetAsync(id);
        await _tagDomainService.DeleteByBlogIdAsync(id);
        await LocalEventBus.PublishAsync(new BlogDeletedEventArgs() { BlogFileId = blog.BlogFileId }); 
        await _blogPostRepository.DeleteManyAsync([id]);
        return ApiResponse.Ok();
    }

    public async Task<ApiResponse<List<TagListOutputDto>>> GetTagNames(TagListInputDto input)
    {
        var query = _blogPostRepository.DbQueryable
            .Includes(x => x.Tags)
            .WhereIF(input.CategoryId != Guid.Empty, x => x.CategoryId == input.CategoryId);

        var items = await query.ToListAsync();

        var tagDtos = items
            .SelectMany(x => x.Tags)
            .Where(t => t != null && !string.IsNullOrWhiteSpace(t.TagName))
            .GroupBy(t => t.Id) // 按 Id 去重
            .Select(g => new TagListOutputDto()
            {
                Id = g.Key,
                TagName = g.First().TagName
            })
            .ToList();
        
        return ApiResponse<List<TagListOutputDto>>.Ok(tagDtos);
    }
}