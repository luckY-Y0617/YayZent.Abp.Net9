namespace YayZent.Framework.Blog.Application.Contracts.Dtos.BlogPost;

public class GetBlogForUpdateOutputDto
{
    public string? Title { get; set; } 
    
    public string? Summary { get; set; } 
    
    public Guid? CategoryId { get; set; } 
    
    public List<Guid>? TagIds { get; set; } 
    
    public string? CoverImgUrl { get; set; } 
    
    public string? Content { get; set; }
}