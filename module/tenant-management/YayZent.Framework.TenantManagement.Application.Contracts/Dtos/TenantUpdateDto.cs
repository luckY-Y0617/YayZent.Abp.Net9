using SqlSugar;

namespace YayZent.Framework.TenantManagement.Application.Contracts.Dtos;

public class TenantUpdateDto
{
    public string? Name { get;  set; }
    public int? EntityVersion { get;  set; }

    public string? TenantConnectionString { get;  set; }

    public DbType? DbType { get;  set; }
}