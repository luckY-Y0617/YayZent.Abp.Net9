using System.Security.Claims;

namespace YayZent.Framework.Auth.Application.Contracts.Dtos.Auth;

/// <summary>
/// 跨模块传递声明信息
/// </summary>
public class UserClaimsContext
{
    public List<Claim> Claims { get; set; }
}