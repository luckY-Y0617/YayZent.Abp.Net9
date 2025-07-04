using YayZent.Framework.Rbac.Domain.Shared.Enums;

namespace YayZent.Framework.Rbac.Application.Contracts.Dtos;

public class RoleDto
{
    public Guid Id { get;  set; }

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
    /// 角色名
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码 
    ///</summary>
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 描述 
    ///</summary>
    public string? Remark { get; set; }
    /// <summary>
    /// 角色数据范围 
    ///</summary>
    public DataScopeEnum DataScope { get; set; } = DataScopeEnum.All;

    /// <summary>
    /// 状态
    /// </summary>
    public bool State { get; set; } = true;
}