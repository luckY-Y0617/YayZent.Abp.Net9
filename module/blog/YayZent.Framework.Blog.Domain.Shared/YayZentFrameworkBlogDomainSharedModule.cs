using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace YayZent.Framework.Blog.Domain.Shared;

[DependsOn(typeof(AbpDddDomainSharedModule))]
public class YayZentFrameworkBlogDomainSharedModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        
    }
}