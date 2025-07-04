using AutoMapper;
using YayZent.Framework.Blog.Application.Contracts.Dtos.Comment;
using YayZent.Framework.Blog.Domain.Entities;

namespace YayZent.Framework.Blog.Application.Mapping;

public class CommentAutoMapperProfile: Profile
{
    public CommentAutoMapperProfile()
    {
        CreateMap<CommentAggregateRoot, GetCommentsOutputDto>()
            .ForMember(dest => dest.CreationTime,
                opt => opt.MapFrom(src => src.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")));
    }
}