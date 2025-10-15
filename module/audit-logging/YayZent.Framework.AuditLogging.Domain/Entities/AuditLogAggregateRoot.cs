using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using YayZent.Framework.AuditLogging.Domain.Shared.Consts;
using YayZent.Framework.Core.Attributes;

namespace YayZent.Framework.AuditLogging.Domain.Entities;

[DisableAuditing]
[SugarTable("AuditLog")]
[SugarIndex($"index_{nameof(ExecutionTime)}", nameof(TenantId), OrderByType.Asc, nameof(ExecutionTime), OrderByType.Asc)]
[SugarIndex($"index_{nameof(ExecutionTime)}_{nameof(UserId)}", nameof(TenantId), OrderByType.Asc, nameof(UserId), OrderByType.Asc, nameof(ExecutionTime), OrderByType.Asc)]
public class AuditLogAggregateRoot : AggregateRoot<Guid>, IMultiTenant
{
    
    [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
    public override Guid Id { get; protected set; }

    public string? ApplicationName { get; set; }

    public Guid? UserId { get; protected set; }

    public string? UserName { get; protected set; }

    public string? TenantName { get; protected set; }

    public Guid? ImpersonatorUserId { get; protected set; }

    public string? ImpersonatorUserName { get; protected set; }

    public Guid? ImpersonatorTenantId { get; protected set; }

    public string? ImpersonatorTenantName { get; protected set; }

    public DateTime? ExecutionTime { get; protected set; }

    public int? ExecutionDuration { get; protected set; }

    public string? ClientIpAddress { get; protected set; }

    public string? ClientName { get; protected set; }

    public string? ClientId { get; set; }

    public string? CorrelationId { get; set; }

    [SugarColumn(Length = 2000)]
    public string? BrowserInfo { get; protected set; }

    public string? HttpMethod { get; protected set; }

    public string? Url { get; protected set; }

    [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? Exceptions { get; protected set; }

    public string? Comments { get; protected set; }

    public int? HttpStatusCode { get; set; }

    public Guid? TenantId { get; protected set; }

    // 导航属性，默认初始化避免null引用
    [Navigate(NavigateType.OneToMany, nameof(EntityChangeEntity.AuditLogId))]
    public List<EntityChangeEntity> EntityChanges { get; protected set; }

    [Navigate(NavigateType.OneToMany, nameof(AuditLogActionEntity.AuditLogId))]
    public List<AuditLogActionEntity> Actions { get; protected set; } 

    
    public AuditLogAggregateRoot()
    {
        EntityChanges = new List<EntityChangeEntity>();
        Actions = new List<AuditLogActionEntity>();
    }

    public AuditLogAggregateRoot(
        Guid id,
        string? applicationName,
        Guid? tenantId,
        string? tenantName,
        Guid? userId,
        string? userName,
        DateTime executionTime,
        int executionDuration,
        string? clientIpAddress,
        string? clientName,
        string? clientId,
        string? correlationId,
        string? browserInfo,
        string? httpMethod,
        string? url,
        int? httpStatusCode,
        Guid? impersonatorUserId,
        string? impersonatorUserName,
        Guid? impersonatorTenantId,
        string? impersonatorTenantName,
        ExtraPropertyDictionary extraProperties,
        List<EntityChangeEntity> entityChanges,
        List<AuditLogActionEntity> actions,
        string? exceptions,
        string? comments) : base(id)
    {
        ApplicationName = applicationName.Truncate(AuditLogConsts.MaxApplicationNameLength);
        TenantId = tenantId;
        TenantName = tenantName.Truncate(AuditLogConsts.MaxTenantNameLength);
        UserId = userId;
        UserName = userName.Truncate(AuditLogConsts.MaxUserNameLength);
        ExecutionTime = executionTime;
        ExecutionDuration = executionDuration;
        ClientIpAddress = clientIpAddress.Truncate(AuditLogConsts.MaxClientIpAddressLength);
        ClientName = clientName.Truncate(AuditLogConsts.MaxClientNameLength);
        ClientId = clientId.Truncate(AuditLogConsts.MaxClientIdLength);
        CorrelationId = correlationId.Truncate(AuditLogConsts.MaxCorrelationIdLength);
        BrowserInfo = browserInfo.Truncate(AuditLogConsts.MaxBrowserInfoLength);
        HttpMethod = httpMethod.Truncate(AuditLogConsts.MaxHttpMethodLength);
        Url = url.Truncate(AuditLogConsts.MaxUrlLength);
        HttpStatusCode = httpStatusCode;
        ImpersonatorUserId = impersonatorUserId;
        ImpersonatorUserName = impersonatorUserName.Truncate(AuditLogConsts.MaxUserNameLength);
        ImpersonatorTenantId = impersonatorTenantId;
        ImpersonatorTenantName = impersonatorTenantName.Truncate(AuditLogConsts.MaxTenantNameLength);

        ExtraProperties = extraProperties ?? new ExtraPropertyDictionary();
        EntityChanges = entityChanges ?? new List<EntityChangeEntity>();
        Actions = actions ?? new List<AuditLogActionEntity>();

        Exceptions = exceptions;
        Comments = comments.Truncate(AuditLogConsts.MaxCommentsLength);
    }

}
