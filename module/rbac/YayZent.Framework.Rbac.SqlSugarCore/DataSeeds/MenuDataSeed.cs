using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using YayZent.Framework.Rbac.Domain.Entities;
using YayZent.Framework.Rbac.Domain.Shared.Enums;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Rbac.SqlSugarCore.DataSeeds;

public class MenuDataSeed(IGuidGenerator guidGenerator, ISqlSugarRepository<MenuAggregateRoot> menuRepository): IDataSeedContributor, ITransientDependency
{
    private readonly IGuidGenerator _guidGenerator = guidGenerator;
    private readonly ISqlSugarRepository<MenuAggregateRoot> _menuRepository = menuRepository;

    public List<MenuAggregateRoot> GetDataSeed()
    {
        var list = new List<MenuAggregateRoot>();
        
        // 顶级菜单
        var systemMenu = new MenuAggregateRoot(Guid.NewGuid())
        {
            MenuName = "系统管理",
            RouterName = "system",
            Router = "/system",
            Component = "Layout",
            PermissionCode = "",
            ParentMenuId = null,
            MenuIcon = "setting",
            IsLink = false,
            IsCache = false,
            IsShow = true,
            Remark = "系统管理根菜单",
            MenuType = MenuTypeEnum.Catalog,
            MunuSource = MenuSourceEnum.Pure,
            OrderNum = 0,
            State = true
        };
        list.Add(systemMenu);
        
        // 子菜单：用户管理
        var userMenu = new MenuAggregateRoot(Guid.NewGuid(), systemMenu.Id)
        {
            MenuName = "用户管理",
            RouterName = "user",
            Router = "user/index",
            Component = "system/user/index",
            PermissionCode = "system:user:list",
            MenuIcon = "user",
            IsLink = false,
            IsCache = true,
            IsShow = true,
            MenuType = MenuTypeEnum.Menu,
            MunuSource = MenuSourceEnum.Pure,
            MenuAccessLevel = MenuAccessLevelEnum.Default,
            OrderNum = 1,
            State = true
        };
        list.Add(userMenu);
        
        // 子菜单：角色管理
        var roleMenu = new MenuAggregateRoot(Guid.NewGuid(), systemMenu.Id)
        {
            MenuName = "角色管理",
            RouterName = "role",
            Router = "role/index",
            Component = "system/role/index",
            PermissionCode = "system:role:list",
            MenuIcon = "peoples",
            IsLink = false,
            IsCache = true,
            IsShow = true,
            MenuType = MenuTypeEnum.Menu,
            MunuSource = MenuSourceEnum.Pure,
            MenuAccessLevel = MenuAccessLevelEnum.Default,
            OrderNum = 2,
            State = true
        };
        list.Add(roleMenu);
        
        var blogMenu = new MenuAggregateRoot(_guidGenerator.Create())
        {
            MenuName = "博客管理",
            RouterName = "blog",
            Router = "/blog",
            Component = "Layout",
            PermissionCode = "",
            ParentMenuId = null,
            MenuIcon = "document",
            IsLink = false,
            IsCache = false,
            IsShow = true,
            Remark = "博客管理目录",
            MenuType = MenuTypeEnum.Catalog,
            MunuSource = MenuSourceEnum.Pure,
            OrderNum = 10,
            State = true
        };
        list.Add(blogMenu);
        
        var blogAddMenu = new MenuAggregateRoot(_guidGenerator.Create(), blogMenu.Id)
        {
            MenuName = "新增博客",
            RouterName = "blog_add",
            Router = "add",
            Component = "blog/add/index",
            PermissionCode = "blog:add", // 🔑 关键权限码
            MenuIcon = "plus-circle",
            IsLink = false,
            IsCache = false,
            IsShow = true,
            MenuType = MenuTypeEnum.Menu,
            MunuSource = MenuSourceEnum.Pure,
            MenuAccessLevel = MenuAccessLevelEnum.AdminOnly,
            OrderNum = 1,
            State = true
        };
        list.Add(blogAddMenu);
        return list;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if(! await _menuRepository.AnyAsync(x => true))
        {
            await _menuRepository.InsertManyAsync(GetDataSeed());
        }
    }
}