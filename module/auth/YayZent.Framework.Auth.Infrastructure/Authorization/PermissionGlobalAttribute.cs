using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http;
using YayZent.Framework.Auth.Application.Contracts.Authorization;
using YayZent.Framework.Auth.Domain.Shared.Authorization;

namespace YayZent.Framework.Auth.Infrastructure.Authorization;

public class PermissionGlobalAttribute(IPermissionHandler permissionHandler): ActionFilterAttribute, ITransientDependency
{
    private readonly IPermissionHandler _permissionHandler = permissionHandler;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor) return;
        List<PermissionAttribute> permissionAttributes  = controllerActionDescriptor.MethodInfo
            .GetCustomAttributes<PermissionAttribute>(inherit: true).ToList();

        if (permissionAttributes .Count == 0) return;

        foreach (var permissionAttribute in permissionAttributes)
        {
            if (!_permissionHandler.IsPass(permissionAttribute.Code))
            {
                context.Result = new ObjectResult(new
                {
                    error = new RemoteServiceErrorInfo
                    {
                        Code = "403",
                        Message = "您无权限访问,请联系管理员申请",
                        Details = $"您无权限访问该接口 - {context.HttpContext.Request.Path.Value}"
                    }
                })
                {
                    StatusCode = 403
                };
            }
            return;
        }
    }
}