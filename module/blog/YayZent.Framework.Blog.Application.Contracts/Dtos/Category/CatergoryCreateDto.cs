namespace YayZent.Framework.Blog.Application.Contracts.Dtos.Category;

public class CatergoryCreateDto
{
    public required string CategoryName { get; set; }
    
    public int SequenceNumber { get; set; }
}