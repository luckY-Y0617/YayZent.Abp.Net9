using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Volo.Abp.EventBus.Local;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using YayZent.Framework.Auth.Application.Contracts.Token;
using YayZent.Framework.Auth.Domain.Shared.Consts;
using YayZent.Framework.Auth.Domain.Shared.Etos;
using YayZent.Framework.Core.Options;

namespace YayZent.Framework.Auth.Infrastructure.Token;

public class TokenProvider(IOptions<JwtOptions> jwtOptions, IOptions<RefreshJwtOptions> refreshJwtOptions,
    ILocalEventBus localEventBus, ICurrentTenant currentTenant): ITokenProvider
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly RefreshJwtOptions _refreshJwtOptions = refreshJwtOptions.Value;
    private readonly ILocalEventBus _localEventBus = localEventBus;
    private readonly ICurrentTenant _currentTenant = currentTenant;

    public async Task<string> CreateAccessTokenAsync(List<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecurityKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddSeconds(_jwtOptions.Expiration),
            notBefore: DateTime.Now,
            signingCredentials: creds
        );
        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return await Task.FromResult(tokenString);
    }

    public async Task<string> CreateRefreshTokenAsync(Guid userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refreshJwtOptions.SecurityKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expireTime = DateTime.Now.AddSeconds(_refreshJwtOptions.Expiration);
        var claims = new List<Claim>
        {
            new Claim(AbpClaimTypes.UserId, userId.ToString()),
            new Claim(TokenTypeConst.Refresh, "true")
        };
        
        var token = new JwtSecurityToken(
            issuer: _refreshJwtOptions.Issuer,
            audience: _refreshJwtOptions.Audience,
            claims: claims,
            expires: expireTime,
            notBefore: DateTime.Now,
            signingCredentials: creds
        );
        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        await _localEventBus.PublishAsync(new RefreshTokenCreatedEventArgs() { UserId = userId, Token = tokenString , ExpireTime = expireTime });
        return await Task.FromResult(tokenString);
    }
}