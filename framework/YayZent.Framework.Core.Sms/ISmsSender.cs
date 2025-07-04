using Volo.Abp.DependencyInjection;

namespace YayZent.Framework.Core.Sms;

public interface ISmsSender: ITransientDependency
{
    Task SendAsync(string phoneNumber, string code);
}