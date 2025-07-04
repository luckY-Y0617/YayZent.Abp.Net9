using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using UAParser;
using Volo.Abp.Security.Claims;

namespace YayZent.Framework.AspNetCore.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    /// 获取客户端 IP 地址，优先从 X-Forwarded-For 头读取，若无则使用 RemoteIpAddress，并做常见格式和规则校验。
    /// </summary>
    /// <param name="context">HTTP 上下文，可为 null。</param>
    /// <returns>客户端 IP（不含端口），异常情况返回 "127.0.0.1"。</returns>
    public static string GetClientIp(this HttpContext? context)
    {
        // 1. 上下文为空时直接返回空串
        if (context is null) return string.Empty;

        // 2. 尝试从 X-Forwarded-For 头获取（当应用部署在反向代理/负载均衡后面时常用）
        var result = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        // 3. 如果没有从头拿到，使用连接信息中的 RemoteIpAddress
        if (string.IsNullOrEmpty(result))
        {
            result = context.Connection.RemoteIpAddress?.ToString();
        }

        // 4. 依然为空或为 IPv6 本地回环 (::1) 时，统一使用 IPv4 回环
        if (string.IsNullOrEmpty(result) || result.Contains("::1"))
        {
            result = "127.0.0.1";
        }

        // 5. 去掉 IPv4-mapped IPv6 前缀（如 "::ffff:192.168.0.1"）
        result = result.Replace("::ffff:", string.Empty);

        // 6. 如果带有端口号，则移除末尾的端口部分
        //    匹配格式 ":<1~5 位数字>"
        result = Regex.Replace(result, @":\d{1,5}$", string.Empty);

        // 7. 校验是否符合 IPv4 格式（带或不带端口都可）
        bool isValidIp =
            // 纯 IPv4
            Regex.IsMatch(result, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$")
            // 或 IPv4:端口
            || Regex.IsMatch(result, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?):\d{1,5}$");

        // 8. 最终返回合法 IP，否则回退到本地回环
        return isValidIp ? result : "127.0.0.1";
    }
    
    /// <summary>
    /// 起请求的客户端软件的类型、操作系统、浏览器版本、设备类型等信息
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string? GetUserAgent(this HttpContext context)
    {
        return context.Request.Headers["User-Agent"];
    }

    public static List<string> GetUserPermissions(this HttpContext context, string permissionName)
    {
        return context.User.Claims.Where(x => x.Type == permissionName).Select(x => x.Value).ToList();
    }

    public static string? GetUserName(this HttpContext context)
    {
        return context.User.Claims.Where(x => x.Type == AbpClaimTypes.UserName).Select(x => x.Value).FirstOrDefault();
    }

    public static ClientInfo GetClientInfo(this HttpContext context)
    {
        var str = context.GetUserAgent();
        var uaParser = Parser.GetDefault();
        try
        {
            return uaParser.Parse(str);
        }
        catch
        {
            return new ClientInfo("null",
                new OS("null", "null", "null", "null", "null"),
                new Device("null", "null", "null"),
                new UserAgent("null", "null", "null", "null"));
        }
    }
}