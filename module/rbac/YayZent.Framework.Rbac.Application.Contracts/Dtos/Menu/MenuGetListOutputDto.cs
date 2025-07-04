using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using YayZent.Framework.Rbac.Domain.Shared.Enums;

namespace YayZent.Framework.Rbac.Application.Contracts.Dtos.Menu;

public class MenuGetListOutputDto: EntityDto<Guid>
{
    public DateTime CreationTime { get; set; }
    
    public Guid CreatorId { get; set; }
    
    public bool State {get; set; }
    
    public string MenuName { get; set; }
    
    public MenuTypeEnum MenuType { get; set; }
    
    public string? PermissionCode { get; set; }
    
    public Guid ParentId { get; set; }
    
    public string? Router { get; set; }
    
    public bool IsLink { get; set; }
    
    public bool IsCache { get; set; }
    
    public bool IsShow { get; set; }
    
    public string? Remark { get; set; }
    
    public string? Component  { get; set; }
    
    public string? Query  { get; set; }
    
    public string? RouterName { get; set; }
    
    public int OrderNum { get; set; }
}