using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using YayZent.Abp.Domain.Shared;
using YayZent.Framework.AuditLogging.Domain;
using YayZent.Framework.Auth.Domain;
using YayZent.Framework.Auth.Domain.Shared;
using YayZent.Framework.Bbs.Domain;
using YayZent.Framework.Mapster;
using YayZent.Framework.Rbac.Domain;
using YayZent.Framework.TenantManagement.Domain;

namespace YayZent.Abp.Domain;

[DependsOn(typeof(AbpDddDomainModule),
    
    typeof(YayZentAbpDomainSharedModule),
    typeof(YayZentFrameworkTenantManagementDomainModule),
    typeof(YayZentFrameworkMapsterModule),
    typeof(YayZentFrameworkRbacDomainModule),
    typeof(YayZentFrameworkAuthDomainModule),
    typeof(YayZentFrameworkAuditLoggingDomainModule),
    typeof(YayZentFrameworkBbsDomainModule)
    )]
public class YayZentAbpDomainModule: AbpModule
{
    
}