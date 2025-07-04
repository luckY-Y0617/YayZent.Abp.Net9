using SqlSugar;
using Volo.Abp.Domain.Entities.Auditing;

namespace YayZent.Framework.Auth.Domain.Entities;

[SugarTable("LoginLog")]
public class LoginLogAggregateRoot: AuditedAggregateRoot<Guid>
{
    public string? LoginUser { get; set; }
    
    public string? LoginLoaction { get; set; }
    
    public string? LoginIp { get; set; }
    
    public string? Browser { get; set; }
    
    public string? Os { get; set; }
    
    public string? LogMessage { get; set; }

    public LoginLogAggregateRoot() {}
    
    public LoginLogAggregateRoot (string? loginUser, string? browser, string? os, string? loginIp, string? loginLocation)
    {
        LoginUser = loginUser;
        Browser = browser;
        LoginIp = loginIp;
        Os = os;
        LoginLoaction = loginLocation;
    }
    
}