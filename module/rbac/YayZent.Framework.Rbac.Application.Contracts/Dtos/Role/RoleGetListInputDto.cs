using YayZent.Framework.Ddd.Application.Contracts.Dtos;

namespace YayZent.Framework.Rbac.Application.Contracts.Dtos.Role;

public class RoleGetListInputDto: PagedAllResultRequestDto
{
    public string? RoleName { get; set; }
    
    public string? RoleCode { get; set; }
    
    public bool? State { get; set; }
}