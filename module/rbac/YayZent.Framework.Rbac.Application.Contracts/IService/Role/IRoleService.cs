using Volo.Abp.Application.Services;
using YayZent.Framework.Ddd.Application.Contracts;
using YayZent.Framework.Rbac.Application.Contracts.Dtos.Role;

namespace YayZent.Framework.Rbac.Application.Contracts.IService.Role;

public interface IRoleService:  ICustomCrudAppService<RoleGetOutputDto, RoleGetListOutputDto, Guid, RoleGetListInputDto, RoleCreateDto, RoleUpdateDto>
{
    
}