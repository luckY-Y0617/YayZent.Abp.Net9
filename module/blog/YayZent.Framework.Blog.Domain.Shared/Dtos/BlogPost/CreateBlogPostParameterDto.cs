namespace YayZent.Framework.Blog.Domain.Shared.Dtos;

public class CreateBlogPostParameterDto
{
    /// <summary>
    /// 标题
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Blog内容
    /// </summary>
    public required string BlogContent { get; set; }
    
    public required string Author { get; set; }
    
    /// <summary>
    /// Blog摘要
    /// </summary>
    public string? Summary { get; set; }
    
    /// <summary>
    /// Blog封面
    /// </summary>
    public Stream? Image { get; set; }
    
    public string? ImageName { get; set; }
    
    public string? Category { get; set; }
    
    public List<Guid>? TagIds { get; set; }
}