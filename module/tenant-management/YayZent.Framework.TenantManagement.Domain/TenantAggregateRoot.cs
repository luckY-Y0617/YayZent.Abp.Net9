using JetBrains.Annotations;
using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.TenantManagement;
using YayZent.Framework.SqlSugarCore.Abstractions;
using Check = Volo.Abp.Check;

namespace YayZent.Framework.TenantManagement.Domain
{
    /// <summary>
    /// 租户聚合根实体
    /// </summary>
    [SugarTable("Tenant")] // 指定数据库表
    [DefaultTenantTable]     // 自定义特性，标记为默认租户表
    public class TenantAggregateRoot : FullAuditedAggregateRoot<Guid>, IHasEntityVersion
    {
        #region 构造函数

        /// <summary>
        /// ORM 框架专用，无参构造函数
        /// </summary>
        public TenantAggregateRoot()
        {
        }

        /// <summary>
        /// 受保护的构造函数，仅限工厂方法内部调用
        /// </summary>
        /// <param name="id">租户ID</param>
        /// <param name="name">租户名称</param>
        protected TenantAggregateRoot(Guid id, [NotNull] string name)
            : base(id)
        {
            SetName(name);
        }

        #endregion

        #region 属性

        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }

        public virtual string Name { get; protected set; }
        
        public virtual string NormalizedName { get; protected set; }

        public int EntityVersion { get; protected set; }

        public string TenantConnectionString { get; protected set; }

        public DbType DbType { get; protected set; }

        [SugarColumn(IsIgnore = true)]
        public override ExtraPropertyDictionary ExtraProperties 
        { 
            get => base.ExtraProperties; 
            protected set => base.ExtraProperties = value; 
        }

        #endregion

        #region 工厂方法

        /// <summary>
        /// 创建新的租户对象
        /// </summary>
        /// <param name="id">租户ID</param>
        /// <param name="name">租户名称</param>
        /// <returns>租户聚合根实例</returns>
        public static TenantAggregateRoot Create(Guid id, [NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name), TenantConsts.MaxNameLength);
            return new TenantAggregateRoot(id, name);
        }

        #endregion

        #region 方法

        /// <summary>
        /// 设置数据库连接字符串和类型
        /// </summary>
        public virtual void SetConnectionString(DbType dbType, string connectionString)
        {
            Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));
            DbType = dbType;
            TenantConnectionString = connectionString;
        }

        /// <summary>
        /// 设置租户名称
        /// </summary>
        private void SetName([NotNull] string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), TenantConsts.MaxNameLength);
        }

        #endregion
    }
}
