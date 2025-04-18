using YayZent.Framework.Core.File.Enums;

namespace YayZent.Framework.Core.File.Abstractions;

public interface IStorageClientResolver
{
    IFileClient Resolve(StorageType storageType, string provider);
}