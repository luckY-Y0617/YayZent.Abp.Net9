using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.DependencyInjection;
using YayZent.Framework.Bbs.Application.Contracts.IServices;

namespace YayZent.Framework.Bbs.Infrastructure.AccessLog;

// 本地进程/线程内的原子操作，只保证在单台机器的同一个进程中的多线程间线程安全。
public class RedisAccessLogCounter : IAccessLogCounter, ITransientDependency
{
    private static int _count = 0;

    /// <summary>
    /// 原子性累加
    /// </summary>
    public void Increment()
    {
        Interlocked.Increment(ref _count);
    }


    public Task<int> GetAndResetCountAsync()
    {
        return Task.FromResult(Interlocked.Exchange(ref _count, 0));
    }

    public Task ResetAsync()
    {
        Interlocked.Exchange(ref _count, 0);
        return Task.CompletedTask;
    }
}