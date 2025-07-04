using YayZent.Framework.Rbac.Application.Contracts.Dtos.Role;

namespace YayZent.Framework.Rbac.Application.Contracts.Dtos;

public class UserRoleMenuDto
{
    public UserDto User { get; set; } = new();              // 用户信息
    public HashSet<RoleDto> Roles { get; set; } = new();     // 用户的角色集合
    public HashSet<MenuDto> Menus { get; set; } = new();     // 用户的菜单集合
    public HashSet<string> RoleCodes { get; set; } = new();   // 用户的角色编码集合
    public HashSet<string> PermissionCodes { get; set; } = new(); // 用户的权限编码集合
}