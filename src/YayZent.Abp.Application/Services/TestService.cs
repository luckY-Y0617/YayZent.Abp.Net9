using Volo.Abp;
using Volo.Abp.Application.Services;
using YayZent.Abp.Application.Contracts.IServices;

namespace YayZent.Abp.Application.Services;

[RemoteService]
public class TestService: ApplicationService, ITestService
{
    public async Task<string> HelloWorldAsync()
    {
        return await Task.FromResult("Hello World!");
    }
}