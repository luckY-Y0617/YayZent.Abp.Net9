namespace YayZent.Framework.Auth.Domain.Shared.Etos;

public class RefreshTokenCreatedEventArgs
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpireTime { get; set; }
}