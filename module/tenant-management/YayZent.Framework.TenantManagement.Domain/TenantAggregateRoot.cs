using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.TenantManagement;
using YayZent.Framework.SqlSugarCore.Abstractions;
using Check = Volo.Abp.Check;

namespace YayZent.Framework.TenantManagement.Domain;

/// <summary>
/// 租户聚合根实体
/// </summary>
[SugarTable("Tenant")] // 指定数据库表
[DefaultTenantTable]     // 自定义特性，标记为默认租户表
public class TenantAggregateRoot : FullAuditedAggregateRoot<Guid>, IHasEntityVersion
{
    [SugarColumn(IsPrimaryKey = true)]
    public override Guid Id { get; protected set; }

    public string Name { get; private set; } = string.Empty;
    
    public string NormalizedName { get; protected set; } = string.Empty;

    public int EntityVersion { get; protected set; }

    public string TenantConnectionString { get; private set; } = string.Empty;

    public DbType DbType { get; protected set; }

    [SugarColumn(IsIgnore = true)]
    public override ExtraPropertyDictionary ExtraProperties 
    { 
        get => base.ExtraProperties; 
        protected set => base.ExtraProperties = value; 
    }
    
    public TenantAggregateRoot() {}

    protected TenantAggregateRoot(Guid id, string name, DbType dbType, string connectionString) : base(id)
    {
        SetName(name);
        SetConnectionString(dbType, connectionString);
    }
    
    /// <summary>
    /// 设置数据库连接字符串和类型
    /// </summary>
    private void SetConnectionString(DbType dbType, string connectionString)
    {
        Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));
        DbType = dbType;
        TenantConnectionString = connectionString;
    }

    /// <summary>
    /// 设置租户名称
    /// </summary>
    private void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), TenantConsts.MaxNameLength);
        NormalizedName = Name.ToUpper();
    }
}

