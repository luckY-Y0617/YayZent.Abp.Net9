namespace YayZent.Framework.Blog.Domain.Shared.Etos;

public class BlogUpdatedEventArgs
{
    public Guid BlogFileId { get; set; }

    public string ContentPath { get; set; }
    
    public string? ImagePath { get; set; }
}