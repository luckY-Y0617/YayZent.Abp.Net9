using Volo.Abp.Auditing;
using Volo.Abp.DependencyInjection;
using YayZent.Framework.AuditLogging.Domain.Entities;

namespace YayZent.Framework.AuditLogging.Domain;

public interface IAuditLogInfoToAuditLogConverter: ITransientDependency
{
    Task<AuditLogAggregateRoot> ConvertAsync(AuditLogInfo auditLogInfo);
}