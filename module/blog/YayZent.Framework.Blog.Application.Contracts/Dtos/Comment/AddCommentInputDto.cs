namespace YayZent.Framework.Blog.Application.Contracts.Dtos.Comment;

public class AddCommentInputDto
{
    public Guid BlogPostId { get; set; }
    
    
    public string Content { get; set; } = string.Empty;
}