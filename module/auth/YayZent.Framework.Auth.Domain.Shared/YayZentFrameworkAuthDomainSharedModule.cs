using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace YayZent.Framework.Auth.Domain.Shared;

[DependsOn(typeof(AbpDddDomainSharedModule))]
public class YayZentFrameworkAuthDomainSharedModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {

    }
}