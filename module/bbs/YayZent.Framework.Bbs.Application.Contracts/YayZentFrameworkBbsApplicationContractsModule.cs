using Volo.Abp.Modularity;
using YayZent.Framework.Bbs.Domain.Shared;
using YayZent.Framework.Ddd.Application.Contracts;

namespace YayZent.Framework.Bbs.Application.Contracts;

[DependsOn(typeof(YayZentFrameworkDddApplicationContractsModule),
    typeof(YayZentFrameworkBbsDomainSharedModule))]
public class YayZentFrameworkBbsApplicationContractsModule: AbpModule
{
    
}