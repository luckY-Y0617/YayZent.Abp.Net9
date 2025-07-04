namespace YayZent.Framework.Blog.Application.Contracts.Dtos.Category;

public class CatergoryCreateDto
{
    public string CategoryName { get; set; }
    
    public List<string> TagNames { get; set; }
}