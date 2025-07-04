using Microsoft.AspNetCore.Http;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;
using YayZent.Framework.AspNetCore.Extensions;
using YayZent.Framework.Auth.Application.Contracts.Authorization;
using YayZent.Framework.Auth.Domain.Shared.Consts;

namespace YayZent.Framework.Auth.Infrastructure.Authorization;

public class DefaultPermissionHandler(ICurrentUser currentUser, IHttpContextAccessor httpContextAccessor): IPermissionHandler
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    
    public bool IsPass(string permission)
    {
        var permissions = _httpContextAccessor.HttpContext?.GetUserPermissions(TokenTypeConst.Permission);
        return permissions?.Contains("*:*:*") == true || permissions?.Contains(permission) == true;
    }
}