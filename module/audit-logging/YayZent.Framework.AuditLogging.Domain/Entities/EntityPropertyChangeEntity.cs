using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using YayZent.Framework.Core.Attributes;

namespace YayZent.Framework.AuditLogging.Domain.Entities;

[SugarTable("EntityPropertyChange")]
[SugarIndex($"index_{nameof(EntityChangeId)}", nameof(EntityChangeId), OrderByType.Asc)]
public class EntityPropertyChangeEntity: Entity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    
    public Guid? EntityChangeId { get; set; }
    
    public string? NewValue { get; set; }
    
    public string? OriginalValue { get; set; }
    
    public string? PropertyName { get; set; }
    
    public string? PropertyTypeFullName { get; set; }
    
    public EntityPropertyChangeEntity() {}

    public EntityPropertyChangeEntity(IGuidGenerator guidGenerator, Guid entityChangeId, EntityPropertyChangeInfo entityPropertyChangeInfo,
        Guid? tenantId = null):base(guidGenerator.Create())
    {
        TenantId = tenantId;
        EntityChangeId = entityChangeId;
        NewValue = entityPropertyChangeInfo.NewValue;
        OriginalValue = entityPropertyChangeInfo.OriginalValue;
        PropertyName = entityPropertyChangeInfo.PropertyName;
        PropertyTypeFullName = entityPropertyChangeInfo.PropertyTypeFullName;
    }
}