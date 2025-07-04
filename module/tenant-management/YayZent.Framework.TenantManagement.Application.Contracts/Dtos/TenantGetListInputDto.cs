using Volo.Abp.Application.Dtos;

namespace YayZent.Framework.TenantManagement.Application.Contracts.Dtos;

public class TenantGetListInputDto: PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 查询开始时间条件
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 查询结束时间条件
    /// </summary>
    public DateTime? EndTime { get; set; }
    
    public  string? Name { get;  set; }
}