using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Data;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.TenantManagement.Domain;

[DependsOn(typeof(AbpTenantManagementDomainSharedModule),
    typeof(AbpDddDomainModule),
    typeof(YayZentFrameworkSqlSugarCoreAbstrationsModule))]
public class YayZentFrameworkTenantManagementDomainModule:AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        services.Replace(new ServiceDescriptor(typeof(ITenantStore), typeof(CustomTenantStore), ServiceLifetime.Transient));
        services.Replace(new ServiceDescriptor(typeof(IConnectionStringResolver), typeof(CustomMultiTenantConnectionStringResolver), ServiceLifetime.Transient));
    }
}