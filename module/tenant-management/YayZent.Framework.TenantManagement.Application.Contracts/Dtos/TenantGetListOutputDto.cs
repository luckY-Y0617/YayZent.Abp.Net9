using SqlSugar;
using Volo.Abp.Application.Dtos;

namespace YayZent.Framework.TenantManagement.Application.Contracts.Dtos;

public class TenantGetListOutputDto: EntityDto<Guid>
{
    public  string? Name { get;  set; }
    
    public string NormalizedName { get;  set; }
    
    public int EntityVersion { get;  set; }

    public string? TenantConnectionString { get;  set; }

    public DbType DbType { get;  set; }
    
    public DateTime CreationTime { get; set; }
}