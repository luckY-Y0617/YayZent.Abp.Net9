using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Uow;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.SqlSugarCore.Uow;

public class SqlSugarTransactionApi(ISqlSugarDbContext dbContext) : ITransactionApi
{
    private readonly ISqlSugarDbContext _dbContext = dbContext;
    private bool _completed = false;

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_completed)
        {
            return;
        }

        await _dbContext.SqlSugarClient.Ado.CommitTranAsync();
        _completed = true;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_completed)
        {
            return;
        }

        await _dbContext.SqlSugarClient.Ado.RollbackTranAsync();
        _completed = true;
    }

    public void Dispose()
    {
        // Nothing to dispose here
    }

    public ISqlSugarDbContext GetDbContext()
    {
        return _dbContext;
    }
}