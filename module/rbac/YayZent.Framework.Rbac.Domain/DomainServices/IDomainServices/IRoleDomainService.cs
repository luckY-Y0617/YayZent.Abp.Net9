using Volo.Abp.Domain.Services;
using YayZent.Framework.Rbac.Domain.Entities;

namespace YayZent.Framework.Rbac.Domain.DomainServices.IDomainServices;

public interface IRoleDomainService: IDomainService
{
    Task<List<RoleMenuEntity>> SetRoleMenusAsync(List<Guid> roleIds, List<Guid> menuIds);
}