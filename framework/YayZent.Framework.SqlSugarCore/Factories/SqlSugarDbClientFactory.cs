using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using YayZent.Framework.Core.Extensions;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.SqlSugarCore.Factories;

public class SqlSugarDbClientFactory(IAbpLazyServiceProvider lazyServiceProvider, ILogger<SqlSugarDbClientFactory> logger): ISqlSugarDbClientFactory
{
    private readonly IAbpLazyServiceProvider _lazyServiceProvider = lazyServiceProvider;
    private readonly ILogger<SqlSugarDbClientFactory> _logger = logger;
    private DbConnOptions DbConnOptions => _lazyServiceProvider.LazyGetRequiredService<IOptions<DbConnOptions>>().Value;
    private ISerializeService SerializeService => _lazyServiceProvider.LazyGetRequiredService<ISerializeService>();
    private TenantConfigurationWrapper TenantConfigurationWrapper => _lazyServiceProvider.LazyGetRequiredService<TenantConfigurationWrapper>();
    private IEnumerable<ISqlSugarDbContextInterceptor> SqlSugarInterceptors => _lazyServiceProvider.LazyGetRequiredService<IEnumerable<ISqlSugarDbContextInterceptor>>();

    public async Task<ISqlSugarClient> InitAsync()
    {
        var dbContextCreationContext = await GetCurrentConnectionString();
        var connectionConfig = BuildConnectionConfig(action: options =>
        {
            options.ConnectionString = dbContextCreationContext.ConnectionString;
            options.DbType =  dbContextCreationContext.DbType;
        });
        
        var sqlSugarClient = new SqlSugarClient(connectionConfig);
        
        SetDbAop(sqlSugarClient);
        
        return sqlSugarClient;
    }

    public async Task<ISqlSugarClient> CreateAsync(SqlSugarDbContextCreationContext config)
    {
        var connectionConfig = BuildConnectionConfig(action: options =>
        {
            options.ConnectionString = config.ConnectionString;
            options.DbType = config.DbType;
        });

        var sqlSugarClient = new SqlSugarClient(connectionConfig);

        SetDbAop(sqlSugarClient);

        return await Task.FromResult(sqlSugarClient);
    }

    
    private async Task<SqlSugarDbContextCreationContext> GetCurrentConnectionString()
    {
        if (!DbConnOptions.EnabledSaasMultiTenancy)
        {
            _logger.LogInformation("未开启多租户，使用默认连接");
            return new SqlSugarDbContextCreationContext(DbConnOptions.Url, DbConnOptions.DbType);
        }
        
        // 根据当前租户，查出真正要用的连接字符串
        var tenantConfiguration = await TenantConfigurationWrapper.GetAsync();

        if (tenantConfiguration == null)
        {
            throw new BusinessException("tenant is not configured");
        }

        return new SqlSugarDbContextCreationContext(tenantConfiguration.GetCurrentConnectionString(), tenantConfiguration.GetCurrentDbType());
    }

    protected ConnectionConfig BuildConnectionConfig(Action<ConnectionConfig>? action = null)
    {
        var dbConnOptions = DbConnOptions;

        #region 组装Options

        var slaveConfigs = new List<SlaveConnectionConfig>();
        if (dbConnOptions.EnbaleReadWriteSplitting)
        {
            if (dbConnOptions.ReadWriteSplittingUrl is null)
            {
                throw new ArgumentException("读写分离未配置");
            }
            
            var readWriteSplittingUrl = dbConnOptions.ReadWriteSplittingUrl;
            
            slaveConfigs.AddRange(readWriteSplittingUrl.Select(url => new SlaveConnectionConfig() {ConnectionString = url}));
        }
        #endregion
        
        #region 组装连接config

        var connectionConfig = new ConnectionConfig()
        {
            ConfigId = ConnectionStrings.DefaultConnectionStringName,
            DbType = dbConnOptions.DbType,
            ConnectionString = dbConnOptions.Url,
            IsAutoCloseConnection = true,
            SlaveConnectionConfigs = slaveConfigs,
            ConfigureExternalServices = new ConfigureExternalServices()
            {
                EntityNameService = (type, entity) =>
                {
                    if (dbConnOptions.EnableUnderLine && !entity.DbTableName.Contains("_"))
                    {
                        entity.DbTableName = UtilMethods.ToUnderLine(entity.DbTableName);
                    }
                },
                EntityService = (propertyInfo, columnInfo) =>
                {
                    //是否可空
                    if (new NullabilityInfoContext().Create(propertyInfo).WriteState is NullabilityState.Nullable)
                    {
                        columnInfo.IsNullable = true;
                    }

                    if (dbConnOptions.EnableUnderLine && !columnInfo.IsIgnore && !columnInfo.DbColumnName.Contains("_"))
                    {
                        columnInfo.DbColumnName = UtilMethods.ToUnderLine(columnInfo.DbColumnName);
                    }

                    Action<PropertyInfo, EntityColumnInfo> entityService = (p, c) => { };
                    foreach (var interceptor in SqlSugarInterceptors.OrderBy(x => x.ExecutionOrder))
                    {
                        entityService += interceptor.EntityService;
                    }

                    entityService?.Invoke(propertyInfo, columnInfo);
                }
            }
        };

        action?.Invoke(connectionConfig);
        #endregion
        
        return connectionConfig;
    }

    protected void SetDbAop(ISqlSugarClient sqlSugarClient)
    {
        sqlSugarClient.CurrentConnectionConfig.ConfigureExternalServices.SerializeService = SerializeService;

        Action<string, SugarParameter[]> onSqlExecuting = (s, m) => { };
        Action<string, SugarParameter[]> afterSqlExecuted = (s, m) => { };
        Action<object, DataFilterModel> onDataExecuting = (s, m) => { };
        Action<object, DataAfterModel> afterDataExecuted = (s, m) => { };
        Action<ISqlSugarClient> onSqlSugarClientConfig = (s) => { };

        foreach (var interceptor in SqlSugarInterceptors.OrderBy(x => x.ExecutionOrder))
        {
            onSqlExecuting += interceptor.OnSqlExecuting;
            afterSqlExecuted += interceptor.AfterSqlExecuted;
            onDataExecuting += interceptor.OnDataExecuting;
            afterDataExecuted += interceptor.AfterDataExecuted;
            
            onSqlSugarClientConfig += interceptor.OnSqlSugarClientConfig;
        }
        
        onSqlSugarClientConfig?.Invoke(sqlSugarClient);
        sqlSugarClient.Aop.OnLogExecuted = afterSqlExecuted;
        sqlSugarClient.Aop.OnLogExecuting = onSqlExecuting;
        sqlSugarClient.Aop.DataExecuted = afterDataExecuted;
        sqlSugarClient.Aop.DataExecuting = onDataExecuting;
    }
    
}