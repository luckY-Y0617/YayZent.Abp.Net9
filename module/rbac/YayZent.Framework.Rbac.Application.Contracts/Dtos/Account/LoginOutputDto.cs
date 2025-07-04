namespace YayZent.Framework.Rbac.Application.Contracts.Dtos.Account;

public class LoginOutputDto
{
    public string? Token { get; set; }
    
    public string? UserName { get; set; }
}