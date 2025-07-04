using AutoMapper;
using YayZent.Framework.Auth.Domain.Entities;
using YayZent.Framework.Rbac.Domain.Shared.Etos;

namespace YayZent.Framework.Auth.Application.Mapping;

public class AuthApplicationAutoMapperProfile: Profile
{
    public AuthApplicationAutoMapperProfile()
    {
        CreateMap<LoginLogAggregateRoot, LoginEventArgs>();
    }
}