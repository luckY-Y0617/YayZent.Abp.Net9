using SqlSugar;

namespace YayZent.Framework.SqlSugarCore.Abstractions;

public class SqlSugarDbContextCreationContext(string connectionString, DbType dbType)
{
    public string ConnectionString { get; } = connectionString;
    
    public DbType DbType { get; } = dbType;
    
}