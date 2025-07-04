using SqlSugar;
using Volo.Abp.Domain.Entities.Auditing;
using YayZent.Framework.Bbs.Domain.Shared.Enums;

namespace YayZent.Framework.Bbs.Domain.Entities.Access;

[SugarTable("AccessLog")]
public class AccessLogAggregateRoot: AuditedAggregateRoot<Guid>
{
    public long Count { get;set; }
    
    public AccessLogTypeEnum AccessLogType { get; set; }
    
    public void ClickOnce()
    {
        Count++;
    }
    
}