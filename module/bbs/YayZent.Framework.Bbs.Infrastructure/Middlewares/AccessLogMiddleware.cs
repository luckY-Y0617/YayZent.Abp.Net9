using Microsoft.AspNetCore.Http;
using Volo.Abp.DependencyInjection;
using YayZent.Framework.Bbs.Application.Contracts.IServices;

namespace YayZent.Framework.Bbs.Infrastructure.MiddleWares;

public class AccessLogMiddleware : IMiddleware, ITransientDependency
{
    private readonly IAccessLogCounter _counter;

    public AccessLogMiddleware(IAccessLogCounter counter)
    {
        _counter = counter;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);
        _counter.Increment();
    }
}

