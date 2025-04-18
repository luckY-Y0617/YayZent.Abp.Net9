namespace YayZent.Framework.SqlSugarCore.Abstractions;
public interface ISugarDbContextProvider<TDbContext> where TDbContext : ISqlSugarDbContext
{
    Task<TDbContext> GetDbContextAsync();
}