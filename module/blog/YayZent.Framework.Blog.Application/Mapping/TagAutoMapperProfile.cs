using AutoMapper;
using YayZent.Framework.Blog.Application.Contracts.Dtos.Tag;
using YayZent.Framework.Blog.Domain.Entities;

namespace YayZent.Framework.Blog.Application.Mapping;

public class TagAutoMapperProfile: Profile
{
    public TagAutoMapperProfile()
    {
        CreateMap<TagAggregateRoot, TagGetListOutputDto>();
    }
}