namespace YayZent.Framework.Auth.Application.Cache;

public class EmailCaptchaCacheItem(string code)
{
    public string Code { get; set; } = code;
}