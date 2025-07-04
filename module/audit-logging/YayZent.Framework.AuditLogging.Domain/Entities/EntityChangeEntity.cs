using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using YayZent.Framework.Core.Attributes;

namespace YayZent.Framework.AuditLogging.Domain.Entities;

[SystemTable]
[SugarTable("EntityChange")]
[SugarIndex($"index_{nameof(AuditLogId)}", nameof(AuditLogId), OrderByType.Asc)]
[SugarIndex($"index_{nameof(TenantId)}_{nameof(EntityId)}", nameof(TenantId), OrderByType.Asc, nameof(EntityTypeFullName), OrderByType.Asc, nameof(EntityId), OrderByType.Asc)]
public class EntityChangeEntity: Entity<Guid>, IMultiTenant
{
    public override Guid Id { get; protected set; }
    public Guid AuditLogId { get; set; }
    
    public Guid? TenantId { get; }
    
    public DateTime? ChangeTime { get; set; }
    
    public EntityChangeType? ChangeType { get; set; }
    
    public Guid? EntityTenantId { get; set; }
    
    public string? EntityId { get; set; }
    
    public string? EntityTypeFullName { get; set; }
    
    public List<EntityPropertyChangeEntity> PropertyChanges { get; set; } = new();
    
    public EntityChangeEntity() {}

    public EntityChangeEntity(IGuidGenerator guidGenerator, Guid auditLogId, EntityChangeInfo entityChangeInfo, Guid? tenantId = null):base(guidGenerator.Create())
    {
        AuditLogId = auditLogId;
        ChangeTime = entityChangeInfo.ChangeTime;
        ChangeType = entityChangeInfo.ChangeType;
        EntityId = entityChangeInfo.EntityId;
        EntityTenantId = tenantId;
        TenantId = tenantId;
        EntityTypeFullName = entityChangeInfo.EntityTypeFullName;
        PropertyChanges = entityChangeInfo.PropertyChanges
            .Select(x => new EntityPropertyChangeEntity(guidGenerator,Id, x, tenantId))
            .ToList();
    }
}