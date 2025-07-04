namespace YayZent.Framework.Rbac.Application.Contracts.Dtos.Account;

public class LoginInputDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string Uuid { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;
}