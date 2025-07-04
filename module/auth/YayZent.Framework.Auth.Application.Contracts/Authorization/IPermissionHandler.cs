using Volo.Abp.DependencyInjection;

namespace YayZent.Framework.Auth.Application.Contracts.Authorization;

// 权限判断
public interface IPermissionHandler: ITransientDependency
{
    bool IsPass(string permission);
}