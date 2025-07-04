using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using YayZent.Framework.Rbac.Domain.Shared.Options;

namespace YayZent.Framework.Rbac.Domain.Shared;

[DependsOn(typeof(AbpDddDomainSharedModule))]
public class YayZentFrameworkRbacDomainSharedModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var rbac = configuration.GetSection("RbacOptions");
        var options = configuration.GetSection("RbacOptions").Get<RbacOptions>();
        context.Services.Configure<RbacOptions>(configuration.GetSection("RbacOptions"));
    }
}