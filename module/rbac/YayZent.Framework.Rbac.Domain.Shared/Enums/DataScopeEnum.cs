namespace YayZent.Framework.Rbac.Domain.Shared.Enums;

public enum DataScopeEnum
{
    //所有数据权限
    All = 0,
    //自定义数据权限
    Custom = 1,
    //本部门数据权限
    Dept = 2,
    //本部门及以下部门
    DeptFollow = 3,
    //仅本人数据权限
    User = 4
}