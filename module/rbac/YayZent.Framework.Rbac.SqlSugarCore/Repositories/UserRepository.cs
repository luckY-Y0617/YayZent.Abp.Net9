using Volo.Abp.DependencyInjection;
using YayZent.Framework.Rbac.Domain.Entities;
using YayZent.Framework.Rbac.Domain.Repositories;
using YayZent.Framework.SqlSugarCore.Abstractions;
using YayZent.Framework.SqlSugarCore.Repositories;

namespace YayZent.Framework.Rbac.SqlSugarCore.Repositories;

public class UserRepository(ISugarDbContextProvider<ISqlSugarDbContext> dbContextProvider): 
    SqlSugarRepository<UserAggregateRoot, Guid>(dbContextProvider), IUserRepository, ITransientDependency
{
    public async Task<UserAggregateRoot> GetUserAllInfoAsync(Guid userId)
    {
        var res = new UserAggregateRoot();
        try
        {
            res = await DbQueryable.Includes(x => x.Roles, u => u.Menus)
                .InSingleAsync(userId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        
        return res;
    }
}