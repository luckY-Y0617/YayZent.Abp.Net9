namespace YayZent.Framework.Rbac.Domain.Shared.Enums;

public enum MenuTypeEnum
{
    // 目录 是用来组织菜单的分组容器，本身不跳页面；
    // 菜单 是可以跳转到具体页面的路由项。
    Catalog = 0,  //目录
    Menu = 1,  //菜单
    Component = 2//组件
}