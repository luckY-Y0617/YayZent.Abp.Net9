using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using YayZent.Framework.Rbac.Domain.Shared;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Rbac.Domain;

[DependsOn(typeof(AbpDddDomainModule),
    typeof(YayZentFrameworkRbacDomainSharedModule),
    typeof(YayZentFrameworkSqlSugarCoreAbstrationsModule))]
public class YayZentFrameworkRbacDomainModule: AbpModule
{
    
}