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
        string? summary, Stream? image, string? imageName, string categoryName, List<Guid>? tagIds)
    {
        Guid blogPostId = _guidGenerator.Create();
        Guid fileId = _guidGenerator.Create();
        DateTime today = DateTime.Now;
        String key = $"{today:yyyy/MM/dd}/{blogPostId}";
        String blogKey = $"{key}/{title}.md";
        String imageKey = $"{key}/{imageName}";
        Stream contentStream = StreamHelper.StringToStream(blogContent);

        BlogPostAggregateRoot blogPost = new BlogPostAggregateRoot(blogPostId, author, title, summary);
        
        BlogFileEntity blogFile = await CreateBlogFileAsync(fileId, blogContent, blogKey, imageKey,contentStream, image);
        blogPost.SetFile(blogFile); 
        var category = await _categoryDomainService.CreateOrGetCategoryAsync(categoryName, 0);
        blogPost.SetCategory(category);
        var tagList = await _tagDomainService.GetTagListByIdsAsync(tagIds);
        blogPost.SetTags(tagList);
        
        await CreateBlogPostTagAsync(blogPostId, tagList);

        return blogPost;
    }
    

    private async Task<BlogFileEntity> CreateBlogFileAsync(Guid fileId, string fileContent,string blogkey, string imagekey, Stream content, Stream? image)
    {
        Uri? fileBackUpUrl = null;
        Uri? fileUploadUrl = null;
        Uri? imageBackUpUrl = null;
        Uri? imageUploadUrl = null;
        long fileSizeInBytes = content.Length + (image?.Length ?? 0);
        if (image?.Length > 0)
        {
            (imageBackUpUrl, imageUploadUrl) = await UploadFileAsync(imagekey, image);
        }

        if (content?.Length > 0)
        {
            (fileBackUpUrl, fileUploadUrl) = await UploadFileAsync(blogkey, content);
        }
        
        var blogfile = new BlogFileEntity(fileId, fileSizeInBytes, fileContent ,fileBackUpUrl, fileUploadUrl,
            imageUploadUrl, imageBackUpUrl);
        return blogfile;
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

    private async Task<(Uri, Uri)> UploadFileAsync(string key, Stream content)
    {
        var backupStorageClient = _fileClientResolver.Resolve(StorageType.Local, "Local");
        var uploadStorageClient = _fileClientResolver.Resolve(StorageType.Obs, "HuaWeiYun");
        content.Position = 0;
        Uri backUpUrl = await backupStorageClient.SaveFileAsync(key, content);
        content.Position = 0;
        Uri uploadUrl = await uploadStorageClient.SaveFileAsync(key, content);
        return (backUpUrl, uploadUrl);
    }
    
}