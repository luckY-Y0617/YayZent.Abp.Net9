using Volo.Abp;
using Volo.Abp.Application.Services;

namespace YayZent.Abp.Application.Contracts.IServices;

[RemoteService]
public interface ITestService: IApplicationService
{
    Task<string> HelloWorldAsync();
}