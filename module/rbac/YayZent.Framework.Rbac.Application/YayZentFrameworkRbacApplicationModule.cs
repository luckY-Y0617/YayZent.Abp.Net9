using Lazy.Captcha.Core.Generator;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using YayZent.Framework.AspNetCore;
using YayZent.Framework.Core.Email;
using YayZent.Framework.Core.Sms;
using YayZent.Framework.Ddd.Application;
using YayZent.Framework.Rbac.Application.Contracts;
using YayZent.Framework.Rbac.Domain;

namespace YayZent.Framework.Rbac.Application;

[DependsOn(typeof(YayZentFrameworkAspNetCoreModule),
    typeof(YayZentFrameworkCoreSmsModule),
    typeof(YayZentFrameworkDddApplicationModule),
    typeof(YayZentFrameworkCoreEmailModule),
    
    typeof(YayZentFrameworkRbacApplicationContractsModule),
    typeof(YayZentFrameworkRbacDomainModule)
    )]
public class YayZentFrameworkRbacApplicationModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<YayZentFrameworkRbacApplicationModule>();
            
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<YayZentFrameworkRbacApplicationModule>(); // 会自动扫描当前模块中的 Profile 类
        });
        
        context.Services.AddCaptcha(options =>
        {
            options.CaptchaType = CaptchaType.ARITHMETIC;
        });
    }
}