namespace YayZent.Framework.Auth.Application.Cache;

public class CaptchaPhoneCacheItem(string code)
{
    public string Code { get; set; } = code;
}