using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using YayZent.Framework.SqlSugarCore;
using YayZent.Framework.SqlSugarCore.Abstractions;
using YayZent.Framework.SqlSugarCore.Uow;

namespace YayZent.Framework.SqlSugarCore.Uow;

public class UowSqlSugarDbContextProvider<TDbContext>(
    IUnitOfWorkManager unitOfWorkManager,
    IConnectionStringResolver connectionStringResolver,
    IServiceProvider serviceProvider,
    ICancellationTokenProvider cancellationTokenProvider,
    ICurrentTenant currentTenant,
    ICurrentDbContextAccessor currentDbContextAccessor
) : ISugarDbContextProvider<TDbContext> where TDbContext : ISqlSugarDbContext
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IUnitOfWorkManager _unitOfWorkManager = unitOfWorkManager;
    private readonly IConnectionStringResolver _connectionStringResolver = connectionStringResolver;
    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly ICurrentDbContextAccessor _currentDbContextAccessor = currentDbContextAccessor;
    protected readonly ICancellationTokenProvider CancellationTokenProvider = cancellationTokenProvider;

    public ILogger<UowSqlSugarDbContextProvider<TDbContext>> Logger { get; set; } = NullLogger<UowSqlSugarDbContextProvider<TDbContext>>.Instance;

    public virtual async Task<TDbContext> GetDbContextAsync()
    {
        var connStringName = ConnectionStrings.DefaultConnectionStringName;
        // 根据当前租户，查出真正要用的连接字符串
        var connString = await ResolveConnectionStringAsync(connStringName);
        var tenantId = _currentTenant.Id; // 当前租户Id
        var dbContextKey = $"{typeof(TDbContext).FullName}_{tenantId?.ToString() ?? "Host"}_{connString}";

        var unitOfWork = _unitOfWorkManager.Current
                         ?? throw new AbpException("DbContext只能在工作单元中使用，当前线程没有开启工作单元");

        if (unitOfWork.FindDatabaseApi(dbContextKey) is SqlSugarDatabaseApi databaseApi)
        {
            return (TDbContext)databaseApi.DbContext;
        }

        var dbContext = await CreateOrGetDbContextAsync(unitOfWork, tenantId);

        unitOfWork.AddDatabaseApi(dbContextKey, new SqlSugarDatabaseApi(dbContext));

        return dbContext;
    }

    protected virtual async Task<TDbContext> CreateOrGetDbContextAsync(IUnitOfWork unitOfWork, Guid? tenantId)
    {
        TDbContext dbContext;

        var transactionApiKey = $"SqlSugarCore_{typeof(TDbContext).FullName}_{tenantId?.ToString() ?? "Host"}";

        if (unitOfWork.Options.IsTransactional)
        {
            if (unitOfWork.FindTransactionApi(transactionApiKey) is SqlSugarTransactionApi activeTransaction)
            {
                dbContext = (TDbContext)activeTransaction.GetDbContext();
            }
            else
            {
                dbContext = _serviceProvider.GetRequiredService<TDbContext>();
                var transaction = new SqlSugarTransactionApi(dbContext);
                unitOfWork.AddTransactionApi(transactionApiKey, transaction);

                await dbContext.SqlSugarClient.Ado.BeginTranAsync();
            }
        }
        else
        {
            dbContext = _serviceProvider.GetRequiredService<TDbContext>();
        }

        return dbContext;
    }

    protected virtual async Task<string> ResolveConnectionStringAsync(string connectionStringName)
    {
        // 如果有忽略多租户属性，则用默认租户
        if (typeof(TDbContext).IsDefined(typeof(IgnoreMultiTenancyAttribute), false))
        {
            using (_currentTenant.Change(null))
            {
                return await _connectionStringResolver.ResolveAsync(connectionStringName);
            }
        }

        return await _connectionStringResolver.ResolveAsync(connectionStringName);
    }
}
