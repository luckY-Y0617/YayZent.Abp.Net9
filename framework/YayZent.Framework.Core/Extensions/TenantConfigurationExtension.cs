using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using SqlSugar;
using Volo.Abp;

namespace YayZent.Framework.Core.Extensions;

public static class TenantConfigurationExtension
{
    /// <summary>
    /// 获取当前连接字符串
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentConnectionString(this TenantConfiguration tenantConfiguration)
    {
        if (tenantConfiguration.ConnectionStrings == null || string.IsNullOrWhiteSpace(tenantConfiguration.ConnectionStrings.Default))
        {
            throw new ArgumentException("租户配置缺少默认连接字符串。");
        }

        return tenantConfiguration.ConnectionStrings.Default;
    }
    
    /// <summary>
    /// 获取当前连接名
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentConnectionName(this TenantConfiguration tenantConfiguration)
    {
        return  tenantConfiguration.Name;
    }

    public static DbType GetCurrentDbType(this TenantConfiguration tenantConfiguration)
    {
        var name = tenantConfiguration.Name;
        
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("tenant name is empty");
        }

        var atIndex = name.LastIndexOf('@');
        if (atIndex == -1 || atIndex == name.Length - 1)
        {
            throw new ArgumentException("tenant name is invalid");
        }

        var dbTypeString = name[(atIndex + 1)..];
        return Enum.TryParse<DbType>(dbTypeString, out var dbType) 
            ? dbType 
            : throw new ArgumentException($"不支持的数据库类型: {dbTypeString}");
    }

    public static string GetNormalizedName(this string name)
    {
        return name.Contains("@") 
            ? name.Substring(0, name.LastIndexOf('@'))
            : name;
    }
}