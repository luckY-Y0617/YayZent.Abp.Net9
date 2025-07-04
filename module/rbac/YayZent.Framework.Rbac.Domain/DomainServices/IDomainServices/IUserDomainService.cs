using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using YayZent.Framework.Rbac.Domain.Entities;

namespace YayZent.Framework.Rbac.Domain.DomainServices.IDomainServices;

public interface IUserDomainService: IDomainService, ITransientDependency
{
    Task<UserAggregateRoot> GetAsync(Guid id);
    
    Task<UserAggregateRoot> CreateUserByPhoneAsync(string userName, string password, string phone);
    
    Task<UserAggregateRoot> CreateUserByEmailAsync(string userName, string password, string email);

    Task<List<UserRoleEntity>> SetUserRolesAsync(List<Guid> userIds, List<Guid> roleIds);

    Task<List<UserPostEntity>> SetUserPostsAsync(List<Guid> userIds, List<Guid> postIds);

    Task<List<UserRoleEntity>> SetDefaultRolesAsync(Guid userId);

    Task<List<UserRoleEntity>> SetAdminRoleAsync(Guid userId);

    Task<UserAggregateRoot> LoginValidationAsync(string username, string password);
}