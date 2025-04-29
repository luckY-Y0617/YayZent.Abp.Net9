using SqlSugar;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.SqlSugarCore.Factories;

// 工厂类：根据数据库类型选择备份策略
public static class BackupStrategyFactory
{
    public static IBackupStrategy GetBackupStrategy(DbType dbType)
    {
        return dbType switch
        {
            DbType.MySql => new MySqlBackupStrategy(),
            DbType.Sqlite => new SqliteBackupStrategy(),
            DbType.SqlServer => new SqlServerBackupStrategy(),
            _ => throw new NotImplementedException($"未实现 {dbType} 类型的备份策略")
        };
    }
}

// 针对不同数据库类型的备份策略
public class MySqlBackupStrategy : IBackupStrategy
{
    public void Backup(ISqlSugarClient sqlSugarClient, string directory, string file)
    {
        sqlSugarClient.DbMaintenance.BackupDataBase(sqlSugarClient.Ado.Connection.Database, Path.Combine(directory, $"{file}.sql"));
    }
}

public class SqliteBackupStrategy : IBackupStrategy
{
    public void Backup(ISqlSugarClient sqlSugarClient, string directory, string file)
    {
        sqlSugarClient.DbMaintenance.BackupDataBase(null, Path.Combine(directory, $"{file}.db"));
    }
}

public class SqlServerBackupStrategy : IBackupStrategy
{
    public void Backup(ISqlSugarClient sqlSugarClient, string directory, string file)
    {
        sqlSugarClient.DbMaintenance.BackupDataBase(sqlSugarClient.Ado.Connection.Database, Path.Combine(directory, $"{file}.bak"));
    }
}