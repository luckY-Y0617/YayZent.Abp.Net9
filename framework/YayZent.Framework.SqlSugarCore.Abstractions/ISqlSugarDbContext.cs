using SqlSugar;

namespace YayZent.Framework.SqlSugarCore.Abstractions;

public interface ISqlSugarDbContext
{
    ISqlSugarClient SqlSugarClient { get; }
    
    void BackupDatabase();
}