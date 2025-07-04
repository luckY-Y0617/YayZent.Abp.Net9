using SqlSugar;
using Volo.Abp.Domain.Entities.Auditing;
using YayZent.Framework.Core.Helper;

namespace YayZent.Framework.Auth.Domain.Entities;

[SugarTable("RefreshToken")]
public class RefreshTokenAggregateRoot: AuditedAggregateRoot<Guid>
{
    public Guid UserId { get; set; }
    
    [SugarColumn(ColumnDescription = "RefreshToken", Length = 88)]
    public string Token { get; set; } = string.Empty;
    
    public DateTime ExpiresAt { get; set; }

    public bool IsActive { get; set; } = false;
    
    public string? Device { get; set; }
    
    public RefreshTokenAggregateRoot() {}

    public RefreshTokenAggregateRoot(Guid id, Guid userId, string token, DateTime expiresAt):base(id)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty", nameof(userId));
        }
        
        UserId = userId;
        Token = HashHelper.ComputeSha256Hash(token, HashHelper.HashEncoding.Hex);
        ExpiresAt = expiresAt;
    }

    public bool VerifyToken(string token)
    {
        return HashHelper.ComputeSha256Hash(token, HashHelper.HashEncoding.Hex).Equals(Token);
    }

    public void Activate()
    {
        IsActive = true;
    }
}