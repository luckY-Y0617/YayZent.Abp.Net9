using Volo.Abp.DependencyInjection;
using YayZent.Framework.Rbac.Application.Contracts.Dtos;

namespace YayZent.Framework.Rbac.Application.Contracts.IService.User;

public interface IUserInternalService: ITransientDependency
{
    Task<UserRoleMenuDto> GetUserInfoAsync(Guid userId);
    Task<List<UserRoleMenuDto>> GetInfoListAsync(List<Guid> userIds);
    
    Task<string> RetrivePassword(string phone, string oldPassword, string newPassword);
    
    Task<UserDto> GetUserAsync(Guid userId);
    
    Task<UserDto> ValidateAndGetUserAsync(string userName, string password);
    
    Task<bool> IsEmailRegisteredAsync(string email);
    
    Task<bool> IsPhoneNumberRegisteredAsync(string phone);
    
    Task InsertNewUserByPhoneAsync(string userName, string password, string phone);
    
    Task InsertNewUserByEmailAsync(string userName, string password, string email);
}