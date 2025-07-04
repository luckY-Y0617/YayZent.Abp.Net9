using AutoMapper;
using YayZent.Framework.Blog.Application.Contracts.Dtos.BlogPost;
using YayZent.Framework.Blog.Application.Contracts.Dtos.Category;
using YayZent.Framework.Blog.Domain.Entities;

namespace YayZent.Framework.Blog.Application.Mapping;

public class BlogAutoMapperProfile: Profile
{
    public BlogAutoMapperProfile()
    {
        CreateMap<BlogPostAggregateRoot, BlogPostListOutputDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(x => x.Catergory != null ? x.Catergory.CategoryName : null))
            .ForMember(dest => dest.CreationTime,
                opt => opt.MapFrom(x => x.CreationTime.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.ImageUrl,
                opt => opt.MapFrom(x => x.BlogFile != null ? x.BlogFile.ImageUploadUrl : null));

        CreateMap<BlogPostAggregateRoot, MenuItem>()
            .ForMember(dest => dest.Label,
                opt => opt.MapFrom(x => x.Title))
            .ForMember(dest => dest.Value,
                opt => opt.MapFrom(x => x.Id));
    }
}