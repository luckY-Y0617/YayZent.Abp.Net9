using Volo.Abp.Application.Dtos;

namespace YayZent.Framework.TenantManagement.Application.Contracts.Dtos;

public class TenantSelectOutputDto: EntityDto<Guid>
{
    public string Name { get; set; }
}