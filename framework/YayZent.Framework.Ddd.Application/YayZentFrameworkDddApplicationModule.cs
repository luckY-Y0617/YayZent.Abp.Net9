using Volo.Abp.Application;
using Volo.Abp.Modularity;
using YayZent.Framework.Ddd.Application.Contracts;

namespace YayZent.Framework.Ddd.Application;

[DependsOn(
    typeof(AbpDddApplicationModule),
    typeof(YayZentFrameworkDddApplicationContractsModule))]
public class YayZentFrameworkDddApplicationModule: AbpModule
{
    
}