namespace YayZent.Framework.Auth.Application.Contracts.Dtos.Auth;

public class EmailCaptchaForRegisterInputDto
{
    public string Email { get; set; } = string.Empty;
        
    public string Uuid { get; set; }

    public string Code { get; set; }
}