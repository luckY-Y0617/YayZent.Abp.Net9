using YayZent.Framework.Auth.Application.Contracts.Enums;

namespace YayZent.Framework.Auth.Application.Contracts.Captcha;

public interface ICaptchaService
{
    Task<byte[]> GenerateImageCaptchaAsync(Guid uuid);

    void ValidateImageCaptcha(string uuid, string code);
    
    Task<string> SendPhoneCaptchaAsync(ValidationPhoneTypeEnum validationPhoneType, string phone);

    Task ValidatePhoneCaptchaAsync(ValidationPhoneTypeEnum validationPhoneType, string phone, string code);

    Task<string> SendEmailCaptchaAsync(ValidationEmailTypeEnum validationEmailType, string email);

    Task ValidateEmailCaptchaAsync(ValidationEmailTypeEnum validationEmailType, string email, string code);
}