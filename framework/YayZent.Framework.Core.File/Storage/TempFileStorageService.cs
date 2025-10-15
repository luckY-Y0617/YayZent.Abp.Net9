namespace YayZent.Framework.Core.File.Storage;

// 建议改为分布式存储
public class TempFileStorageService: IFileStorageService
{
    private readonly string _tempFolder;

    public TempFileStorageService()
    {
        _tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "TempFiles");
        Directory.CreateDirectory(_tempFolder); // 自动创建目录
    }

    public async Task<string> SaveTempFileAsync(Stream stream, string? extension = null)
    {
        var tempFileId = Guid.NewGuid().ToString("N");
        if (!string.IsNullOrEmpty(extension))
        {
            extension = extension.TrimStart('.');
            tempFileId += $".{extension}";
        }

        // 确保目录存在
        Directory.CreateDirectory(_tempFolder);

        var filePath = Path.Combine(_tempFolder, tempFileId);

        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fs);

        return tempFileId;
    }
    public async Task<MemoryStream> GetTempFileAsync(string tempFileId)
    {
        var filePath = Path.Combine(_tempFolder, tempFileId);
        await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var ms = new MemoryStream();
        await fs.CopyToAsync(ms);
        ms.Position = 0;
        return ms;
    }

    public Task DeleteTempFileAsync(string tempFileId)
    {
        var filePath = Path.Combine(_tempFolder, tempFileId);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}