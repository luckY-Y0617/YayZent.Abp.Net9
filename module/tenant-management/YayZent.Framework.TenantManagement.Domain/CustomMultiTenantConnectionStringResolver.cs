using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace YayZent.Framework.TenantManagement.Domain;

public class CustomMultiTenantConnectionStringResolver: DefaultConnectionStringResolver
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CustomMultiTenantConnectionStringResolver> _logger;

    public CustomMultiTenantConnectionStringResolver(
        IOptionsMonitor<AbpDbConnectionOptions> options,
        ICurrentTenant currentTenant,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<CustomMultiTenantConnectionStringResolver> logger)
        : base(options)
    {
        _currentTenant = currentTenant;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// 异步解析指定名称的连接字符串，基于当前租户上下文
    /// </summary>
    /// <param name="connectionStringName">连接字符串名称（可选）</param>
    /// <returns>解析后的连接字符串</returns>
    public override async Task<string> ResolveAsync(string? connectionStringName = null)
    {
        // 如果当前租户 ID 为空（例如：单租户应用场景或未切换租户），则使用默认逻辑解析连接字符串
        if (_currentTenant.Id == null)
        {
            return await base.ResolveAsync(connectionStringName);
        }

        var tenantId = _currentTenant.Id.Value;
        // 查找租户配置信息
        var tenantConfig = await FindTenantConfigurationAsync(tenantId);
        if (tenantConfig == null || tenantConfig.ConnectionStrings.IsNullOrEmpty())
        {
            throw new BusinessException($"未找到租户 (ID: {tenantId}) 的数据库配置信息！");
        }

        // 获取租户的默认连接字符串（可为空）
        var tenantDefaultConnectionString = tenantConfig.ConnectionStrings?.Default;

        // 如果请求的是默认连接字符串
        if (connectionStringName == null ||
            connectionStringName == ConnectionStrings.DefaultConnectionStringName)
        {
            if (!tenantDefaultConnectionString.IsNullOrWhiteSpace())
            {
                // 返回租户默认连接字符串
                _logger.LogInformation("使用租户 (ID: {tenantId}) 的默认连接字符串。", tenantId);
                return tenantDefaultConnectionString;
            }

            // 租户未配置默认连接字符串，使用全局默认连接字符串
            _logger.LogInformation("租户 (ID: {tenantId}) 未配置默认连接字符串，使用全局默认。", tenantId);
            return Options.ConnectionStrings.Default!;
        }

        // 请求的是特定名称的连接字符串
        // 尝试获取租户为该名称配置的连接字符串
        var tenantConnString = tenantConfig.ConnectionStrings?.GetOrDefault(connectionStringName);
        if (!tenantConnString.IsNullOrWhiteSpace())
        {
            // 找到租户特定连接字符串
            _logger.LogInformation("使用租户 (ID: {tenantId}) 的连接字符串 '{connName}'。", tenantId, connectionStringName);
            return tenantConnString;
        }

        // 如果数据库映射存在并启用了租户隔离，尝试使用映射数据库名称获取连接字符串
        var database = Options.Databases.GetMappedDatabaseOrNull(connectionStringName);
        if (database is { IsUsedByTenants: true })
        {
            tenantConnString = tenantConfig.ConnectionStrings?.GetOrDefault(database.DatabaseName);
            if (!tenantConnString.IsNullOrWhiteSpace())
            {
                // 找到租户特定连接字符串（映射后的数据库名）
                _logger.LogInformation(
                    "使用租户 (ID: {tenantId}) 的连接字符串 '{dbName}'（原名称 '{connName}'）。",
                    tenantId, database.DatabaseName, connectionStringName);
                return tenantConnString;
            }
        }

        // 如果租户配置了默认连接字符串，则使用租户默认连接字符串
        if (!tenantDefaultConnectionString.IsNullOrWhiteSpace())
        {
            _logger.LogInformation("租户 (ID: {tenantId}) 未配置 '{connName}' 的连接字符串，使用租户默认连接字符串。",
                tenantId, connectionStringName);
            return tenantDefaultConnectionString;
        }

        // 最终回退到全局默认连接字符串
        _logger.LogInformation("租户 (ID: {tenantId}) 未配置 '{connName}' 的连接字符串，使用全局默认连接字符串。",
            tenantId, connectionStringName);
        return await base.ResolveAsync(connectionStringName);
    }

    /// <summary>
    /// 异步查找并返回指定租户 ID 的租户配置信息
    /// </summary>
    /// <param name="tenantId">租户 ID</param>
    /// <returns>租户配置信息，若未找到则返回 null</returns>
    private async Task<TenantConfiguration?> FindTenantConfigurationAsync(Guid tenantId)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var tenantStore = scope.ServiceProvider.GetRequiredService<ITenantStore>();
            return await tenantStore.FindAsync(tenantId);
        }
    }

}