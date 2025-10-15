using Microsoft.AspNetCore.Http;

namespace YayZent.Framework.Blog.Application.Contracts.Dtos.BlogPost;

public class UpdateBlogInputDto
{
    public Guid BlogId { get; set; }
    
    public required string Title { get; set; }

    public required string BlogContent { get; set; }

    public required string Author { get; set; } 
    
    public Guid CategoryId { get; set; }
    
    public IFormFile? Image { get; set; }

    public string? Summary { get; set; }
    public string TagIds { get; set; }
}