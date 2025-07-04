using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.TenantManagement.Application.Contracts;

[DependsOn(typeof(AbpDddApplicationContractsModule),
    typeof(AbpTenantManagementDomainSharedModule),
    typeof(YayZentFrameworkSqlSugarCoreAbstrationsModule))]
public class YayZentFrameworkTenantManagementApplicationContractsModule: AbpModule
{
    
}