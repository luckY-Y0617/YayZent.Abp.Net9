using YayZent.Framework.AuditLogging.Domain.Entities;

namespace YayZent.Framework.AuditLogging.Domain;

public class EntityChangeWithUsername
{
    public EntityChangeEntity EntityChange { get; set; }

    public string UserName { get; set; }
}