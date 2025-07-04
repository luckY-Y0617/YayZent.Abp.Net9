using Volo.Abp.Modularity;
using YayZent.Framework.SqlSugarCore;

namespace YayZent.Framework.Bbs.SqlSugarCore;

[DependsOn(typeof(YayZentFrameworkSqlSugarCoreModule))]
public class YayZentFrameworkBbsSqlSugarCoreModule: AbpModule
{
    
}