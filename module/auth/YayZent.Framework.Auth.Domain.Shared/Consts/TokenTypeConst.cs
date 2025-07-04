namespace YayZent.Framework.Auth.Domain.Shared.Consts;

/// <summary>
/// 谁定义这些值的语义，就应该拥有它们,同时防止反向依赖
/// </summary>
public class TokenTypeConst
{
    public const string Id = nameof(Id);

    public const string UserName = nameof(UserName);

    public const string TenantId = nameof(TenantId);

    public const string DeptId = nameof(DeptId);

    public const string Email = nameof(Email);

    public const string PhoneNumber = nameof(PhoneNumber);

    public const string Roles = nameof(Roles);

    public const string Permission = nameof(Permission);

    public const string RoleInfo=nameof(RoleInfo);

    public const string Refresh=nameof(Refresh);
}