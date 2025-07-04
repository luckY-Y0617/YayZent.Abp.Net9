namespace YayZent.Framework.Auth.Domain.Shared.Authorization;

public class PermissionAttribute(string code): Attribute
{
    public string Code { get; set; } = code;

}