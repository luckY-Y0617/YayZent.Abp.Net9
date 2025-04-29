using SqlSugar;
using DbType = SqlSugar.DbType;

namespace YayZent.Framework.SqlSugarCore.Abstractions;

public interface ISqlSugarDbClientFactory
{
    ISqlSugarClient Init();
    
    ISqlSugarClient Create(SqlSugarDbContextCreationContext config);
}