namespace YayZent.Framework.Blog.Application.Contracts.Dtos.Tag;

public class TagCreateDto
{
    public string TagName { get; set; }
    
    public Guid CategoryId { get; set; }
}