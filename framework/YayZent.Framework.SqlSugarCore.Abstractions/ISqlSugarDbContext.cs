using SqlSugar;

namespace YayZent.Framework.SqlSugarCore.Abstractions;

public interface ISqlSugarDbContext
{
    ISqlSugarClient SqlSugarClient { get;  set; }
    
    void BackupDatabase();
}