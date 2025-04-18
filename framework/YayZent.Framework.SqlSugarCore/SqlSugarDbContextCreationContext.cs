using System.Data.Common;
using Volo.Abp;

namespace YayZent.Framework.SqlSugarCore;

public class SqlSugarDbContextCreationContext(string connString, string connStringName)
{
    public static SqlSugarDbContextCreationContext Current => _current.Value;
    
    private static readonly AsyncLocal<SqlSugarDbContextCreationContext> _current = new AsyncLocal<SqlSugarDbContextCreationContext>();
    
    public string ConnString { get; } = connString;
    
    public string ConnStringName { get; } = connStringName;
    
    public DbConnection ExistingConnection { get; internal set;}

    /// <summary>
    /// Binds the SqlSugarDbContextCreationContext to the current thread, allowing the DbContex to access the correct
    /// connection infomartion when it is created;
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static IDisposable Use(SqlSugarDbContextCreationContext context)
    {
        var previousValue = Current;
        _current.Value = context;
        return new DisposeAction(() => _current.Value = previousValue);
    }
}