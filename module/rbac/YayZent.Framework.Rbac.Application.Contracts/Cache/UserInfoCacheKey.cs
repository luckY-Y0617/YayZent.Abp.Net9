namespace YayZent.Framework.Rbac.Application.Contracts.Cache;

public class UserInfoCacheKey(Guid userId)
{
    public Guid UserId { get; set; } = userId;

    public override string ToString()
    {
        return $"UserId: {UserId}";
    }
}