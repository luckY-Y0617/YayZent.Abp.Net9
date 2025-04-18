using SqlSugar;
using Volo.Abp.Uow;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.SqlSugarCore.Uow;

public class SqlSugarTransactionApi(ISqlSugarDbContext sqlSugarDbContext): ITransactionApi, ISupportsRollback
{
    private ISqlSugarDbContext _sqlSugarDbContext = sqlSugarDbContext;

    public ISqlSugarDbContext GetDbContext()
    {
        return _sqlSugarDbContext;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _sqlSugarDbContext.SqlSugarClient.Ado.CommitTranAsync();
    }

    public void Dispose()
    {
        _sqlSugarDbContext.SqlSugarClient.Ado.Dispose();
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _sqlSugarDbContext.SqlSugarClient.Ado.RollbackTranAsync();
    }
}