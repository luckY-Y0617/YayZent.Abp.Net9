using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace YayZent.Framework.SqlSugarCore;

public class TenantConfigurationWrapper(IServiceProvider serviceProvider): ITransientDependency
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private  ICurrentTenant CurrentTenant => _serviceProvider.GetRequiredService<ICurrentTenant>();
    private  ITenantStore TenantStore => _serviceProvider.GetRequiredService<ITenantStore>();

    public async Task<TenantConfiguration?> GetAsync()
    {
        if (CurrentTenant.Id.HasValue)
        {
            var config = await TenantStore.FindAsync(CurrentTenant.Id.Value);
            if (config == null)
            {
                throw new ApplicationException("未找到租户，Id={tenantId}");
            }
            return config;
        }

        if (!CurrentTenant.Name.IsNullOrWhiteSpace())
        {
            var config = await TenantStore.FindAsync(CurrentTenant.Name);
            if (config == null)
            {
                throw new ApplicationException("未找到租户，Id={tenantId}");
            }
            return config;
        }
        
        return await TenantStore.FindAsync(ConnectionStrings.DefaultConnectionStringName);
    }
}