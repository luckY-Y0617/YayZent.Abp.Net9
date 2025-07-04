using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectMapping;

namespace YayZent.Framework.Mapster;

[DependsOn(typeof(AbpObjectMappingModule))]
public class YayZentFrameworkMapsterModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<IAutoObjectMappingProvider, MapsterAutoObjectMappingProvider>();
    }
}