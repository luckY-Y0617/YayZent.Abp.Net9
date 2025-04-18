using Volo.Abp.Modularity;
using YayZent.Framework.Blog.Domain.Shared;
using YayZent.Framework.Ddd.Application.Contracts;

namespace YayZent.Framework.Blog.Application.Contracts;

[DependsOn(typeof(YayZentFrameworkDddApplicationContractsModule),
    typeof(YayZentFrameworkBlogDomainSharedModule))]
public class YayZentFrameworkBlogApplicationContractsModule: AbpModule
{
    
}