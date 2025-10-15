using SqlSugar;
using DbType = SqlSugar.DbType;

namespace YayZent.Framework.SqlSugarCore.Abstractions;

public interface ISqlSugarDbClientFactory
{ 
    Task<ISqlSugarClient> InitAsync();
    
    Task<ISqlSugarClient> CreateAsync(SqlSugarDbContextCreationContext config);
}