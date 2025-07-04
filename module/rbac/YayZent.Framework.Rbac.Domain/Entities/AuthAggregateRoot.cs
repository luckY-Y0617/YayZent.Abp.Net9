using SqlSugar;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Auditing;

namespace YayZent.Framework.Rbac.Domain.Entities;

[SugarTable("Auth")]
public class AuthAggregateRoot: AuditedAggregateRoot<Guid>, ISoftDelete, IHasCreationTime
{
    /// <summary>
    /// 获取或设置用户 ID，标识该认证记录所关联的用户。
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 获取或设置 OpenId，用于第三方认证时与外部系统（如微信、QQ等）进行身份匹配的标识。
    /// </summary>
    public string? OpenId { get; set; }

    /// <summary>
    /// 获取或设置认证用户的名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置认证类型，如 "微信"、"QQ"、"邮箱" 等，表示用户身份验证的来源。
    /// </summary>
    public string AuthType { get; set; }

    /// <summary>
    /// 获取或设置该认证记录是否被标记为删除。
    /// 该字段实现了软删除功能，表示记录在逻辑上已被删除。
    /// </summary>
    public bool IsDeleted { get; }

    /// <summary>
    /// 获取附加的扩展属性字典，可以用来存储额外的数据。
    /// </summary>
    public override ExtraPropertyDictionary ExtraProperties { get; protected set; }
    
    public AuthAggregateRoot(){}

    public AuthAggregateRoot(string authType, Guid userId, string openId)
    {
        AuthType = authType;
        UserId = userId;
        OpenId = openId;
    }
    
    public AuthAggregateRoot(string authType, Guid userId, string openId, string name) : this(authType, userId, openId)
    {
        Name = name;
    }
}