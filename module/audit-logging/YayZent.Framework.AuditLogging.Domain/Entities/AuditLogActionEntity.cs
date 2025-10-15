using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using YayZent.Framework.Core.Attributes;

namespace YayZent.Framework.AuditLogging.Domain.Entities;

[DisableAuditing]
[SugarTable("AuditLogAction")]
[SugarIndex($"index_{nameof(AuditLogId)}", nameof(AuditLogId), OrderByType.Asc)]
[SugarIndex($"index_{nameof(TenantId)}_{nameof(ExecutionTime)}", nameof(TenantId), OrderByType.Asc, nameof(ServiceName), OrderByType.Asc, nameof(MethodName), OrderByType.Asc, nameof(ExecutionTime), OrderByType.Asc)]
public class AuditLogActionEntity: Entity<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }
    
    public virtual Guid? AuditLogId { get; set; }
    
    public virtual string? ServiceName { get; set; }
    
    public virtual string? MethodName { get; set; }
    
    public virtual string? Parameters { get; set; }
    
    public virtual DateTime? ExecutionTime { get; set; }
    
    public virtual int? ExecutionDuration { get; set; }
    
    
    public AuditLogActionEntity(){}

    public AuditLogActionEntity(Guid id, Guid auditLogId, AuditLogActionInfo actionInfo, Guid? tenantId):base(id)
    {
        TenantId = tenantId;
        AuditLogId = auditLogId;
        ServiceName = actionInfo.ServiceName;
        MethodName = actionInfo.MethodName;
        Parameters = actionInfo.Parameters;
        ExecutionTime = actionInfo.ExecutionTime;
        ExecutionDuration = actionInfo.ExecutionDuration;
        
    }
}

