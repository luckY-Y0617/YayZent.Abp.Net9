using Volo.Abp.Modularity;
using YayZent.Abp.Application.Contracts;
using YayZent.Abp.Domain;
using YayZent.Framework.Blog.Application;
using YayZent.Framework.Ddd.Application;

namespace YayZent.Abp.Application;

[DependsOn(typeof(YayZentAbpApplicationContractsModule),
    typeof(YayZentAbpDomainModule),
    
    typeof(YayZentFrameworkDddApplicationModule),
    
    typeof(YayZentFrameworkBlogApplicationModule))]
public class YayZentAbpApplicationModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Console.WriteLine("debug v1");
    }
}