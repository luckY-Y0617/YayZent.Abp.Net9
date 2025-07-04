using SqlSugar;
using Volo.Abp.Domain.Entities;

namespace YayZent.Framework.Rbac.Domain.Entities;

[SugarTable("RoleMenu")]
public class RoleMenuEntity: Entity<Guid>
{
    public Guid RoleId { get; set; }
    
    public Guid MenuId { get; set; }
    
    public RoleMenuEntity() {}

    public RoleMenuEntity(Guid roleId, Guid menuId)
    {
        RoleId = roleId;
        MenuId = menuId;
    }
}