using YayZent.Framework.Rbac.Domain.Entities;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Rbac.Domain.Repositories;

public interface IUserRepository: ISqlSugarRepository<UserAggregateRoot>
{
    Task<UserAggregateRoot> GetUserAllInfoAsync(Guid userId);
}