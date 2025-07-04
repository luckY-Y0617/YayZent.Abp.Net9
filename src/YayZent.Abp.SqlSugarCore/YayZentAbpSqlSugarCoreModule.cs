using Volo.Abp.Modularity;
using YayZent.Abp.Domain;
using YayZent.Framework.AuditLogging.SqlSugarCore;
using YayZent.Framework.Auth.SqlSugarCore;
using YayZent.Framework.Bbs.SqlSugarCore;
using YayZent.Framework.Blog.SqlSugarCore;
using YayZent.Framework.Rbac.SqlSugarCore;
using YayZent.Framework.SqlSugarCore.Abstractions;
using YayZent.Framework.TenantManagement.SqlSugarCore;

namespace YayZent.Abp.SqlSugarCore;

[DependsOn(typeof(YayZentAbpDomainModule),
    
    typeof(YayZentFrameworkSqlSugarCoreAbstrationsModule),
    typeof(YayZentFrameworkBlogSqlSugarCoreModule),
    typeof(YayZentFrameworkTenantManagementSqlSugarCoreModule),
    typeof(YayZentFrameworkRbacSqlSugarCoreModule),
    typeof(YayZentFrameworkAuthSqlSugarCoreModule),
    typeof(YayZentFrameworkAuditLoggingSqlSugarCoreModule),
    typeof(YayZentFrameworkBbsSqlSugarCoreModule)
    )]
public class YayZentAbpSqlSugarCoreModule: AbpModule
{
    
}