using SqlSugar;
using Volo.Abp.Domain.Entities;

namespace YayZent.Framework.Rbac.Domain.Entities;

[SugarTable("UserRole")]
public class UserRoleEntity: Entity<Guid>
{
    public Guid UserId { get; set; }
    
    public Guid RoleId { get; set; }
}