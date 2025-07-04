using YayZent.Framework.SqlSugarCore.Abstractions;
using YayZent.Framework.SqlSugarCore.Repositories;
using YayZent.Framework.TenantManagement.Domain;

namespace YayZent.Framework.TenantManagement.SqlSugarCore;

public class SqlSugarTenantRepository(ISugarDbContextProvider<ISqlSugarDbContext> sugarDbContextProvider) : SqlSugarRepository<TenantAggregateRoot, Guid>(sugarDbContextProvider), ISqlSugarTenantRepository
{
    public async Task<TenantAggregateRoot> FindByNameAsync(string name, bool includeDetails = true)
    {
        return await DbQueryable.FirstAsync(x => x.Name == name);
    }

    public async Task<List<TenantAggregateRoot>> GetListAsync(string sorting = null, int maxResultCount = Int32.MaxValue, int skipCount = 0, string? filter = null,
        bool includeDetails = false)
    {
        return await DbQueryable.WhereIF(!string.IsNullOrEmpty(filter), x => x.Name.Contains(filter))
            .OrderByIF(!string.IsNullOrEmpty(sorting), sorting)
            .ToPageListAsync(skipCount, maxResultCount);
    }

    public async Task<long> GetCountAsync(string? filter = null)
    {
        return await DbQueryable.WhereIF(!string.IsNullOrEmpty(filter),x=>x.Name.Contains(filter)) .CountAsync();
    }
}