namespace YayZent.Framework.Auth.Application.Contracts.Dtos.Auth;

public class LoginOutputDto
{
    public string Token { get; set; }
    
    public string UserName { get; set; }
    
    public Guid UserId { get; set; }
}