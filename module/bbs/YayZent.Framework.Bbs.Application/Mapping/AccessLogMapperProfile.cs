using AutoMapper;
using YayZent.Framework.Bbs.Application.Contracts.Dtos;
using YayZent.Framework.Bbs.Domain.Entities.Access;

namespace YayZent.Framework.Bbs.Application.Mapping;

public class AccessLogMapperProfile: Profile
{
    public AccessLogMapperProfile()
    {
        CreateMap<AccessLogAggregateRoot, AccessLogDto>()
            .ForMember(dest => dest.CreationTime,
                opt => opt.MapFrom(src => src.CreationTime.Date.ToString("yyyy-MM-dd HH:mm:ss")));
    }
}