namespace YayZent.Framework.Blog.Application.Contracts.Dtos.Category;

public class MenuOutputDto
{
    public string? CategoryName { get; set; }
    
    public List<MenuItem> Children { get; set; }

}

public class MenuItem
{
    public string Label { get; set; } = string.Empty;
        
    public Guid Value { get; set; }
}