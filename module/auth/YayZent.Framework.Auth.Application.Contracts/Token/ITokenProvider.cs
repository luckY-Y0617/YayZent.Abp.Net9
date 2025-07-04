using System.Security.Claims;
using Volo.Abp.DependencyInjection;

namespace YayZent.Framework.Auth.Application.Contracts.Token;

/// <summary>
/// 创建 access/refresh token
/// </summary>
public interface ITokenProvider: ITransientDependency
{
    Task<string> CreateAccessTokenAsync(List<Claim> claims);
    Task<string> CreateRefreshTokenAsync(Guid userId);
}