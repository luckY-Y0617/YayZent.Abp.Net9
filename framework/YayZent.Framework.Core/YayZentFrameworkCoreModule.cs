using Newtonsoft.Json;
using Volo.Abp.Modularity;
using YayZent.Framework.Core.Serialization;

namespace YayZent.Framework.Core;

[DependsOn()]
public class YayZentFrameworkCoreModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<JsonSerializerSettings>(settings =>
        {
            settings.NullValueHandling = NullValueHandling.Ignore; // 忽略 null 值
            settings.ContractResolver = new NonPublicPropertiesResolver(); // 使用自定义的 ContractResolver
        });
    }
}