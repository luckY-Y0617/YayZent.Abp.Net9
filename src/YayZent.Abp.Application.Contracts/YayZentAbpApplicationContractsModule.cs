using Volo.Abp.Modularity;
using YayZent.Abp.Domain.Shared;
using YayZent.Framework.Auth.Application.Contracts;
using YayZent.Framework.Bbs.Application.Contracts;
using YayZent.Framework.Blog.Application.Contracts;
using YayZent.Framework.Blog.Domain.Shared;
using YayZent.Framework.Ddd.Application.Contracts;
using YayZent.Framework.Rbac.Application.Contracts;
using YayZent.Framework.TenantManagement.Application.Contracts;

namespace YayZent.Abp.Application.Contracts;

[DependsOn(typeof(YayZentAbpDomainSharedModule),
    
    typeof(YayZentFrameworkBlogDomainSharedModule),
    typeof(YayZentFrameworkDddApplicationContractsModule),
    typeof(YayZentFrameworkBlogApplicationContractsModule),
    typeof(YayZentFrameworkTenantManagementApplicationContractsModule),
    typeof(YayZentFrameworkRbacApplicationContractsModule),
    typeof(YayZentFrameworkAuthApplicationContractsModule),
    typeof(YayZentFrameworkBbsApplicationContractsModule))
]
public class YayZentAbpApplicationContractsModule: AbpModule
{
    
}