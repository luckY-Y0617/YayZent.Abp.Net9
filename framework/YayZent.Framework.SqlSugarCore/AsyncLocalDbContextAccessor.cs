using SqlSugar;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.SqlSugarCore;

public class AsyncLoaclDbContextAccessor
{
    public static AsyncLoaclDbContextAccessor Instance { get; } = new();

    public ISqlSugarDbContext? CurrentDbContext
    {
        get => _currentDbContext.Value;
        set => _currentDbContext.Value = value;
    }

    private readonly AsyncLocal<ISqlSugarDbContext> _currentDbContext = new AsyncLocal<ISqlSugarDbContext>();
}