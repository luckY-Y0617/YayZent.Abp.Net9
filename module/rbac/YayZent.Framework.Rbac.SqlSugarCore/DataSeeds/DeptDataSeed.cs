using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using YayZent.Framework.Rbac.Domain.Entities;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Rbac.SqlSugarCore.DataSeeds;

public class DeptDataSeed(IGuidGenerator guidGenerator,ISqlSugarRepository<DeptAggregateRoot> deptRepository): IDataSeedContributor, ITransientDependency
{
    private readonly IGuidGenerator _guidGenerator = guidGenerator;
    private readonly ISqlSugarRepository<DeptAggregateRoot> _deptRepository = deptRepository;

    public List<DeptAggregateRoot> GetDataSeed()
    {
        var list = new List<DeptAggregateRoot>();

        var rootId = _guidGenerator.Create();
        var dept1 = new DeptAggregateRoot(rootId)
        {
            DeptName = "总部",
            DeptCode = "000",
            ParentDeptId = Guid.Empty,
            Leader = "CEO",
            OrderNum = 0,
            State = true,
            IsDeleted = false
        };
        list.Add(dept1);

        var dept2 = new DeptAggregateRoot(_guidGenerator.Create(), rootId)
        {
            DeptName = "技术中心",
            DeptCode = "001",
            Leader = "张工",
            OrderNum = 1,
            State = true,
            IsDeleted = false
        };
        list.Add(dept2);

        var dept3 = new DeptAggregateRoot(_guidGenerator.Create(), rootId)
        {
            DeptName = "后端组",
            DeptCode = "0011",
            Leader = "李后端",
            OrderNum = 1,
            State = true,
            IsDeleted = false
        };
        list.Add(dept3);

        var dept4 = new DeptAggregateRoot(_guidGenerator.Create(), rootId)
        {
            DeptName = "前端组",
            DeptCode = "0012",
            Leader = "王前端",
            OrderNum = 2,
            State = true,
            IsDeleted = false
        };
        list.Add(dept4);

        var adminId = _guidGenerator.Create();
        var dept5 = new DeptAggregateRoot(adminId, rootId)
        {
            DeptName = "行政中心",
            DeptCode = "003",
            Leader = "吴行政",
            OrderNum = 3,
            State = true,
            IsDeleted = false
        };
        
        
        var dept6 = new DeptAggregateRoot(_guidGenerator.Create(), adminId)
        {
            DeptName = "人事部",
            DeptCode = "0031",
            Leader = "郑人事",
            OrderNum = 1,
            State = true,
            IsDeleted = false
        };
        list.Add(dept6);

        var dept7 = new DeptAggregateRoot(_guidGenerator.Create(), adminId)
        {
            DeptName = "财务部",
            DeptCode = "0032",
            Leader = "周财务",
            OrderNum = 2,
            State = true,
            IsDeleted = false
        };
        list.Add(dept7);

        var dept8 = new DeptAggregateRoot(_guidGenerator.Create(), rootId)
        {
            DeptName = "运维中心",
            DeptCode = "004",
            Leader = "施运维",
            OrderNum = 4,
            State = true,
            IsDeleted = false
        };
        list.Add(dept8);
        
        return list;
    }
    
    public async Task SeedAsync(DataSeedContext context)
    {
        if(! await _deptRepository.AnyAsync(x => true))
        {
            await _deptRepository.InsertManyAsync(GetDataSeed());
        }
    }
}