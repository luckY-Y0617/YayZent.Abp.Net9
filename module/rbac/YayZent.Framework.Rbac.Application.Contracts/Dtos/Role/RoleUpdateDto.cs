using YayZent.Framework.Rbac.Domain.Shared.Enums;

namespace YayZent.Framework.Rbac.Application.Contracts.Dtos.Role;

public class RoleUpdateDto
{
    public string? RoleName { get; set; }
    
    public string? RoleCode { get; set; }
    
    public string? Remark { get; set; }

    public DataScopeEnum DataScope { get; set; } = DataScopeEnum.All;

    public bool State { get; set; } = true;
    
    public int OrderNum { get; set; }
    
    public List<Guid> DeptIds { get; set; }
    
    public List<Guid> RoleIds { get; set; }
}