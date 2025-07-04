using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.SqlSugarCore;

public class AsyncLocalCurrentDbContextAccessor: ICurrentDbContextAccessor
{
    private readonly AsyncLocal<BasicDbContextInfo?> _current = new AsyncLocal<BasicDbContextInfo?>();

    public BasicDbContextInfo? Current
    {
        get => _current.Value;
        set => _current.Value = value;
    }
}