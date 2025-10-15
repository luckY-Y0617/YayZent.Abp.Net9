using SqlSugar;
using Volo.Abp.Application.Dtos;

namespace YayZent.Framework.TenantManagement.Application.Contracts.Dtos;

public class TenantGetOutputDto: EntityDto<Guid>
{
    public string Name { get;  set; } = string.Empty;
    
    public int EntityVersion { get;  set; }
    
    public string NormalizedName { get;  set; } = string.Empty;

    public string TenantConnectionString { get;  set; } = string.Empty;

    public DbType DbType { get;  set; }

    public DateTime CreationTime { get; set; }
}