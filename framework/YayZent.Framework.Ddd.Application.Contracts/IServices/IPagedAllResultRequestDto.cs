using Volo.Abp.Application.Dtos;

namespace YayZent.Framework.Ddd.Application.Contracts.IServices;

public interface IPagedAllResultRequestDto : IPageTimeResultRequestDto, IPagedAndSortedResultRequest
{
}