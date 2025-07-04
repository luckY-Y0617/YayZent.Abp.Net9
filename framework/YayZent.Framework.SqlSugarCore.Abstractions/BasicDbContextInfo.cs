using SqlSugar;

namespace YayZent.Framework.SqlSugarCore.Abstractions;

public class BasicDbContextInfo(ISqlSugarClient? sqlSugarClient, DbType dbType)
{
    public ISqlSugarClient? SqlSugarClient { get; } = sqlSugarClient;

    public DbType DbType { get; } = dbType;

}