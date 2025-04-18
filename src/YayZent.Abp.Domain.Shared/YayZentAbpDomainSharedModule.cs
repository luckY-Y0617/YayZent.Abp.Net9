using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using YayZent.Framework.Blog.Domain.Shared;

namespace YayZent.Abp.Domain.Shared;

[DependsOn(typeof(AbpDddDomainSharedModule),
    typeof(YayZentFrameworkBlogDomainSharedModule))]
public class YayZentAbpDomainSharedModule: AbpModule
{
    
}