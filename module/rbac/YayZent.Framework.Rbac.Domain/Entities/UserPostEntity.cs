using SqlSugar;
using Volo.Abp.Domain.Entities;

namespace YayZent.Framework.Rbac.Domain.Entities;

[SugarTable("UserPost")]
public class UserPostEntity: Entity<Guid>
{
    public Guid UserId { get; init; }
    
    public Guid PostId { get; init; }
    
    public UserPostEntity() {}

    public UserPostEntity(Guid userId, Guid postId)
    {
        UserId = userId;
        PostId = postId;
    }
}