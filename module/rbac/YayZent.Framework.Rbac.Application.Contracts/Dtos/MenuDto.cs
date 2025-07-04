using YayZent.Framework.Rbac.Domain.Shared.Enums;

namespace YayZent.Framework.Rbac.Application.Contracts.Dtos;

public class MenuDto
{
    public Guid Id { get;  set; }
    /// <summary>
    /// 菜单来源
    /// </summary>
    public MenuSourceEnum MenuSource { get; set; } 
    public string? RouterName { get; set; }

    /// <summary>
    /// 逻辑删除
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 创建者
    /// </summary>
    public Guid? CreatorId { get; set; }

    /// <summary>
    /// 最后修改者
    /// </summary>
    public Guid? LastModifierId { get; set; }

    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime? LastModificationTime { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int OrderNum { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    public bool State { get; set; }

    /// <summary>
    /// 菜单名
    /// </summary>
    public string MenuName { get; set; } = string.Empty;
    /// <summary>
    ///  
    ///</summary>
    public MenuTypeEnum MenuType { get; set; } = MenuTypeEnum.Menu;
    /// <summary>
    ///  
    ///</summary>
    public string? PermissionCode { get; set; }
    /// <summary>
    ///  
    ///</summary>

    public Guid ParentId { get; set; }

    /// <summary>
    /// 菜单图标 
    ///</summary>

    public string? MenuIcon { get; set; }
    /// <summary>
    /// 菜单组件路由 
    ///</summary>

    public string? Router { get; set; }
    /// <summary>
    /// 是否为外部链接 
    ///</summary>

    public bool IsLink { get; set; }
    /// <summary>
    /// 是否缓存 
    ///</summary>

    public bool IsCache { get; set; }
    /// <summary>
    /// 是否显示 
    ///</summary>
    public bool IsShow { get; set; } = true;

    /// <summary>
    /// 描述 
    ///</summary>
    public string? Remark { get; set; }
    /// <summary>
    /// 组件路径 
    ///</summary>
    public string? Component { get; set; }
    /// <summary>
    /// 路由参数 
    ///</summary>
    public string? Query { get; set; }
}