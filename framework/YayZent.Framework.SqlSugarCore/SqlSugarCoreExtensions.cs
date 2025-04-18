using Microsoft.Extensions.DependencyInjection;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.SqlSugarCore;

public static class SqlSugarCoreExtensions
{
    public static IServiceCollection AddDbContext<TDbContext>(this IServiceCollection service,
        ServiceLifetime lifetime = ServiceLifetime.Transient) where TDbContext : class, ISqlSugarDbContextInterceptor
    {
        service.AddTransient<ISqlSugarDbContextInterceptor, TDbContext>();
        return service;
    }
    
    public static IServiceCollection AddDbContext<TDbContext>(this IServiceCollection service,
        Action<DbConnOptions> options) where TDbContext : class, ISqlSugarDbContextInterceptor
    {
        service.Configure<DbConnOptions>(options.Invoke);
        service.AddDbContext<TDbContext>();
        return service;
    }
}