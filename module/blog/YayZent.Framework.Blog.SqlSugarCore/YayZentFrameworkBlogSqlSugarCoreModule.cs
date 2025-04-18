using Volo.Abp.Modularity;
using YayZent.Framework.Blog.Domain;
using YayZent.Framework.SqlSugarCore;

namespace YayZent.Framework.Blog.SqlSugarCore;

[DependsOn(typeof(YayZentFrameworkSqlSugarCoreModule),
    typeof(YayZentFrameworkBlogDomainModule))]
public class YayZentFrameworkBlogSqlSugarCoreModule: AbpModule 
{
    
}