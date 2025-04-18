using Volo.Abp.Modularity;
using YayZent.Abp.Domain.Shared;
using YayZent.Framework.Blog.Application.Contracts;
using YayZent.Framework.Blog.Domain.Shared;
using YayZent.Framework.Ddd.Application.Contracts;

namespace YayZent.Abp.Application.Contracts;

[DependsOn(typeof(YayZentAbpDomainSharedModule),
    typeof(YayZentFrameworkBlogDomainSharedModule),
    typeof(YayZentFrameworkDddApplicationContractsModule),
    
    typeof(YayZentFrameworkBlogApplicationContractsModule))
]
public class YayZentAbpApplicationContractsModule: AbpModule
{
    
}