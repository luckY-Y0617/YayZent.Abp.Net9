using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using YayZent.Framework.Rbac.Domain.Entities;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Rbac.SqlSugarCore.DataSeeds;

public class UserDataSeed(IGuidGenerator guidGenerator, ISqlSugarRepository<UserAggregateRoot> userRepository): IDataSeedContributor, ITransientDependency
{
    private readonly IGuidGenerator _guidGenerator = guidGenerator;
    private ISqlSugarRepository<UserAggregateRoot> _userRepository = userRepository;

    public List<UserAggregateRoot> GetDataSeed()
    {
        var list = new List<UserAggregateRoot>();
        
        var adminUser = UserAggregateRoot.RegisterByEmail(
            userName: "admin",
            password: "admin123",
            email: "1735671830@qq.com"
        );

        list.Add(adminUser);
        
        var defaultUser = UserAggregateRoot.RegisterByEmail(
            userName: "default",
            password: "default123",
            email: "29077095600@qq.com"
        );
        list.Add(defaultUser);
        return list;
    }
    
    public async Task SeedAsync(DataSeedContext context)
    {
        if(! await _userRepository.AnyAsync(x => true))
        {
            await _userRepository.InsertManyAsync(GetDataSeed());
        }
    }
}