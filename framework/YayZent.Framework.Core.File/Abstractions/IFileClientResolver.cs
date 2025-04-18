using Volo.Abp.DependencyInjection;
using YayZent.Framework.Core.File.Enums;

namespace YayZent.Framework.Core.File.Abstractions;

public interface IFileClientResolver: ITransientDependency
{
    IFileClient Resolve(StorageType storageType, string provider);
}