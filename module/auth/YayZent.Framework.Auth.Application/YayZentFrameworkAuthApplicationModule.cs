using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using YayZent.Framework.Auth.Application.Contracts;
using YayZent.Framework.Auth.Domain;
using YayZent.Framework.Ddd.Application;

namespace YayZent.Framework.Auth.Application;

[DependsOn(typeof(YayZentFrameworkDddApplicationModule),
    typeof(YayZentFrameworkAuthDomainModule),
    typeof(YayZentFrameworkAuthApplicationContractsModule))]
public class YayZentFrameworkAuthApplicationModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<YayZentFrameworkAuthApplicationModule>();
            
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<YayZentFrameworkAuthApplicationModule>(); // 会自动扫描当前模块中的 Profile 类
        });
    }
}