using SqlSugar;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.SqlSugarCore;

public class AsyncLocalDbContextAccessor
{
    public static AsyncLocalDbContextAccessor Instance { get; } = new();

    public ISqlSugarDbContext? CurrentDbContext
    {
        get => _currentDbContext.Value;
        set => _currentDbContext.Value = value;
    }

    private readonly AsyncLocal<ISqlSugarDbContext> _currentDbContext = new AsyncLocal<ISqlSugarDbContext>();
}