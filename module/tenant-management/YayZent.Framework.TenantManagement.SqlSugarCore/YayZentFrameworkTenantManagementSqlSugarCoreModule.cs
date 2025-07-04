using Volo.Abp.Modularity;
using YayZent.Framework.SqlSugarCore.Abstractions;
using YayZent.Framework.TenantManagement.Domain;

namespace YayZent.Framework.TenantManagement.SqlSugarCore;

[DependsOn(typeof(YayZentFrameworkSqlSugarCoreAbstrationsModule),
    typeof(YayZentFrameworkTenantManagementDomainModule))]
public class YayZentFrameworkTenantManagementSqlSugarCoreModule:AbpModule
{
    
}