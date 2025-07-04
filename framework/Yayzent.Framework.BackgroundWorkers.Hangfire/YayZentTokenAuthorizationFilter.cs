using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace Yayzent.Framework.BackgroundWorkers.Hangfire;

public class YayZentTokenAuthorizationFilter (IServiceProvider serviceProvider): IDashboardAsyncAuthorizationFilter, ITransientDependency
{
    private const string BearerPrefix = "Bearer ";
    private const string TokenCookieKey = "Token";
    private const string HtmlContentType = "text/html";

    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private string _requiredUsername = "cc";
    private TimeSpan _tokenExpiration = TimeSpan.FromMinutes(10);


    public YayZentTokenAuthorizationFilter SetRequiredUsername(string username)
    {
        _requiredUsername = username ?? throw new ArgumentNullException(nameof(username));
        return this;
    }

    public YayZentTokenAuthorizationFilter SetTokenExpiration(TimeSpan expiration)
    {
        _tokenExpiration = expiration;
        return this;
    }

    public bool Authorize(DashboardContext context)
    {
        return true;
    }

    public Task<bool> AuthorizeAsync(DashboardContext context)
    {
        return Task.FromResult(Authorize(context));
    }

    /// <summary>
    /// 判断权限
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private bool IsAuthorizedUser(ICurrentUser user)
    {
        return user.UserName == _requiredUsername;
    }

    private bool TryExtractBearerToken(HttpContext httpContext, out string? token)
    {
        token = null;
        var authHeader = httpContext.Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith(BearerPrefix))
        {
            token = authHeader[BearerPrefix.Length..].Trim();
            return true;
        }
        return false;
    }

    private void SetTokenCookie(HttpContext httpContext, string token)
    {
        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.Now.Add(_tokenExpiration),
            HttpOnly = true,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Strict
        };
        httpContext.Response.Cookies.Append(TokenCookieKey, token, cookieOptions);
    }

    private void SetChallengeResponse(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = 401;
        httpContext.Response.ContentType = HtmlContentType;

        var html = @"
        <html>
        <head>
            <title>Hangfire Dashboard Authorization</title>
            <style>
                body { font-family: Arial, sans-serif; margin: 40px; }
                .container { max-width: 400px; margin: 0 auto; }
                .form-group { margin-bottom: 15px; }
                input[type='text'] { width: 100%; padding: 8px; }
                button { background: #337ab7; color: white; border: none; padding: 10px 15px; cursor: pointer; }
                button:hover { background: #286090; }
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Authorization Required</h2>
                <div class='form-group'>
                    <input type='text' id='token' placeholder='Enter your Bearer token...' />
                </div>
                <button onclick='authorize()'>Authorize</button>
            </div>
            <script>
                function authorize() {
                    var token = document.getElementById('token').value;
                    if (token.length < 10) {
                        alert('Invalid token');
                        return;
                    }
                    document.cookie = 'Token=' + encodeURIComponent(token) + '; path=/';
                    window.location.reload();
                }
            </script>
        </body>
        </html>";

        httpContext.Response.WriteAsync(html);
    }
}
