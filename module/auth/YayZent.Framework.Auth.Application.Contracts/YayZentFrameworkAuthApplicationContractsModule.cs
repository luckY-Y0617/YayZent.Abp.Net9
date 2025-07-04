using Volo.Abp.Modularity;
using YayZent.Framework.Auth.Domain.Shared;
using YayZent.Framework.Ddd.Application.Contracts;

namespace YayZent.Framework.Auth.Application.Contracts;

[DependsOn(typeof(YayZentFrameworkDddApplicationContractsModule),
    typeof(YayZentFrameworkAuthDomainSharedModule))]
public class YayZentFrameworkAuthApplicationContractsModule: AbpModule
{
    
}