using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace YayZent.Framework.Blog.Application.Contracts.Dtos;

public class CreateBlogPostRequest
{
    public required string Title { get; set; }
    public string? Summary { get; set; }
    public List<string>? Tags { get; set; }
    public required string BlogContent { get; set; }
    public IFormFile? Image { get; set; }
}

public class CreateBlogPostRequestValidator : AbstractValidator<CreateBlogPostRequest>
{
    public CreateBlogPostRequestValidator()
    {
        long maxFileSize = 50 * 1024 * 1024;
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.BlogContent).NotEmpty();
        RuleFor(x => x.Image).Must(f => f.Length > 0 && f.Length < maxFileSize).When(x => x.Image is not null);
    }
}