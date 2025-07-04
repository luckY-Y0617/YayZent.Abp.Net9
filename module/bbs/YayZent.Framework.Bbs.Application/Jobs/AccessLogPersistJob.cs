using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using YayZent.Framework.Bbs.Domain.Entities.Access;
using YayZent.Framework.Bbs.Domain.Shared.Consts;
using YayZent.Framework.Bbs.Domain.Shared.Enums;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Bbs.Application.Jobs;

public class AccessLogPersistJob : HangfireBackgroundWorkerBase, ITransientDependency
{
    private readonly ISqlSugarRepository<AccessLogAggregateRoot> _repository;
    private readonly AbpDistributedCacheOptions _cacheOptions;
    private readonly IConnectionMultiplexer _reids;

    public AccessLogPersistJob(
        ISqlSugarRepository<AccessLogAggregateRoot> repository,
        IOptions<AbpDistributedCacheOptions> cacheOptions,
        IConnectionMultiplexer reids)
    {
        _repository = repository;
        _cacheOptions = cacheOptions.Value;
        _reids = reids;

        RecurringJobId = "访问日志写入数据库";
        CronExpression = "0 0 * * * ?"; // 每小时执行一次
    }

    private string CacheKeyPrefix => _cacheOptions.KeyPrefix;

    public override async Task DoWorkAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var cacheKey = $"{CacheKeyPrefix}{AccessLogCacheConst.Key}:{DateTime.Now:yyyyMMdd}";

        var db = _reids.GetDatabase();
        // 从缓存中取字符串
        var cachedValue = (await db.StringGetAsync(cacheKey)).ToString();
        var count = long.TryParse(cachedValue, out var parsedCount) ? parsedCount : 0;
        
        if (count == 0)
        {
            return;
        }

        var entity = await _repository.DbQueryable
            .Where(x => x.AccessLogType == AccessLogTypeEnum.Request)
            .Where(x => x.CreationTime.Date == DateTime.Today)
            .FirstAsync(cancellationToken);

        if (entity != null)
        {
            entity.Count = count + 1;
            await _repository.UpdateAsync(entity,true, cancellationToken);
        }
        else
        {
            var newEntity = new AccessLogAggregateRoot
            {
                Count = count,
                AccessLogType = AccessLogTypeEnum.Request
            };
            await _repository.InsertAsync(newEntity, true,cancellationToken);
        }

        // 删除前一天缓存
        var yesterdayKey = $"{CacheKeyPrefix}{AccessLogCacheConst.Key}:{DateTime.Now.AddDays(-1):yyyyMMdd}";
        await db.KeyDeleteAsync(yesterdayKey);
    }
}
