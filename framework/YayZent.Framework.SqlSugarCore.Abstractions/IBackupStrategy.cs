using SqlSugar;

namespace YayZent.Framework.SqlSugarCore.Abstractions;

// 数据库备份策略接口
public interface IBackupStrategy
{
    void Backup(ISqlSugarClient sqlSugarClient, string directory, string file);
}