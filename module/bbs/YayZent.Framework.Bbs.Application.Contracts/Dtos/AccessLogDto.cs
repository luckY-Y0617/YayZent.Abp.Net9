using YayZent.Framework.Bbs.Domain.Shared.Enums;

namespace YayZent.Framework.Bbs.Application.Contracts.Dtos;

public class AccessLogDto
{
    public int Count { get; set; }
    
    public AccessLogTypeEnum AccessLogType { get; set; }
    
    public string CreationTime { get; set; }
}