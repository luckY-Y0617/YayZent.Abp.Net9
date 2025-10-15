namespace YayZent.Framework.Blog.Application.Contracts.Dtos.Tag;

public class TagCreateDto
{
    public required string TagName { get; set; }
    
    public int SequenceNumber { get; set; }
}