namespace YayZent.Framework.Blog.Application.Contracts.Dtos;

public class GetBlogPostDetailResponse
{
    public string? Content { get; set; }
    
    public string? Title { get; set; }
    
    public string? Author { get; set; }
    
    public DateTimeOffset? CreationTime { get; set; }
    
    public DateTimeOffset? RevisonTime { get; set; }
}