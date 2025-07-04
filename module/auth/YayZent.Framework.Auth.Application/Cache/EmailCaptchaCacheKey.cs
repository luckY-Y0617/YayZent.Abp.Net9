using YayZent.Framework.Auth.Application.Contracts.Enums;

namespace YayZent.Framework.Auth.Application.Cache;

public class EmailCaptchaCacheKey(ValidationEmailTypeEnum emailType, string email)
{
    public ValidationEmailTypeEnum EmailType { get; set; } = emailType;
    
    public string Email { get; set; } = email;
    
    public override string ToString()
    {
        return $"EmailCaptcha:{EmailType}:{Email}";
    }
}