namespace YayZent.Framework.Blog.Application.Contracts.Dtos.Comment;

public class ReplyCommentOutputDto
{
    public Guid Id { get; set; }
    
    public Guid? UserId { get; set; }
    
    public Guid ParentCommentId { get; set; }
    
    public string Content { get; set; }
    
    public string? UserName { get; set; }
    
    public string CreationTime { get; set; }
}