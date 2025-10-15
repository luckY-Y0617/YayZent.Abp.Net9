using SqlSugar;
using Volo.Abp.Domain.Entities;

namespace YayZent.Framework.Blog.Domain.Entities;

[SugarTable("BlogFile")]
public class BlogFileEntity: Entity<Guid>
{
    /// <summary>
    /// blog内容
    /// </summary>
    [SugarColumn(ColumnDataType = "longtext")]
    public string FileContent { get; set; }

    /// <summary>
    /// 图片远程Url
    /// </summary>
    public string? ImageUploadUrl { get; set; }
    
    /// <summary>
    /// 图片本地Url
    /// </summary>
    public string? ImageBackUpUrl { get; set; }
    
    /// <summary>
    /// 文件远程Url
    /// </summary>
    public string? FileUploadUrl { get; set; } 

    /// <summary>
    /// 文件本地Url
    /// </summary>
    public string? FileBackUpUrl { get; set; }

    public BlogFileEntity() {}

    public BlogFileEntity(Guid id, string fileContent) : base(id)
    {
        FileContent = fileContent;
    }

    public void SetUrl(Uri? fileUploadUrl, Uri? fileBackUpUrl, Uri? imageUploadUrl, Uri? imageBackUpUrl)
    {
        FileUploadUrl = fileUploadUrl?.ToString();
        FileBackUpUrl = fileBackUpUrl?.LocalPath;
        ImageUploadUrl = imageUploadUrl?.ToString();
        ImageBackUpUrl = imageBackUpUrl?.LocalPath;
    }

}