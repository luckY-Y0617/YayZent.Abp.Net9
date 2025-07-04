using Volo.Abp.Application;
using Volo.Abp.Modularity;
using YayZent.Framework.Ddd.Application;
using YayZent.Framework.TenantManagement.Application.Contracts;
using YayZent.Framework.TenantManagement.Domain;

namespace YayZent.Framework.TenantManagement.Application;

[DependsOn(typeof(YayZentFrameworkDddApplicationModule),
    typeof(YayZentFrameworkTenantManagementDomainModule),
    typeof(YayZentFrameworkTenantManagementApplicationContractsModule))]
public class YayZentFrameworkTenantManagementApplicationModule: AbpModule
{
}