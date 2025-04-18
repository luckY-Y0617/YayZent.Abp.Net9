namespace YayZent.Framework.Blog.Domain.Shared.Dtos.Category;

public class CreateCategoryParameterDto
{
    public required string CategoryName { get; set; }
    
    public int SequenceNumber { get; set; }
}