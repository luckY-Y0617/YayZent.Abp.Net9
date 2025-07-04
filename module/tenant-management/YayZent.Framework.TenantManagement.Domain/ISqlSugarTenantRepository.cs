using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.TenantManagement.Domain;

public interface ISqlSugarTenantRepository: ISqlSugarRepository<TenantAggregateRoot, Guid>
{
    Task<TenantAggregateRoot> FindByNameAsync(string name, bool includeDetails = true);

    Task<List<TenantAggregateRoot>> GetListAsync(string sorting, int maxResultCount = int.MaxValue, int skipCount = 0, 
        string? filter = null, bool includeDetails = false);

    Task<long> GetCountAsync(string? filter = null);
}