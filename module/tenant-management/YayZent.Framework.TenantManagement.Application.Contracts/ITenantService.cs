using Volo.Abp.Application.Services;
using YayZent.Framework.TenantManagement.Application.Contracts.Dtos;

namespace YayZent.Framework.TenantManagement.Application.Contracts;

public interface ITenantService: ICrudAppService<TenantGetOutputDto, TenantGetListOutputDto, Guid, TenantGetListInputDto, TenantCreateDto, TenantUpdateDto>
{
    
}