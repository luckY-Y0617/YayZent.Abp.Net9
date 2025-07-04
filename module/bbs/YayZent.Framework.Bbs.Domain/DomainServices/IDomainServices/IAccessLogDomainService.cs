using Volo.Abp.Domain.Services;
using YayZent.Framework.Bbs.Domain.Entities.Access;
using YayZent.Framework.Bbs.Domain.Shared.Enums;

namespace YayZent.Framework.Bbs.Domain.DomainServices.IDomainServices;

public interface IAccessLogDomainService: IDomainService
{
    Task<List<AccessLogAggregateRoot>> BuildWeekStatistics(AccessLogTypeEnum accessLogType);
}