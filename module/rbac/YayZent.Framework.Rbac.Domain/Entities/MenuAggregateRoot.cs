using SqlSugar;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using YayZent.Framework.Core.Interfaces;
using YayZent.Framework.Rbac.Domain.Shared.Enums;

namespace YayZent.Framework.Rbac.Domain.Entities;

[SugarTable("Menu")]
public class MenuAggregateRoot: AuditedAggregateRoot<Guid>, ISoftDelete,IOrderNum, IState
{
    /// <summary>
    /// 菜单名
    /// </summary>
    public string MenuName { get; set; } = string.Empty;
    
    /// <summary>
    /// 路由名称
    /// </summary>
    public string? RouterName { get; set; }
    
    /// <summary>
    /// 权限编码
    /// </summary>
    public string? PermissionCode { get; set; }
    
    /// <summary>
    /// 父菜单Id
    /// </summary>
    public Guid? ParentMenuId { get; set; }
    
    /// <summary>
    /// 菜单图标
    /// </summary>
    public string? MenuIcon { get; set; }
    
    /// <summary>
    /// 菜单路由组件
    /// </summary>
    public string? Router { get; set; }
    
    /// <summary>
    /// 是否为外部链接
    /// </summary>
    public bool IsLink { get; set; }
    
    /// <summary>
    /// 是否缓存
    /// </summary>
    public bool IsCache {get;set;}
    
    /// <summary>
    /// 是否显示
    /// </summary>
    public bool IsShow { get; set; }
    
    /// <summary>
    /// 描述
    /// </summary>
    public string? Remark { get; set; }
    
    /// <summary>
    /// 组件路径
    /// </summary>
    public string? Component  { get; set; }
    
    /// <summary>
    /// 路由参数
    /// </summary>
    public string? Query  { get; set; }

    /// <summary>
    /// 菜单来源
    /// </summary>
    public MenuSourceEnum MunuSource { get; set; } = MenuSourceEnum.Pure;
    
    /// <summary>
    /// 菜单类型
    /// </summary>
    public MenuTypeEnum MenuType { get; set; } = MenuTypeEnum.Menu;

    /// <summary>
    /// 
    /// </summary>
    public MenuAccessLevelEnum MenuAccessLevel { get; set; } = MenuAccessLevelEnum.Default;
    
    /// <summary>
    /// 逻辑删除
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int OrderNum { get; set; } = 0;
    
    /// <summary>
    /// 状态
    /// </summary>
    public bool State { get; set; }
    
    /// <summary>
    /// 子菜单
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<MenuAggregateRoot> SubMenus { get; } = new List<MenuAggregateRoot>();
    
    public MenuAggregateRoot() {}
    
    public MenuAggregateRoot(Guid id) : base(id) {}
    
    public MenuAggregateRoot(Guid id, Guid parentMenuId) : base(id) {ParentMenuId = parentMenuId;}
}