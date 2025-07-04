using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using YayZent.Framework.Blog.Application.Contracts;
using YayZent.Framework.Blog.Domain;
using YayZent.Framework.Core.File;
using YayZent.Framework.Core.Rendering;
using YayZent.Framework.Ddd.Application;

namespace YayZent.Framework.Blog.Application;

[DependsOn(
    typeof(AbpAutoMapperModule),
    
    typeof(YayZentFrameworkBlogApplicationContractsModule),
    typeof(YayZentFrameworkBlogDomainModule),
    typeof(YayZentFrameworkDddApplicationModule),
    typeof(YayZentFrameworkCoreRenderingModule),
    typeof(YayZentFrameworkCoreFileModule))]
public class YayZentFrameworkBlogApplicationModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //注册 AutoMapper 对象映射器 到依赖注入容器中。它允许你在该模块中通过 _objectMapper 使用 AutoMapper，并启用模块隔离（多模块系统中防止映射配置污染其他模块）。
            context.Services.AddAutoMapperObjectMapper<YayZentFrameworkBlogApplicationModule>();
            
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<YayZentFrameworkBlogApplicationModule>(); // 会自动扫描当前模块中的 Profile 类
            });
        }
}