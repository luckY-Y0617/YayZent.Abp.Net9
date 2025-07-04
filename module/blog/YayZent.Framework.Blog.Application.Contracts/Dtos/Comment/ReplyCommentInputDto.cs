namespace YayZent.Framework.Blog.Application.Contracts.Dtos.Comment;

public class ReplyCommentInputDto
{
    public Guid ParentCommentId { get; set; }
    
    public Guid BlogPostId { get; set; }
    
    public string Content { get; set; } = string.Empty;
}