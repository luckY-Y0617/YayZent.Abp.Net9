namespace YayZent.Framework.Blog.Application.Contracts.Dtos.Comment;

public class GetCommentsOutputDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string? ReplyUserName { get; set; }
    public string? CreationTime { get; set; }
}