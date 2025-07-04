using YayZent.Framework.Bbs.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Bbs.Domain.Entities.Access;
using YayZent.Framework.Bbs.Domain.Shared.Enums;
using YayZent.Framework.Core.Extensions;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Bbs.Domain.DomainServices;

public class AccessLogDomainService: IAccessLogDomainService
{
    private readonly ISqlSugarRepository<AccessLogAggregateRoot> _repository;

    public AccessLogDomainService(ISqlSugarRepository<AccessLogAggregateRoot> repository)
    {
        _repository = repository;
    }


    public async Task<List<AccessLogAggregateRoot>> BuildWeekStatistics(AccessLogTypeEnum accessLogType)
    {
        var weekStart = DateTime.Now.GetWeekStart();

        var filtered = (await _repository.DbQueryable
                .Where(x => x.AccessLogType == accessLogType && x.CreationTime >= weekStart)
                .ToListAsync())
                .GroupBy(x => x.CreationTime.Date)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.CreationTime).First());

        var result = Enumerable.Range(0, 7)
            .Select(i =>
            {
                var date = weekStart.AddDays(i).Date;
                return filtered.TryGetValue(date, out var value)
                    ? value
                    : new AccessLogAggregateRoot
                    {
                        CreationTime = date,
                        Count = 0,
                        AccessLogType = accessLogType
                    };
            })
            .ToList();

        return await Task.FromResult(result);
    }
}