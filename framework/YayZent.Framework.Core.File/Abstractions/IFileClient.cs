using YayZent.Framework.Core.File.Enums;

namespace YayZent.Framework.Core.File.Abstractions;

public interface IStorageClient
{
    string Provider { get; }
    StorageType StorageType { get; }
    
    Task<Uri> SaveFileAsync(string key, Stream content, CancellationToken cancellationToken = default);
}