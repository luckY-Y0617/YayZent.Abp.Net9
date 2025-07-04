using YayZent.Framework.Rbac.Domain.Shared.Enums;

namespace YayZent.Framework.Rbac.Application.Contracts.Dtos.Role;

public class UpdateDataScopeInputDto
{
    public Guid RoleId { get; set; }

    public List<Guid> DeptIds { get; set; }

    public DataScopeEnum DataScope { get; set; }
}