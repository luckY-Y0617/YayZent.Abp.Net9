using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using YayZent.Abp.Domain.Shared;
using YayZent.Framework.Auth.Infrastructure;
using YayZent.Framework.Bbs.Infrastructure;
using YayZent.Framework.Rbac.Domain.Shared.Options;

namespace YayZent.Abp.Infrastructure;

[DependsOn(typeof(YayZentAbpDomainSharedModule),
    typeof(YayZentAbpDomainSharedModule),
    
    typeof(YayZentFrameworkAuthInfrastructureModule),
    typeof(YayZentFrameworkBbsInfrastructureModule)
    )]
public class YayZentAbpInfrastructureModule: AbpModule
{

}