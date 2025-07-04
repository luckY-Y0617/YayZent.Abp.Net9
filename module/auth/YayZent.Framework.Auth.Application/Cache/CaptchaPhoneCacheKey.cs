using YayZent.Framework.Auth.Application.Contracts.Enums;

namespace YayZent.Framework.Auth.Application.Cache;

public class CaptchaPhoneCacheKey(ValidationPhoneTypeEnum phoneType, string phone)
{
    public ValidationPhoneTypeEnum ValidationPhoneType { get; set; } = phoneType;

    public string Phone { get; set; } = phone;

    // ABP 框架的 IDistributedCache<T> 接口，它其实是通过 ToString() 方法来生成 Key 的
    public override string ToString()
    {
        return $"Phone: {ValidationPhoneType.ToString()}, Phone: {Phone}";
    }
}