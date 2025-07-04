using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Local;
using YayZent.Framework.Bbs.Domain.Shared.Etos;

namespace YayZent.Abp.Web.Jobs.bbs;

public class AccessLogFlushJob : HangfireBackgroundWorkerBase, ITransientDependency
{
    private readonly ILocalEventBus _localEventBus;

    public AccessLogFlushJob(ILocalEventBus localEventBus)
    {
        _localEventBus = localEventBus;
        RecurringJobId = "访问AccessLog写入缓存";
        CronExpression = "0 * * * * ?";
    }
    
    public override async Task DoWorkAsync(CancellationToken cancellationToken = new CancellationToken())
    {
       await _localEventBus.PublishAsync(new AccessLogResetArgs());
    }
}
