using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.MultiTenancy.ConfigurationStore;
using Volo.Abp.Uow;

namespace YayZent.Framework.TenantManagement.Domain;

public class CustomTenantStore: DefaultTenantStore, ITenantStore
{
    private readonly ISqlSugarTenantRepository _tenantRepository;
    private readonly AbpDefaultTenantStoreOptions _tenantOptions;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IMemoryCache _cache;
    private readonly ICurrentTenant _currentTenant;
    private readonly ILogger<CustomTenantStore> _logger;

    // 缓存键前缀
    private const string CacheKeyPrefixId = "Tenant_Id_";
    private const string CacheKeyPrefixName = "Tenant_Name_";

    /// <summary>
    /// 构造函数，注入 SqlSugar 客户端、租户配置选项、内存缓存和日志记录器
    /// </summary>
    public CustomTenantStore(
        ISqlSugarTenantRepository tenantRepository,
        IOptionsMonitor<AbpDefaultTenantStoreOptions> optionsMonitor,
        IMemoryCache memoryCache,
        IUnitOfWorkManager unitOfWorkManager,
        ICurrentTenant currentTenant,
        ILogger<CustomTenantStore> logger): base(optionsMonitor)
    {
        _tenantRepository = tenantRepository;
        _tenantOptions = optionsMonitor.CurrentValue;
        _cache = memoryCache ;
        _unitOfWorkManager = unitOfWorkManager;
        _currentTenant = currentTenant;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// 异步根据租户Id查找租户信息
    /// </summary>
    /// <param name="id">租户唯一标识符（GUID）</param>
    /// <returns>匹配的 TenantConfiguration 实例或 null</returns>
    public new async Task<TenantConfiguration?> FindAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("开始按ID查找租户，租户ID: {TenantId}", id);

            // 首先尝试从缓存中获取租户信息
            string cacheKey = CacheKeyPrefixId + id;
            if (_cache.TryGetValue(cacheKey, out TenantConfiguration? cachedTenant))
            {
                _logger.LogInformation("从缓存中找到租户，租户ID: {TenantId}", id);
                return cachedTenant;
            }

            // 如果缓存中不存在，则检查配置中的静态租户列表（根据 ID 匹配）
            var configTenant = _tenantOptions.Tenants.FirstOrDefault(t => t.Id == id);
            if (configTenant != null)
            {
                // 将配置租户写入缓存，设定过期时间
                _cache.Set(cacheKey, configTenant, TimeSpan.FromMinutes(5));
                return configTenant;
            }


            // 查询数据库获取租户信息（假设有 TenantEntity 映射数据库中的租户表）
            using (_currentTenant.Change(null))
            {
                using (var uow = _unitOfWorkManager.Begin(isTransactional: false))
                {
                    var dbTenant = await _tenantRepository.DbQueryable
                        .Where(t => t.Id == id)
                        .FirstAsync();
                    if (dbTenant != null)
                    {
                        // 将数据库结果转换为 TenantConfiguration
                        var tenantConfig = new TenantConfiguration(dbTenant.Id, dbTenant.Name);
                        tenantConfig.ConnectionStrings![ConnectionStrings.DefaultConnectionStringName] = dbTenant.TenantConnectionString;

                        // 将查询结果存入缓存，设定过期时间
                        _cache.Set(cacheKey, tenantConfig, TimeSpan.FromMinutes(5));
                        _logger.LogInformation("已将租户写入缓存，租户ID: {TenantId}", tenantConfig.Id);

                        return tenantConfig;
                    }
                    // 未找到租户时返回 null
                    _logger.LogInformation("未找到指定ID的租户，租户ID: {TenantId}", id);
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            string errorMsg = $"按ID查找租户时发生异常，租户ID: {id}";
            _logger.LogError(ex, errorMsg);
            throw new Exception(errorMsg, ex);
        }
    }

    /// <summary>
    /// 异步根据租户名称（规范化）查找租户信息
    /// </summary>
    /// <param name="name">租户名称（已规范化，例如大写）</param>
    /// <returns>匹配的 TenantConfiguration 实例或 null</returns>
    public new async Task<TenantConfiguration?> FindAsync(string name)
    {
        try
        {
            _logger.LogInformation("开始按名称查找租户，租户名称: {TenantName}", name);

            // 首先尝试从缓存中获取租户信息
            string cacheKey = CacheKeyPrefixName + name;
            if (_cache.TryGetValue(cacheKey, out TenantConfiguration? cachedTenant))
            {
                _logger.LogInformation("从缓存中找到租户，租户名称: {TenantName}", name);
                return cachedTenant;
            }

            // 如果缓存中不存在，则检查配置中的静态租户列表（根据规范化名称匹配）
            var configTenant = _tenantOptions.Tenants.FirstOrDefault(t => t.NormalizedName == name);
            if (configTenant != null)
            {
                _logger.LogInformation("在配置租户列表中找到租户，租户名称: {TenantName}", name);
                // 将配置租户写入缓存，设定过期时间
                _cache.Set(cacheKey, configTenant, TimeSpan.FromMinutes(5));
                _logger.LogInformation("已将租户写入缓存，租户名称: {TenantName}", name);
                return configTenant;
            }

            // 配置中也未找到，继续查询数据库
            _logger.LogInformation("缓存未命中，开始查询数据库，租户名称: {TenantName}", name);

            // 查询数据库获取租户信息（假设有 TenantEntity 映射数据库中的租户表）
            var dbTenantList = await _tenantRepository.DbQueryable
                .Where(t => t.Name == name)
                .ToListAsync();
            var dbTenant = dbTenantList.FirstOrDefault();

            if (dbTenant != null)
            {
                var tenantConfig = new TenantConfiguration(dbTenant.Id, dbTenant.Name);
                _logger.LogInformation("从数据库查询到租户，租户名称: {TenantName}", tenantConfig.Name);

                // 将查询结果存入缓存，设定过期时间
                _cache.Set(cacheKey, tenantConfig, TimeSpan.FromMinutes(5));
                _logger.LogInformation("已将租户写入缓存，租户名称: {TenantName}", tenantConfig.Name);

                return tenantConfig;
            }

            // 未找到租户时返回 null
            _logger.LogInformation("未找到指定名称的租户，租户名称: {TenantName}", name);
            return null;
        }
        catch (Exception ex)
        {
            string errorMsg = $"按名称查找租户时发生异常，租户名称: {name}";
            _logger.LogError(ex, errorMsg);
            throw new Exception(errorMsg, ex);
        }
    }

    /// <summary>
    /// 异步获取所有租户列表
    /// </summary>
    /// <param name="includeDetails">是否包含详细信息（目前未使用）</param>
    /// <returns>所有租户的列表</returns>
    public new async Task<IReadOnlyList<TenantConfiguration>> GetListAsync(bool includeDetails = false)
    {
        try
        {
            _logger.LogInformation("开始获取所有租户列表");

            var resultList = new List<TenantConfiguration>();

            // 首先将配置中的租户添加到结果列表中
            if (_tenantOptions.Tenants.Length > 0)
            {
                _logger.LogInformation("加载配置中的租户，数量: {ConfigCount}", _tenantOptions.Tenants.Length);
                resultList.AddRange(_tenantOptions.Tenants);
            }

            // 查询数据库中的租户信息
            var dbTenantEntities = await _tenantRepository.DbQueryable.ToListAsync();
            if (dbTenantEntities is { Count: > 0 })
            {
                _logger.LogInformation("加载数据库中的租户，数量: {DbCount}", dbTenantEntities.Count);
                foreach (var dbTenant in dbTenantEntities)
                {
                    // 避免重复：如果配置列表已有相同租户，则跳过
                    if (resultList.Any(t => t.Id == dbTenant.Id))
                    {
                        continue;
                    }

                    var tenantConfig = new TenantConfiguration(dbTenant.Id, dbTenant.Name);
                    resultList.Add(tenantConfig);
                }
            }
            else
            {
                _logger.LogInformation("数据库中未找到任何租户信息");
            }

            return resultList;
        }
        catch (Exception ex)
        {
            var errorMsg = "获取租户列表时发生异常";
            _logger.LogError(ex, errorMsg);
            throw new Exception(errorMsg, ex);
        }
    }
}