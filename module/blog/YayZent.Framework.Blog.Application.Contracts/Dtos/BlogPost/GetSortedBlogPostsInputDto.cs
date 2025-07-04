namespace YayZent.Framework.Blog.Application.Contracts.Dtos.BlogPost;

public class GetSortedBlogPostsInputDto
{
    public bool IsSorted { get; set; }
    
    public int Quantity { get; set; }
}