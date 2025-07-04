namespace YayZent.Framework.Rbac.Domain.Shared.Enums;

public enum MenuAccessLevelEnum
{
    /// <summary>
    /// 所有人都可以访问（不需要权限或公开页面）
    /// </summary>
    Public = 0,

    /// <summary>
    /// 默认角色可访问
    /// </summary>
    Default = 1,

    /// <summary>
    /// 仅管理员角色可访问
    /// </summary>
    AdminOnly = 2
}