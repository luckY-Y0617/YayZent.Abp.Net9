using Microsoft.Extensions.Options;
using YayZent.Framework.Core.File.Abstractions;
using YayZent.Framework.Core.File.Enums;
using YayZent.Framework.Core.File.Options;

namespace YayZent.Framework.Core.File.Clients;

public class LocalStorageClient: IStorageClient
{
    private readonly IOptionsSnapshot<LocalStorageOptions> _options;
    public string Provider => "Local";
    public StorageType StorageType => StorageType.Local;

    public LocalStorageClient(IOptionsSnapshot<LocalStorageOptions> options)
    {
        _options = options;
    }
    public async Task<Uri> SaveFileAsync(string key, Stream content, CancellationToken cancellationToken = default)
    {
        if (key.StartsWith("/"))
        {
            throw new ArgumentException("Key should not start with /");
        }

        // 完整路径
        string fullPath = Path.Combine(_options.Value.WorkingDir, key);
        // 完整目录
        string? fullDir = Path.GetDirectoryName(fullPath);

        if(!Directory.Exists(fullDir))
        {
            Directory.CreateDirectory(fullDir);
        }

        if(System.IO.File.Exists(fullPath))
        {
            System.IO.File.Delete(fullPath);
        }

        try
        {
            using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                await content.CopyToAsync(fs, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("保存文件失败");
        }

        return new Uri(fullPath);
    }
}