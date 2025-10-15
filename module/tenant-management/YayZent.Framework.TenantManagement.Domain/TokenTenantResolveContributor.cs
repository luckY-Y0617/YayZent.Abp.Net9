using Microsoft.AspNetCore.Http;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.MultiTenancy;

namespace YayZent.Framework.TenantManagement.Domain;

public class TokenTenantResolveContributor: HttpTenantResolveContributorBase
{
    public const string ContributorName = "Token";

    public override string Name => ContributorName;

    protected override Task<string?> GetTenantIdOrNameFromHttpContextOrNullAsync(ITenantResolveContext context, HttpContext httpContext)
    {
        var user = httpContext.User;
        if (user.Identity?.IsAuthenticated == true)
        {
            var tenant = user.FindFirst("tenantid")?.Value;
            return Task.FromResult(tenant);
        }
        return Task.FromResult<string?>(null);
    }
}