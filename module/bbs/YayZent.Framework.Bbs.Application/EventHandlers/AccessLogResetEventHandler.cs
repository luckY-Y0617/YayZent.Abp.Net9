using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EventBus;
using YayZent.Framework.Bbs.Application.Contracts.IServices;
using YayZent.Framework.Bbs.Domain.Shared.Consts;
using YayZent.Framework.Bbs.Domain.Shared.Etos;

namespace YayZent.Framework.Bbs.Application.EventHandlers;

public class AccessLogResetEventHandler: ILocalEventHandler<AccessLogResetArgs>,
    ITransientDependency
{
    private readonly IConfiguration _configuration;
    private readonly AbpDistributedCacheOptions _cacheOptions;
    private readonly IAbpDistributedLock _distributedLock;
    private readonly IAccessLogCounter _accessLogCounter;
    private readonly IConnectionMultiplexer _reids;

    public AccessLogResetEventHandler(
        IConfiguration configuration,
        IOptions<AbpDistributedCacheOptions> cacheOptions,
        IAbpDistributedLock distributedLock,
        IAccessLogCounter accessLogCounter,
        IConnectionMultiplexer reids)
    {
        _configuration = configuration;
        _cacheOptions = cacheOptions.Value;
        _distributedLock = distributedLock;
        _accessLogCounter = accessLogCounter;
        _reids = reids;
    }

    private bool EnableRedisCache
    {
        get
        {
            var redisEnabled = _configuration["Redis:IsEnabled"];
            return redisEnabled.IsNullOrEmpty() || bool.Parse(redisEnabled);
        }
    }
    
    //该事件由job定时10秒触发
    public async Task HandleEventAsync(AccessLogResetArgs eventData)
    {
        if (!EnableRedisCache)
        {
            return;
        }

        // 使用分布式锁
        await using var handle = await _distributedLock.TryAcquireAsync(
            "AccessLogLock",
            TimeSpan.FromSeconds(5)
        );

        if (handle == null)
        {
            return;
        }

        try
        {
            //自增长数
            var incrNumber = await _accessLogCounter.GetAndResetCountAsync();

            if (incrNumber > 0)
            {
                var key = $"{_cacheOptions.KeyPrefix}{AccessLogCacheConst.Key}:{DateTime.Now.Date:yyyyMMdd}";
                var db = _reids.GetDatabase();
                await db.StringIncrementAsync(key, incrNumber);
            }
        }
        catch (Exception)
        {
            // Log error if needed
            throw;
        }
    }
}

