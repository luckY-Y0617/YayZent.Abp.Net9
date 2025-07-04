using Volo.Abp.Modularity;
using YayZent.Framework.AuditLogging.Domain;
using YayZent.Framework.AuditLogging.Domain.Shared;
using YayZent.Framework.SqlSugarCore;

namespace YayZent.Framework.AuditLogging.SqlSugarCore;

[DependsOn(typeof(YayZentFrameworkSqlSugarCoreModule),
    typeof(YayZentFrameworkAuditLoggingDomainModule),
    typeof(YayZentFrameworkAuditLoggingDomainSharedModule)
    )]
public class YayZentFrameworkAuditLoggingSqlSugarCoreModule: AbpModule
{
    
}