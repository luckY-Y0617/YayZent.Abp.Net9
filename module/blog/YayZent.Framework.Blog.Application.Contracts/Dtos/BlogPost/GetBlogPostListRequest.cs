using Volo.Abp.Application.Dtos;

namespace YayZent.Framework.Blog.Application.Contracts.Dtos;

public class GetBlogPostListRequest: PagedAndSortedResultRequestDto
{
    public string? Category { get; set; }
    
    public string? YearMonth { get; set; }
    
}