using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Auditing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using YayZent.Framework.AuditLogging.Domain.Repositories;
using YayZent.Framework.Core.Helper;

namespace YayZent.Framework.AuditLogging.Domain;

public class AuditingStore: IAuditingStore, ITransientDependency
{
    public ILogger<AuditingStore> Logger { get; }
    
    protected IAuditLogRepository AuditLogRepository { get; }
    
    protected IUnitOfWorkManager UnitOfWorkManager { get; }
    
    protected AbpAuditingOptions Options { get; }
    
    protected IAuditLogInfoToAuditLogConverter Converter { get; }

    public AuditingStore(ILogger<AuditingStore> logger, IAuditLogRepository auditLogRepository,
        IUnitOfWorkManager unitOfWorkManager, IOptions<AbpAuditingOptions> options, IAuditLogInfoToAuditLogConverter converter)
    {
        Logger = logger;
        AuditLogRepository = auditLogRepository;
        UnitOfWorkManager = unitOfWorkManager;
        Options = options.Value;
        Converter = converter;
    }
    
    public virtual async Task SaveAsync(AuditLogInfo auditInfo)
    {
        if (!Options.HideErrors)
        {
            await SaveLogAsync(auditInfo);
            return;
        }

        try
        {
            await SaveLogAsync(auditInfo);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Could not save the audit log object: " + Environment.NewLine + auditInfo.ToString());
            Logger.LogException(ex, LogLevel.Error);
        }
    }

    protected virtual async Task SaveLogAsync(AuditLogInfo auditInfo)
    {
        Logger.LogDebug("请求追踪:" + JsonHelper.SerializeWithDateFormat(auditInfo, "yyyy-MM-dd HH:mm:ss"));
        using (var uow = UnitOfWorkManager.Begin())
        {
            await AuditLogRepository.InsertAsync(await Converter.ConvertAsync(auditInfo));
            await uow.CompleteAsync();
        }
    }
}