namespace YayZent.Framework.Blog.Application.Contracts.Dtos.BlogPost;

public class GetSortedBlogPostsOutputDto
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
}