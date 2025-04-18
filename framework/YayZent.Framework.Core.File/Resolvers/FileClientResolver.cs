using YayZent.Framework.Core.File.Abstractions;
using YayZent.Framework.Core.File.Enums;

namespace YayZent.Framework.Core.File.Resolvers;

public class StorageClientResolver: IStorageClientResolver
{
    private readonly IEnumerable<IFileClient> _storageClients;

    public StorageClientResolver(IEnumerable<IFileClient> storageClients)
    {
        _storageClients = storageClients;
    }
    
    public IFileClient Resolve(StorageType storageType, string provider)
    {
        return _storageClients.First(client => client.StorageType == storageType && client.Provider == provider);
    }
}