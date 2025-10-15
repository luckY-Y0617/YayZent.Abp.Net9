using Volo.Abp.Modularity;
using YayZent.Framework.AspNetCore.Filters;
using YayZent.Framework.Core;

namespace YayZent.Framework.AspNetCore;

[DependsOn(typeof(YayZentFrameworkCoreModule))]
public class YayZentFrameworkAspNetCoreModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }

    
}