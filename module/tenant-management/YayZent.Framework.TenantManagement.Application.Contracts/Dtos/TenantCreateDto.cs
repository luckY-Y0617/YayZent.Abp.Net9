using SqlSugar;

namespace YayZent.Framework.TenantManagement.Application.Contracts.Dtos;

public class TenantCreateDto
{
    public string Name { get;  set; } = string.Empty;
    
    public string NormalizedName { get;  set; } = string.Empty;
    
    public string TenantConnectionString { get;  set; } = string.Empty;

    public DbType DbType { get;  set; }
}