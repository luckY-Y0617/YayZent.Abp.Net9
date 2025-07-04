using Volo.Abp.Modularity;
using YayZent.Abp.Application.Contracts;
using YayZent.Abp.Domain;
using YayZent.Framework.Auth.Application;
using YayZent.Framework.Bbs.Application;
using YayZent.Framework.Blog.Application;
using YayZent.Framework.Ddd.Application;
using YayZent.Framework.Rbac.Application;
using YayZent.Framework.TenantManagement.Application;

namespace YayZent.Abp.Application;

[DependsOn(typeof(YayZentAbpApplicationContractsModule),
    typeof(YayZentAbpDomainModule),
    
    typeof(YayZentFrameworkDddApplicationModule),
    typeof(YayZentFrameworkBlogApplicationModule),
    typeof(YayZentFrameworkTenantManagementApplicationModule),
    typeof(YayZentFrameworkRbacApplicationModule),
    typeof(YayZentFrameworkAuthApplicationModule),
    typeof(YayZentFrameworkBbsApplicationModule)
    )]
public class YayZentAbpApplicationModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Console.WriteLine("debug v1");
    }
}