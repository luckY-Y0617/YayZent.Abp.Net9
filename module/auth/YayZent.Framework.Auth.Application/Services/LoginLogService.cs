using IPTools.Core;
using Microsoft.AspNetCore.Http;
using Volo.Abp.DependencyInjection;
using YayZent.Framework.AspNetCore.Extensions;
using YayZent.Framework.Auth.Domain.Entities;


namespace YayZent.Framework.Auth.Application.Services;

public class LoginLogService : ITransientDependency
{
    public async Task<LoginLogAggregateRoot> CreateLoginLogAsyncFromHttpContext(HttpContext httpContext)
    {
        var client = httpContext.GetClientInfo();
        var ipAddr = httpContext.GetClientIp();
        var loginUser = httpContext.GetUserName();

        var location = await GetLocationAsync(ipAddr);

        return new LoginLogAggregateRoot(
            loginUser,
            client.Device.Family,
            os: client.OS.ToString(),
            ipAddr,
            $"{location.Province}-{location.City}"
        );
    }

    private async Task<IpInfo> GetLocationAsync(string ip)
    {
        if (ip == "127.0.0.1" || string.IsNullOrEmpty(ip))
        {
            return new IpInfo { Province = "本地", City = "本地" };
        }

        try
        {
            return await Task.Run(() => IpTool.Search(ip));
        }
        catch
        {
            return new IpInfo { Province = ip, City = "未知" };
        }
    }
}
