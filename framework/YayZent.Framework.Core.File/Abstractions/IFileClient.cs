using Volo.Abp.DependencyInjection;
using YayZent.Framework.Core.File.Enums;

namespace YayZent.Framework.Core.File.Abstractions;

public interface IFileClient: ITransientDependency
{
    string Provider { get; }
    StorageType StorageType { get; }
    
    Task<Uri> SaveFileAsync(string key, Stream content, CancellationToken cancellationToken = default);

    Task<Uri> UpdateFileAsync(string? fullpath, Stream content, CancellationToken cancellationToken = default);
    
    Task<Stream> ReadFileAsync(string url, CancellationToken cancellationToken = default);
    
    Task DeleteFileAsync(string? key, CancellationToken cancellationToken = default);
}