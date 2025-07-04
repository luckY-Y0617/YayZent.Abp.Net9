using FluentValidation;

namespace YayZent.Framework.Auth.Application.Contracts.Dtos.Auth;

public class PhoneCaptchaForRegisterInputDto
{
    public string Phone { get; set; } = string.Empty;
        
    public string Uuid { get; set; }

    public string Code { get; set; }
}

public class PhoneCaptchaForRegisterRequestValidator : AbstractValidator<PhoneCaptchaForRegisterInputDto>
{
    public PhoneCaptchaForRegisterRequestValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("手机号不能为空")
            .Matches(@"^\d{11}$").WithMessage("手机号码格式错误！请检查");
    }
}