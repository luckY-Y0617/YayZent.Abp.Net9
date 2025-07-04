using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using YayZent.Framework.Auth.Domain.Shared;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Auth.Domain;

[DependsOn(typeof(AbpDddDomainModule),
    typeof(YayZentFrameworkAuthDomainSharedModule),
    typeof(YayZentFrameworkSqlSugarCoreAbstrationsModule))]
public class YayZentFrameworkAuthDomainModule: AbpModule
{
    
}