using Volo.Abp.Application.Dtos;
using YayZent.Framework.Rbac.Domain.Shared.Enums;

namespace YayZent.Framework.Rbac.Application.Contracts.Dtos.Menu;

public class MenuGetListInputDto: PagedAndSortedResultRequestDto
{
    public bool? State { get; set; }
    public string? MenuName { get; set; }
    public MenuSourceEnum MenuSource { get; set; } = MenuSourceEnum.Ruoyi;
}