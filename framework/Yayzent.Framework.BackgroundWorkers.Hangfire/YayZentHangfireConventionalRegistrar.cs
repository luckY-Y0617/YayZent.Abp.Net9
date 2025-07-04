using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.DependencyInjection;

namespace Yayzent.Framework.BackgroundWorkers.Hangfire;

/// <summary>
/// Hangfire 后台作业的约定式注册器，
/// 用于根据接口约定自动注册实现类为依赖服务。
/// </summary>
public class YayZentHangfireConventionalRegistrar: DefaultConventionalRegistrar
{
    /// <summary>
    /// 检查类型是否禁用约定注册
    /// </summary>
    /// <param name="type">要检查的类型</param>
    /// <returns>如果类型不是 IHangfireBackgroundWorker 或已被禁用则返回 true</returns>
    protected override bool IsConventionalRegistrationDisabled(Type type)
    {
        return !typeof(IHangfireBackgroundWorker).IsAssignableFrom(type) || 
               base.IsConventionalRegistrationDisabled(type);
    }

    /// <summary>
    /// 获取要暴露的服务类型列表
    /// </summary>
    /// <param name="type">实现类型</param>
    /// <returns>服务类型列表</returns>
    protected override List<Type> GetExposedServiceTypes(Type type)
    {
        return new List<Type>
        {
            typeof(IHangfireBackgroundWorker)
        };
    }
}