using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;
using YayZent.Framework.AuditLogging.Domain.Shared;
using YayZent.Framework.Auth.Domain.Shared;
using YayZent.Framework.Bbs.Domain.Shared;
using YayZent.Framework.Blog.Domain.Shared;
using YayZent.Framework.Rbac.Domain.Shared;

namespace YayZent.Abp.Domain.Shared;

[DependsOn(typeof(AbpDddDomainSharedModule),
    typeof(AbpTenantManagementDomainSharedModule),
    
    typeof(YayZentFrameworkBlogDomainSharedModule),
    typeof(YayZentFrameworkRbacDomainSharedModule),
    typeof(YayZentFrameworkAuthDomainSharedModule),
    typeof(YayZentFrameworkAuditLoggingDomainSharedModule),
    typeof(YayZentFrameworkBbsDomainSharedModule)
    )]
public class YayZentAbpDomainSharedModule: AbpModule
{
    
}