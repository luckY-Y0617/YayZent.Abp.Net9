using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using YayZent.Framework.Core.Sms.Options;

namespace YayZent.Framework.Core.Sms;

public class YayZentFrameworkCoreSmsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        context.Services.Configure<AliyunOptions>(configuration.GetSection("AliyunOptions"));
    }
}