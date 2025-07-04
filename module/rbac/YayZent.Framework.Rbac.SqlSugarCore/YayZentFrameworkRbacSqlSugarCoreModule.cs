using Volo.Abp.Modularity;
using YayZent.Framework.Rbac.Domain;
using YayZent.Framework.SqlSugarCore;

namespace YayZent.Framework.Rbac.SqlSugarCore;

[DependsOn(typeof(YayZentFrameworkSqlSugarCoreModule),
    typeof(YayZentFrameworkRbacDomainModule))]
public class YayZentFrameworkRbacSqlSugarCoreModule: AbpModule
{
    
}