using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using YayZent.Framework.Blog.Application.Contracts;
using YayZent.Framework.Blog.Domain;
using YayZent.Framework.Core.File;
using YayZent.Framework.Core.Rendering;
using YayZent.Framework.Ddd.Application;

namespace YayZent.Framework.Blog.Application;

[DependsOn(typeof(YayZentFrameworkBlogApplicationContractsModule),
    typeof(YayZentFrameworkBlogDomainModule),
    
    typeof(YayZentFrameworkDddApplicationModule),
    typeof(YayZentFrameworkCoreRenderingModule),
    typeof(YayZentFrameworkCoreFileModule))]
public class YayZentFrameworkBlogApplicationModule: AbpModule
{

}