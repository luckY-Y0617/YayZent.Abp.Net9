namespace YayZent.Framework.Blog.Application.Contracts.Dtos;

public class CreateBlogPostOutputDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
}