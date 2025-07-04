using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;
using YayZent.Framework.Bbs.Application.Contracts;
using YayZent.Framework.Bbs.Application.Contracts.IServices;
using YayZent.Framework.Bbs.Domain.Shared;
using YayZent.Framework.Bbs.Infrastructure.AccessLog;

namespace YayZent.Framework.Bbs.Infrastructure;

[DependsOn(typeof(YayZentFrameworkBbsDomainSharedModule),
    typeof(YayZentFrameworkBbsApplicationContractsModule),
    typeof(AbpCachingStackExchangeRedisModule))]
public class YayZentFrameworkBbsInfrastructureModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}