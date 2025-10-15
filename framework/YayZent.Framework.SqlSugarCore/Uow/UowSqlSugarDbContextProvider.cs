using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.SqlSugarCore.Uow;

public class UowSqlSugarDbContextProvider<TDbContext>(
    IUnitOfWorkManager unitOfWorkManager,
    IServiceProvider serviceProvider,
    ICurrentTenant currentTenant
) : ISugarDbContextProvider<TDbContext> where TDbContext : ISqlSugarDbContext
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IUnitOfWorkManager _unitOfWorkManager = unitOfWorkManager;
    private readonly ICurrentTenant _currentTenant = currentTenant;

    public ILogger<UowSqlSugarDbContextProvider<TDbContext>> Logger { get; set; } = NullLogger<UowSqlSugarDbContextProvider<TDbContext>>.Instance;

    public async Task<TDbContext> GetDbContextAsync()
    {
        var dbContextKey = $"SqlSugarCore_{typeof(TDbContext).FullName}_{ _currentTenant.Id?.ToString() ?? Guid.Empty.ToString()}";

        var unitOfWork = _unitOfWorkManager.Current ?? throw new AbpException("DbContext只能在工作单元中使用，当前线程没有开启工作单元");

        if (unitOfWork.FindDatabaseApi(dbContextKey) is SqlSugarDatabaseApi databaseApi)
        {
            return (TDbContext)databaseApi.DbContext;
        }

        var dbContext = await CreateOrGetDbContextAsync(unitOfWork);

        unitOfWork.AddDatabaseApi(dbContextKey, new SqlSugarDatabaseApi(dbContext));

        return dbContext;
    }

    protected async Task<TDbContext> CreateOrGetDbContextAsync(IUnitOfWork unitOfWork)
    {
        TDbContext dbContext;

        var transactionApiKey = $"SqlSugarCore_{typeof(TDbContext).FullName}_{_currentTenant.Id?.ToString() ?? Guid.Empty.ToString()}";

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

                //事务是绑定在 SqlSugarClient 实例上的，且具体到 Connection + Transaction 对象组合；
                await dbContext.SqlSugarClient.Ado.BeginTranAsync();
            }
        }
        else
        {
            dbContext = _serviceProvider.GetRequiredService<TDbContext>();
        }

        return dbContext;
    }
}
