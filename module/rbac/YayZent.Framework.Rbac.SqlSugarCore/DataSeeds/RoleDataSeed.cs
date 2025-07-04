using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using YayZent.Framework.Rbac.Domain.Entities;
using YayZent.Framework.Rbac.Domain.Shared.Enums;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Rbac.SqlSugarCore.DataSeeds;

public class RoleDataSeed(IGuidGenerator guidGenerator,ISqlSugarRepository<RoleAggregateRoot> roleAggregateRoot): IDataSeedContributor, ITransientDependency
{
    private readonly IGuidGenerator _guidGenerator = guidGenerator;
    private readonly ISqlSugarRepository<RoleAggregateRoot> _roleRepository = roleAggregateRoot;

    public List<RoleAggregateRoot> GetDataSeed()
    {
        var list = new List<RoleAggregateRoot>();

        var role1 = new RoleAggregateRoot(_guidGenerator.Create())
        {
            RoleName = "管理员",
            RoleCode = "admin",
            DateScope = DataScopeEnum.All,
            OrderNum = 999,
            Remark = "至高无上",
            IsDeleted = false
        };
        
        list.Add(role1);

        var role2 = new RoleAggregateRoot(_guidGenerator.Create())
        {
            RoleName = "默认角色",
            RoleCode = "default",
            DateScope = DataScopeEnum.User,
            OrderNum = 1,
            Remark = "随便看看",
            IsDeleted = false
        };
        list.Add(role2);
        
        return list;
    }
    
    public async Task SeedAsync(DataSeedContext context)
    {
        if(! await _roleRepository.AnyAsync(x => true))
        {
            await _roleRepository.InsertManyAsync(GetDataSeed());
        }
    }
}