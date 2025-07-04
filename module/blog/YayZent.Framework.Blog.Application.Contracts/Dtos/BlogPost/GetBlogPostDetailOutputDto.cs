namespace YayZent.Framework.Blog.Application.Contracts.Dtos.BlogPost;

public class GetBlogPostDetailOutputDto
{
    public string? Content { get; set; }
    
    public string? Title { get; set; }
    
    public string? Author { get; set; }
    
    public string? CreationTime { get; set; }
    
    public string? RevisonTime { get; set; }
}