using Volo.Abp.Application.Dtos;

namespace YayZent.Framework.Ddd.Application.Contracts.IServices;

public interface IPageTimeResultRequestDto : IPagedAndSortedResultRequest
{
    DateTime? StartTime { get; set; }
    DateTime? EndTime { get; set; }
}