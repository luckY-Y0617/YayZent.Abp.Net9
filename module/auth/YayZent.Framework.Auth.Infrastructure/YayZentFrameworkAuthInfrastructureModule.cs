using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using YayZent.Framework.Auth.Application.Contracts;
using YayZent.Framework.Auth.Domain.Shared;
using YayZent.Framework.Auth.Infrastructure.Authorization;

namespace YayZent.Framework.Auth.Infrastructure;

[DependsOn(typeof(YayZentFrameworkAuthDomainSharedModule),
    typeof(YayZentFrameworkAuthApplicationContractsModule))]
public class YayZentFrameworkAuthInfrastructureModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var service = context.Services;
        var configuration = context.Services.GetConfiguration();
        service.AddControllers(options =>
        {
            options.Filters.Add<PermissionGlobalAttribute>();
        });
    }
}