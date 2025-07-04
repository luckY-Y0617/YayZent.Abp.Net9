using Microsoft.AspNetCore.Builder;

namespace YayZent.Framework.Bbs.Infrastructure.MiddleWares;

public static class AccessLogExtensions
{
    public static IApplicationBuilder UseAccessLog(this IApplicationBuilder app)
    {
        app.UseMiddleware<AccessLogMiddleware>();
        return app;
    }
}