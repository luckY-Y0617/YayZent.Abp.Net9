using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Yayzent.Framework.BackgroundWorkers.Hangfire;
using YayZent.Framework.Bbs.Application.Contracts;
using YayZent.Framework.Bbs.Domain;
using YayZent.Framework.Bbs.Domain.Shared;
using YayZent.Framework.Ddd.Application;

namespace YayZent.Framework.Bbs.Application;

[DependsOn(typeof(YayZentFrameworkBbsApplicationContractsModule),
    typeof(YayZentFrameworkBbsDomainModule),
    typeof(YayZentFrameworkBbsDomainSharedModule),
    typeof(YayZentFrameworkBackgroundWorkersHangfireModule),
    typeof(YayZentFrameworkDddApplicationModule))]
public class YayZentFrameworkBbsApplicationModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //注册 AutoMapper 对象映射器 到依赖注入容器中。它允许你在该模块中通过 _objectMapper 使用 AutoMapper，并启用模块隔离（多模块系统中防止映射配置污染其他模块）。
        context.Services.AddAutoMapperObjectMapper<YayZentFrameworkBbsApplicationModule>();
            
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<YayZentFrameworkBbsApplicationModule>(); // 会自动扫描当前模块中的 Profile 类
        });
    }
}