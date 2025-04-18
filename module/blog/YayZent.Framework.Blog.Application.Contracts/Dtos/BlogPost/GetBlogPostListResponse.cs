using Volo.Abp.Application.Dtos;

namespace YayZent.Framework.Blog.Application.Contracts.Dtos;

public class GetBlogPostListResponse
{

    public List<BlogPostDetail>? BlogPostList { get; set; }
    
    public class BlogPostDetail
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public string? Summary { get; set; }
        public string? Category { get; set; }
        public DateTime? CreationTime { get; set; }
        
    }
}