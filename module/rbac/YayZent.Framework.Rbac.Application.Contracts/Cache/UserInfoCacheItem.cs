using YayZent.Framework.Rbac.Application.Contracts.Dtos;

namespace YayZent.Framework.Rbac.Application.Contracts.Cache;

public class UserInfoCacheItem(UserRoleMenuDto info)
{
    public UserRoleMenuDto Info { get; set; } = info;
}