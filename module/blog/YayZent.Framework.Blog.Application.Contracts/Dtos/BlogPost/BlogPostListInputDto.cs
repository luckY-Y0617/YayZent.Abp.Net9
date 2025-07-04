using Volo.Abp.Application.Dtos;

namespace YayZent.Framework.Blog.Application.Contracts.Dtos.BlogPost;

public class BlogPostListInputDto: PagedAndSortedResultRequestDto
{
    public Guid CategoryId { get; set; }
    
    public string? YearMonth { get; set; }
    
    public required int CurrentPage { get; set; } = 1;
    
}