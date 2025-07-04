using Volo.Abp.Data;
using YayZent.Framework.Auth.Domain.Shared.Authorization;

namespace YayZent.Framework.Auth.Domain.Shared.Authorization;

public static class DataPermissionExtensions
{
    public static IDisposable DisablePermissionHandler(this IDataFilter dataFilter)
    {
        return dataFilter.Disable<IDataPermission>();
    }
}