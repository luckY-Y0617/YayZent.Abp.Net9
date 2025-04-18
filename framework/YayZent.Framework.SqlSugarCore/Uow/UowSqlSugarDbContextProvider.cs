using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.SqlSugarCore.Uow;

public class UowSqlSugarDbContextProvider<TDbContext>(IUnitOfWorkManager unitOfWorkManager,IConnectionStringResolver connectionStringResolver,
    IServiceProvider serviceProvider, ICancellationTokenProvider cancellationTokenProvider): ISugarDbContextProvider<TDbContext> where TDbContext: ISqlSugarDbContext
{
    public ILogger<UowSqlSugarDbContextProvider<TDbContext>> Logger { get; set; } = NullLogger<UowSqlSugarDbContextProvider<TDbContext>>.Instance;
    private static AsyncLocalDbContextAccessor DbContextInstance => AsyncLocalDbContextAccessor.Instance;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IUnitOfWorkManager _unitOfWorkManager = unitOfWorkManager;
    private readonly IConnectionStringResolver _connectionStringResolver = connectionStringResolver;
    protected readonly ICancellationTokenProvider CancellationTokenProvider = cancellationTokenProvider;

    public virtual async Task<TDbContext> GetDbContextAsync()
    {
        // The default connection name is "Default."
        var connStringName = ConnectionStrings.DefaultConnectionStringName;
        var connString = await ResolveConnStringAsync(connStringName);
        var dbContextKey = $"{this.GetType().Name}_{connString}";
        
        // `UnitOfWorkManager` is a singleton, meaning there is only one instance of it throughout the entire application runtime.
        // The `UnitOfWork` is transient, meaning a new instance is created each time it is requested. To access the current `UnitOfWork`,
        // we retrieve it from the `AsyncLocal` instance.
        var unitOfWork = _unitOfWorkManager.Current;
        if (unitOfWork == null)
        {
            throw new AbpException("DbContext只能在工作单元中使用，当前线程没有开启工作单元");
        }

        if (unitOfWork.FindDatabaseApi(dbContextKey) is SqlSugarDatabaseApi dataBaseApi)
        {
            return (TDbContext)dataBaseApi.DbContext;
        }
        
        dataBaseApi = new SqlSugarDatabaseApi(await CreateDbContextAsync(unitOfWork, connStringName, connString));
        
        unitOfWork.AddDatabaseApi(dbContextKey, dataBaseApi);

        return (TDbContext)((SqlSugarDatabaseApi)dataBaseApi).DbContext;
    }

    protected virtual async Task<string> ResolveConnStringAsync(string connStringName)
    {
        // _connectionStringResolver gets the connection string from AbpDbConnectionOptions,
        // which includes the ConnectionStrings class and is configured in SqlSugarModule.
        return await _connectionStringResolver.ResolveAsync(connStringName);
    }

    protected virtual async Task<TDbContext> CreateDbContextAsync(IUnitOfWork unitOfWork, string connStringName, string connString)
    {
        var creationContext = new SqlSugarDbContextCreationContext(connString, connStringName);

        using (SqlSugarDbContextCreationContext.Use((creationContext)))
        {
            var dbContext = await CreateDbContextAsync(unitOfWork);
            return dbContext;
        }
    }

    /// <summary>
    /// In Abp Framework, UnitOfWork can be transactional or non-transctional.
    /// The transactional UnitOfWork needs to ensure that multiple database operations are in the same transaction.
    /// You need to create a DbContext with transactions that is suitable for write operations (insert, update, delete).
    /// Non-transactional UnitOfWork does not require transactions. DbContext is obtained directly from ServiceProvider
    /// and is not managed by UnitOfWork transactions. It is suitable for read-only queries (query operations do not require transactions).
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <returns></returns>
    protected virtual async Task<TDbContext> CreateDbContextAsync(IUnitOfWork unitOfWork)
    {
        return unitOfWork.Options.IsTransactional
            ? await CreateDbContextWithTransactionAsync(unitOfWork)
            : unitOfWork.ServiceProvider.GetRequiredService<TDbContext>();
    }
    
    protected virtual async Task<TDbContext> CreateDbContextWithTransactionAsync(IUnitOfWork unitOfWork)
    {
        var transactionApiKey = $"SqlSugarCore_{SqlSugarDbContextCreationContext.Current.ConnString}";
        var activeTransaction = unitOfWork.FindTransactionApi(transactionApiKey) as SqlSugarTransactionApi;

        if (activeTransaction == null)
        {
            var dbContext = unitOfWork.ServiceProvider.GetRequiredService<TDbContext>();
            var transaction = new SqlSugarTransactionApi(dbContext);
            unitOfWork.AddTransactionApi(transactionApiKey, transaction);

            await dbContext.SqlSugarClient.Ado.BeginTranAsync();
            return dbContext;
        }
        else
        {
            return (TDbContext)activeTransaction.GetDbContext();
        }
    }
}