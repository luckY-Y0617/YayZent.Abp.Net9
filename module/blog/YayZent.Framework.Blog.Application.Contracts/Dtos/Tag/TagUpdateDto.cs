namespace YayZent.Framework.Blog.Application.Contracts.Dtos.Tag;

public class TagUpdateDto
{
    public required string TagName { get; set; }
    
    public int SequenceNumber { get; set; }
}