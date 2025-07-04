namespace YayZent.Framework.Rbac.Domain.Shared.Consts;

public class UserConst
{
    public const string LoginError = "登录失败！用户名或密码错误！";
    public const string LoginUserNoExist = "登录失败！用户名不存在！";
    public const string LoginPassworldError = "密码为空，添加失败！";
    public const string CreatePassworldError = "密码格式错误，长度需大于等于6位";
    public const string Exist = "用户已经存在，创建失败！";
    public const string StateIsState = "该用户已被禁用，请联系管理员进行恢复";
    public const string NoPermission = "登录禁用！该用户分配无任何权限，无意义登录！";
    public const string NoRole = "登录禁用！该用户分配无任何角色，无意义登录！";
    public const string NameNotAllowed = "用户名被禁止";
    public const string PhoneRepeat = "手机号已重复";
    public const string EmailRepeat = "手机号已重复";
    public const string ResetPasswordError = "密码错误";

    //子租户管理员
    public const string Admin = "admin";
    public const string AdminRoleCode = "admin";
    public const string AdminPermissionCode = "*:*:*";

    //租户管理员
    public const string TenantAdmin = "tenantAdmin";
    public const string TenantAdminPermissionCode = "*";

    public const string DefaultRoleCode = "default";
    public const string CommonRoleName = "common";
}