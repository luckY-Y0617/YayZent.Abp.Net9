using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using YayZent.Framework.Core.File.Options;

namespace YayZent.Framework.Core.File;

public class YayZentFrameworkCoreStorageModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        context.Services.Configure<HuaWeiYunStorageOptions>(configuration.GetSection("FileService:HuaWeiYun"));
        context.Services.Configure<LocalStorageOptions>(configuration.GetSection("FileService:SMS"));
    }
}