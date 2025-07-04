using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using YayZent.Framework.AuditLogging.Domain.Shared;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.AuditLogging.Domain;

[DependsOn(typeof(AbpDddDomainModule),
    typeof(YayZentFrameworkAuditLoggingDomainSharedModule),
    typeof(YayZentFrameworkSqlSugarCoreAbstrationsModule)
)]
public class YayZentFrameworkAuditLoggingDomainModule: AbpModule
{
    
}