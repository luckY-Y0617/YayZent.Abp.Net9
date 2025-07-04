using Lazy.Captcha.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using YayZent.Framework.Auth.Application.Cache;
using YayZent.Framework.Auth.Application.Contracts.Captcha;
using YayZent.Framework.Auth.Application.Contracts.Enums;
using YayZent.Framework.Core.Email;
using YayZent.Framework.Core.Sms;
using YayZent.Framework.Rbac.Domain.Shared.Options;

namespace YayZent.Framework.Auth.Application.Services;

public class CaptchaService: ICaptchaService, ITransientDependency
{
    private readonly ICaptcha _captcha;
    private readonly RbacOptions _rbacOptions;
    private readonly ISmsSender _smsSender;
    private readonly IEmailSender _emailSender;
    private readonly IDistributedCache<CaptchaPhoneCacheItem, CaptchaPhoneCacheKey> _phoneCache;
    private readonly IDistributedCache<EmailCaptchaCacheItem, EmailCaptchaCacheKey> _emailCache;

    public CaptchaService(IOptions<RbacOptions> rbacOptions, ICaptcha captcha, ISmsSender smsSender, IEmailSender emailSender,
        IDistributedCache<CaptchaPhoneCacheItem, CaptchaPhoneCacheKey> phoneCache, IDistributedCache<EmailCaptchaCacheItem, EmailCaptchaCacheKey> emailCache)
    {
        _rbacOptions = rbacOptions.Value;
        _captcha = captcha;
        _smsSender = smsSender;
        _emailSender = emailSender;
        _phoneCache = phoneCache;
        _emailCache = emailCache;
    }

    public async Task<byte[]> GenerateImageCaptchaAsync(Guid uuid)
    {
        var captcha = _captcha.Generate(uuid.ToString());
        return await Task.FromResult(captcha.Bytes);
    }
    
    public void ValidateImageCaptcha(string uuid, string code)
    {
        if (_rbacOptions.EnableCaptcha == true)
        {
            if (!_captcha.Validate(uuid, code))
            {
                throw new UserFriendlyException("验证码错误");
            }
        }
    }

    public async Task<string> SendPhoneCaptchaAsync(ValidationPhoneTypeEnum validationPhoneType, string phone)
    {
        var cacheKey = new CaptchaPhoneCacheKey(validationPhoneType, phone);
        var cachedItem = await _phoneCache.GetAsync(cacheKey);

        if (cachedItem != null)
        {
            throw new UserFriendlyException($"{phone}已发送过验证码，10分钟后可重试");
        }

        // 生成验证码（4位数字）
        var code = Guid.NewGuid().ToString("N").Substring(0, 4);
        
        // 发送短信
        await _smsSender.SendAsync(phone, code);

        // 缓存验证码，有效期10分钟
        await _phoneCache.SetAsync(
            cacheKey,
            new CaptchaPhoneCacheItem(code),
            new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) });

        return code;
    }
    
    public async Task ValidatePhoneCaptchaAsync(ValidationPhoneTypeEnum validationPhoneType, string phone, string code)
    {
        var captchaPhoneCacheKey = new CaptchaPhoneCacheKey(validationPhoneType, phone);
        var item = await _phoneCache.GetAsync(captchaPhoneCacheKey);
        if (item is not null && item.Code == code)
        {
            await _phoneCache.RemoveAsync(captchaPhoneCacheKey);
            return;
        }

        throw new UserFriendlyException("验证码错误");
    }

    public async Task<string> SendEmailCaptchaAsync(ValidationEmailTypeEnum validationEmailType, string email)
    {
        var cahceKey = new EmailCaptchaCacheKey(validationEmailType, email);
        var cacheItem = await _emailCache.GetAsync(cahceKey);

        if (cacheItem is not null)
        {
            throw new UserFriendlyException($"{email}已发送过验证码，10分钟后可重试");
        }
        
        var code = Guid.NewGuid().ToString("N").Substring(0, 4);
        
        await _emailSender.SendAsync(email, "您的验证码", $"<p>您好，您的验证码是：<strong>{code}</strong>，5分钟内有效，请勿泄露。</p>");
        
        await _emailCache.SetAsync(
            cahceKey,
            new EmailCaptchaCacheItem(code),
            new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) });

        return code;
    }

    public async Task ValidateEmailCaptchaAsync(ValidationEmailTypeEnum validationEmailType, string email, string code)
    {
        var emailCacheKey = new EmailCaptchaCacheKey(validationEmailType, email);
        var cacheItem = await _emailCache.GetAsync(emailCacheKey);
        if (cacheItem is not null && cacheItem.Code == code)
        {
            await _emailCache.RemoveAsync(emailCacheKey);
            return;
        }

        throw new UserFriendlyException("验证码错误");
    }
}