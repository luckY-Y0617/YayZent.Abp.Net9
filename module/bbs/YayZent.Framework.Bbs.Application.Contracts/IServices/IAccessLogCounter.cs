namespace YayZent.Framework.Bbs.Application.Contracts.IServices;

public interface IAccessLogCounter
{
    void Increment();
    Task<int> GetAndResetCountAsync();
    Task ResetAsync();
}