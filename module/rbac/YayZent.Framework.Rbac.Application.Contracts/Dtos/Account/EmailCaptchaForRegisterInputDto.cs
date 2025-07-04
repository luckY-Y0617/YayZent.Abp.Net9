namespace YayZent.Framework.Rbac.Application.Contracts.Dtos.Account;

public class EmailCaptchaForRegisterInputDto
{
    public string Email { get; set; } = string.Empty;
        
    public string Uuid { get; set; }

    public string Code { get; set; }
}