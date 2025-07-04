using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;
using YayZent.Framework.Bbs.Application.Contracts.Dtos;
using YayZent.Framework.Bbs.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Bbs.Domain.Entities.Access;
using YayZent.Framework.Bbs.Domain.Shared.Enums;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Bbs.Application.Services;

public class AccessLogService : ApplicationService
{
    private readonly IObjectMapper _objectMapper;
    private readonly ISqlSugarRepository<AccessLogAggregateRoot> _repository;
    private readonly IAccessLogDomainService _domainService;

    public AccessLogService(IObjectMapper objectMapper ,ISqlSugarRepository<AccessLogAggregateRoot> repository, IAccessLogDomainService domainService)
    {
        _objectMapper = objectMapper;
        _repository = repository;
        _domainService = domainService;
    }

    public async Task<long> GetSummary()
    {
        var sum = await _repository.DbQueryable
            .Where(x => x.AccessLogType == AccessLogTypeEnum.Request)
            .SumAsync(x => x.Count);
        
        return await Task.FromResult(sum);
    }

    /// <summary>
    /// 首页点击触发
    /// </summary>
    public async Task AccessAsync()
    {
        var last = await _repository.DbQueryable
            .Where(x => x.AccessLogType == AccessLogTypeEnum.HomeClick)
            .OrderByDescending(x => x.CreationTime)
            .FirstAsync();

        if (last == null || last.CreationTime.Date != DateTime.Today)
        {
            await _repository.InsertAsync(new AccessLogAggregateRoot { AccessLogType = AccessLogTypeEnum.HomeClick, Count = 1});
        }
        else
        {
            last.ClickOnce(); // 使用实体方法
            await _repository.UpdateAsync(last);
        }
    }

    /// <summary>
    /// 获取全部访问流量(3个月)
    /// </summary>
    /// <param name="accessLogType"></param>
    /// <returns></returns>
    public async Task<List<AccessLogDto>> GetListAsync([FromQuery] AccessLogTypeEnum accessLogType)
    {
        var entities = await _repository.DbQueryable
            .Where(x => x.AccessLogType == accessLogType)
            .Where(x => x.CreationTime >= DateTime.Now.AddMonths(-3))
            .OrderBy(x => x.CreationTime)
            .ToListAsync();

        var output = _objectMapper.Map<List<AccessLogAggregateRoot>, List<AccessLogDto>>(entities);
        return output;
    }

    /// <summary>
    /// 获取当前周首页点击数据
    /// </summary>
    /// <returns></returns>
    public async Task<List<AccessLogDto>> GetWeekAsync([FromQuery] AccessLogTypeEnum accessLogType)
    {

        var weekStaticData = await _domainService.BuildWeekStatistics(accessLogType);

        return _objectMapper.Map<List<AccessLogAggregateRoot>, List<AccessLogDto>>(weekStaticData);
    }
}
