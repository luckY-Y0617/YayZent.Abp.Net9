using SqlSugar;

namespace YayZent.Framework.SqlSugarCore.Abstractions;

public class DbConnOptions
{
    public string? Url { get; set; }
    
    public DbType? DbType { get; set; }

    public bool EnableDbSeed { get; set; } = false;
    
    public bool EnableUnderLine { get; set; } = false;
    
    public bool EnableCodeFirst { get; set; } = false;

    public bool EnableSqlLog { get; set; } = true;

    public List<string>? EntityAssemblies { get; set; }
    
    public bool EnbaleReadWriteSplitting { get; set; }
    
    public List<string>? ReadWriteSplittingUrl { get; set; }
    
    public bool EnabledSaasMultiTenancy { get; set; } = false;
    
}