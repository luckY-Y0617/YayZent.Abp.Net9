using Volo.Abp.Modularity;
using YayZent.Framework.SqlSugarCore;

namespace YayZent.Framework.Auth.SqlSugarCore;

[DependsOn(typeof(YayZentFrameworkSqlSugarCoreModule))]
public class YayZentFrameworkAuthSqlSugarCoreModule: AbpModule
{
    
}