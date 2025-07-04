namespace YayZent.Framework.SqlSugarCore.Abstractions;

public interface ICurrentDbContextAccessor
{
    BasicDbContextInfo? Current { get; set; }
}