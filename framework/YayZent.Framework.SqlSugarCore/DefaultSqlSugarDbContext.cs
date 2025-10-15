using SqlSugar;
using Volo.Abp;
using Volo.Abp.Threading;
using YayZent.Framework.SqlSugarCore.Abstractions;
using YayZent.Framework.SqlSugarCore.Factories;

namespace YayZent.Framework.SqlSugarCore
{
    public class DefaultSqlSugarDbContext : ISqlSugarDbContext
    {
        public ISqlSugarClient SqlSugarClient { get; set; }

        private readonly ISqlSugarDbClientFactory _sqlSugarDbClientFactory;
        private readonly string _backupDirectory;

        public DefaultSqlSugarDbContext(
            ISqlSugarDbClientFactory sqlSugarDbClientFactory,
            string backupDirectory = "backup_database") // 可配置备份目录
        {
            _sqlSugarDbClientFactory = sqlSugarDbClientFactory;
            _backupDirectory = backupDirectory;
            SqlSugarClient = AsyncHelper.RunSync(() => _sqlSugarDbClientFactory.InitAsync()) ;
        }

        // 切换数据库上下文
        public async Task<IDisposable> Use(SqlSugarDbContextCreationContext creationContext)
        {
            var parentSqlSugarClient = SqlSugarClient; // 保存当前的 DbContext

            var sqlSugarClient = await _sqlSugarDbClientFactory.CreateAsync(creationContext);
            SqlSugarClient = sqlSugarClient;

            return new DisposeAction(() =>
            {
                SqlSugarClient = parentSqlSugarClient;
            });
        }

        /// <summary>
        /// 备份的过程可能会涉及到 数据库的锁，或者备份过程中如何优化性能（例如异步备份），你可以考虑优化这部分功能，使备份操作不影响主数据库的性能，尤其是在高并发的场景下。
        /// 并发控制：如果你的备份操作有很多任务同时运行，考虑使用任务队列或者一些异步技术（比如 Task.Run）来异步执行备份，避免阻塞主线程。
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual void BackupDatabase()
        {
            string file = $"{DateTime.Now:yyyyMMddHHmmss}_{SqlSugarClient.Ado.Connection.Database}";
            
            try
            {
                // 检查并创建备份目录
                if (!Directory.Exists(_backupDirectory))
                {
                    Directory.CreateDirectory(_backupDirectory);
                }

                // 使用策略模式处理不同数据库类型的备份
                var backupStrategy = BackupStrategyFactory.GetBackupStrategy(SqlSugarClient.CurrentConnectionConfig.DbType);
                backupStrategy?.Backup(SqlSugarClient, _backupDirectory, file);
            }
            catch (Exception ex)
            {
                // 捕获并抛出具体异常，便于排查问题
                throw new InvalidOperationException("数据库备份失败", ex);
            }
        }
    }

    




}
