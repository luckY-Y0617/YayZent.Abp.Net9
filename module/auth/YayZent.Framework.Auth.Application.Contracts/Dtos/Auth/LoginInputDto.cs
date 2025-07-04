namespace YayZent.Framework.Auth.Application.Contracts.Dtos.Auth;

public class LoginInputDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string Uuid { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;
}