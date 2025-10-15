using AutoMapper;
using YayZent.Framework.Blog.Application.Contracts.Dtos.Category;
using YayZent.Framework.Blog.Domain.Entities;

namespace YayZent.Framework.Blog.Application.Mapping;

public class CategoryAutoMapperProfile: Profile
{
    public CategoryAutoMapperProfile()
    {
        CreateMap<CategoryAggregateRoot, CategoryGetListOutputDto>();
        CreateMap<CatergoryCreateDto, CategoryAggregateRoot>();
        CreateMap<CategoryAggregateRoot, CategoryGetOutputDto>();
        CreateMap<CategoryUpdateDto, CategoryAggregateRoot>();
    }
    
}