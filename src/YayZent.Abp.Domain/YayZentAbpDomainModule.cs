using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using YayZent.Abp.Domain.Shared;

namespace YayZent.Abp.Domain;

[DependsOn(typeof(AbpDddDomainModule),
    typeof(YayZentAbpDomainSharedModule))]
public class YayZentAbpDomainModule: AbpModule
{
    
}