using Volo.Abp.DependencyInjection;

namespace YayZent.Framework.Core.Email;

public interface IEmailSender: ITransientDependency
{
    public Task SendAsync(string to, string subject, string body);
}