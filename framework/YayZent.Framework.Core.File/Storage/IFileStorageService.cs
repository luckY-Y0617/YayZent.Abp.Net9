using Volo.Abp.DependencyInjection;

namespace YayZent.Framework.Core.File.Storage;

public interface IFileStorageService: ITransientDependency
{
    Task<string> SaveTempFileAsync(Stream stream, string? extension = null);
    
    Task<MemoryStream> GetTempFileAsync(string tempFileId);
    
    Task DeleteTempFileAsync(string tempFileId);
}