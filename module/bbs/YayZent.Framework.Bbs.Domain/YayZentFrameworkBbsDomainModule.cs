using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using YayZent.Framework.Bbs.Domain.Shared;

namespace YayZent.Framework.Bbs.Domain;

[DependsOn(typeof(AbpDddDomainModule),
    typeof(YayZentFrameworkBbsDomainSharedModule))]
public class YayZentFrameworkBbsDomainModule: AbpModule
{
    
}