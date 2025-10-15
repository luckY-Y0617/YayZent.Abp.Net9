using SqlSugar;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using YayZent.Framework.Blog.Domain.Shared.Enums;

namespace YayZent.Framework.Blog.Domain.Entities;

[SugarTable("BlogPost")]
public class BlogPostAggregateRoot : AuditedAggregateRoot<Guid>, ISoftDelete
{
    
    /// <summary>
    /// 博客文件Id
    /// </summary>
    public Guid BlogFileId { get; set; }
    
    /// <summary>
    /// 分类
    /// </summary>
    public Guid CategoryId { get; set; }
    
    /// <summary>
    /// 作者
    /// </summary>
    public string Author { get; set; } = string.Empty;
    
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 摘要
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// 浏览数
    /// </summary>
    public int Views { get; set; } = 0;

    /// <summary>
    /// 点赞数
    /// </summary>
    public int Likes { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    public BlogPostStatus Status { get; set; } = BlogPostStatus.Draft;

    /// <summary>
    /// 删除逻辑
    /// </summary>
    public bool IsDeleted { get; set; } = false;


    [Navigate(NavigateType.OneToMany, nameof(CommentAggregateRoot.BlogPostId))]
    public List<CommentAggregateRoot>? Comments { get; private set; }

    [Navigate(typeof(BlogPostTagEntity), nameof(BlogPostTagEntity.BlogPostId), nameof(BlogPostTagEntity.TagId))]
    public List<TagAggregateRoot>? Tags { get; private set; }

    [Navigate(NavigateType.OneToOne, nameof(BlogFileId), nameof(BlogFileEntity.Id))]
    public BlogFileEntity? BlogFile { get; private set; }
    
    [Navigate((NavigateType.ManyToOne), nameof(CategoryId), nameof(CategoryAggregateRoot.Id))]
    public CategoryAggregateRoot? Catergory { get; private set; }

    public BlogPostAggregateRoot()
    {
    }
    
    public BlogPostAggregateRoot(Guid id ,string author, string title, string? summary) : base(id)
    {
        Title = title;
        Summary = summary;
        Author = author;
    }

    public void SetTags(List<TagAggregateRoot>? tags)
    {
        Tags = tags;
    }

    public void SetFile(BlogFileEntity? file)
    {
        BlogFile = file;
    }

    public void SetCategory(CategoryAggregateRoot? catergory)
    {
        Catergory = catergory;
    }

    public void SetComments(List<CommentAggregateRoot>? comments)
    {
        Comments = comments;
    }

    public void AddViews()
    {
        Views++;
    }
}