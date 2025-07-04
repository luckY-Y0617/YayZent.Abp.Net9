using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using YayZent.Framework.Core.Email.Options;

namespace YayZent.Framework.Core.Email;

public class YayZentFrameworkCoreEmailModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClient();
        context.Services.Configure<SendCloudOptions>(context.Services.GetConfiguration().GetSection("SendCloudOptions"));
    }
}