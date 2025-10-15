using AutoMapper;
using YayZent.Framework.Core.Extensions;
using YayZent.Framework.TenantManagement.Application.Contracts.Dtos;
using YayZent.Framework.TenantManagement.Domain;

namespace YayZent.Framework.TenantManagement.Application;

public class TenantAutoMapperProfile: Profile
{
    public TenantAutoMapperProfile()
    {
        CreateMap<TenantAggregateRoot, TenantGetListOutputDto>();
        CreateMap<TenantAggregateRoot, TenantGetOutputDto>();
        CreateMap<TenantCreateDto, TenantAggregateRoot>()
            .ForMember(dest => dest.NormalizedName, 
                opt => opt.MapFrom(x => x.Name.GetNormalizedName()))
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(x => x.Name));
        CreateMap<TenantUpdateDto, TenantAggregateRoot>();
    }
}