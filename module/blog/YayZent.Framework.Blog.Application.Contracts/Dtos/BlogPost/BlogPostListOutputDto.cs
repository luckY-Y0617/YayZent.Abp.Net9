using Volo.Abp.Application.Dtos;

namespace YayZent.Framework.Blog.Application.Contracts.Dtos.BlogPost;

public class BlogPostListOutputDto: EntityDto<Guid>
{

    public required string Title { get; set; }
    public string? Summary { get; set; }
    public string? CategoryName { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public string? ImageUrl { get; set; }
    public string? CreationTime { get; set; }
    public string? Views { get; set; }
}