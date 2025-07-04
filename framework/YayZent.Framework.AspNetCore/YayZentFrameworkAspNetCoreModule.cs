using Volo.Abp.Modularity;
using YayZent.Framework.Core;

namespace YayZent.Framework.AspNetCore;

[DependsOn(typeof(YayZentFrameworkCoreModule))]
public class YayZentFrameworkAspNetCoreModule: AbpModule
{
    
}