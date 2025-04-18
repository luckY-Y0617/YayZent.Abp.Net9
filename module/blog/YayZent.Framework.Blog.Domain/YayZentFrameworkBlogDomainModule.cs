using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using YayZent.Framework.Blog.Domain.DomainServices;
using YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Blog.Domain.Shared;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Blog.Domain;

[DependsOn(typeof(YayZentFrameworkBlogDomainSharedModule),
    typeof(YayZentFrameworkSqlSugarCoreAbstrationsModule),
    
    typeof(AbpDddDomainModule)
    )]
public class YayZentFrameworkBlogDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
    }

}