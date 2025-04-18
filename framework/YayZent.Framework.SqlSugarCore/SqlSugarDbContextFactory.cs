using System.Collections.Concurrent;
using System.Reflection;
using Dm;
using Microsoft.Extensions.Options;
using SqlSugar;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;
using YayZent.Framework.SqlSugarCore.Abstractions;
using Check = Volo.Abp.Check;

namespace YayZent.Framework.SqlSugarCore;

public class SqlSugarDbContextFactory: ISqlSugarDbContext
{
    public ISqlSugarClient SqlSugarClient { get; private set; }
    private IAbpLazyServiceProvider LazyServiceProvider { get; }
    public DbConnOptions DbConnOptions => LazyServiceProvider.LazyGetRequiredService<IOptions<DbConnOptions>>().Value;
    private ISerializeService SerializeService => LazyServiceProvider.LazyGetRequiredService<ISerializeService>();
    private IEnumerable<ISqlSugarDbContextInterceptor> SqlSugarInterceptors => LazyServiceProvider.LazyGetRequiredService<IEnumerable<ISqlSugarDbContextInterceptor>>();
    private static readonly ConcurrentDictionary<string, ConnectionConfig> ConnectionConfigs = new ConcurrentDictionary<string, ConnectionConfig>();

    public SqlSugarDbContextFactory(IAbpLazyServiceProvider lazyServiceProvider)
    {
        LazyServiceProvider = lazyServiceProvider;

        var connectionString = GetCurrentConnectionString();
        var connectionConfig = BuildConnectionConfig(action: options =>
        {
            options.ConnectionString = connectionString;
            options.DbType = GetCurrentDbType();
        });
        
        SqlSugarClient = new SqlSugarClient(connectionConfig);
        
        SetDbAop(SqlSugarClient);
    }
    
    protected string GetCurrentConnectionString()
    {
        var connectionStringResolver = LazyServiceProvider.LazyGetRequiredService<IConnectionStringResolver>();
        var connectionString = AsyncHelper.RunSync(() => connectionStringResolver.ResolveAsync());

        if (connectionString.IsNullOrWhiteSpace())
        {
            Check.NotNull(DbConnOptions.Url, "数据库连接字符串未配置");
        }
        return connectionString;
    }

    protected DbType GetCurrentDbType()
    {
        return DbType.MySql;
    }

    protected ConnectionConfig BuildConnectionConfig(Action<ConnectionConfig>? action = null)
    {
        var dbConnOptions = DbConnOptions;

        #region 组装Options
        if (dbConnOptions.DbType is null)
        {
            throw new ArgumentException("DbType配置为空");
        }

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
            DbType = dbConnOptions.DbType ?? DbType.Sqlite,
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

    public virtual void BackupDatabase()
    {
        string directory = "backup_database";
        string file = DateTime.Now.ToString("yyyyMMddHHmmss") + $"_{SqlSugarClient.Ado.Connection.Database}";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        switch (DbConnOptions.DbType)
        {
            case DbType.MySql:
                SqlSugarClient.DbMaintenance.BackupDataBase(SqlSugarClient.Ado.Connection.Database, $"{Path.Combine(directory, file)}.sql");
                break;
            case DbType.Sqlite:
                SqlSugarClient.DbMaintenance.BackupDataBase(null, $"{file}.db");
                break;
            case DbType.SqlServer:
                SqlSugarClient.DbMaintenance.BackupDataBase(SqlSugarClient.Ado.Connection.Database, $"{Path.Combine(directory, file)}.bak");
                break;
            default:
                throw new NotImplementedException("其他数据库备份未实现");
        }
    }
}