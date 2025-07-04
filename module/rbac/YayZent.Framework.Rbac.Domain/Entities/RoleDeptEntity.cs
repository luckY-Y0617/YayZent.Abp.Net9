using SqlSugar;
using Volo.Abp.Domain.Entities;

namespace YayZent.Framework.Rbac.Domain.Entities;

[SugarTable("RoleDept")]
public class RoleDeptEntity: Entity<Guid>
{
    public Guid RoleId { get; set; }
    
    public Guid DeptId { get; set; }
    
    public RoleDeptEntity() {}

    public RoleDeptEntity(Guid roleId, Guid deptId)
    {
        RoleId = roleId;
        DeptId = deptId;
    }
}