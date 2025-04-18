using SqlSugar;
using Volo.Abp.Domain.Entities;

namespace YayZent.Framework.Blog.Domain.Entities;

[SugarTable("BlogFile")]
public class BlogFileEntity: Entity<Guid>
{
    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileSizeInBytes { get; set; }
    
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
    
    public BlogFileEntity(Guid id, long fileSizeInBytes, string fileContent, Uri? fileUploadUrl, 
        Uri? fileBackUpUrl, Uri? imageUploadUrl, Uri? imageBackUpUrl) : base(id)
    {
        FileSizeInBytes = fileSizeInBytes;
        FileContent = fileContent;
        FileUploadUrl = fileUploadUrl?.ToString();
        FileBackUpUrl = fileBackUpUrl?.ToString();
        ImageUploadUrl = imageUploadUrl?.ToString();
        ImageBackUpUrl = imageBackUpUrl?.ToString();
    }
}