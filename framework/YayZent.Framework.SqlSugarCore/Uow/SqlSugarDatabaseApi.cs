using SqlSugar;
using Volo.Abp.Uow;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.SqlSugarCore.Uow;

public class SqlSugarDatabaseApi(ISqlSugarDbContext dbContext): IDatabaseApi
{
    public ISqlSugarDbContext DbContext { get; } = dbContext;
}