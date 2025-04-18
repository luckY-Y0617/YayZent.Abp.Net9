using Volo.Abp.Modularity;
using YayZent.Abp.Domain;
using YayZent.Framework.Blog.SqlSugarCore;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Abp.SqlSugarCore;

[DependsOn(typeof(YayZentAbpDomainModule),
    typeof(YayZentFrameworkSqlSugarCoreAbstrationsModule),
    typeof(YayZentFrameworkBlogSqlSugarCoreModule))]
public class YayZentAbpSqlSugarCoreModule: AbpModule
{
    
}