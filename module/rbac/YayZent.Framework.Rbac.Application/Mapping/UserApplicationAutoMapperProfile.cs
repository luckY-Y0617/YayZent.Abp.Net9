using AutoMapper;
using YayZent.Framework.Rbac.Application.Contracts.Dtos;
using YayZent.Framework.Rbac.Application.Contracts.Dtos.Role;
using YayZent.Framework.Rbac.Domain.Entities;

namespace YayZent.Framework.Rbac.Application.Mapping;

public class UserApplicationAutoMapperProfile: Profile
{
    public UserApplicationAutoMapperProfile()
    {
        CreateMap<UserAggregateRoot, UserDto>();
        CreateMap<MenuAggregateRoot, MenuDto>();
        CreateMap<RoleAggregateRoot, RoleDto>();
    }
}