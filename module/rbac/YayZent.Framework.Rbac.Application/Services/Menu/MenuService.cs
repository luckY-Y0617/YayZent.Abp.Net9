using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using YayZent.Framework.Ddd.Application;
using YayZent.Framework.Rbac.Application.Contracts.Dtos.Menu;
using YayZent.Framework.Rbac.Domain.Entities;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Rbac.Application.Services.Menu;

public class MenuService: CustomCrudAppService<MenuAggregateRoot, MenuGetOutputDto, MenuGetListOutputDto, Guid, MenuGetListInputDto, MenuCreateDto, MenuUpdateDto>
{
    private readonly ISqlSugarRepository<MenuAggregateRoot, Guid> _menuRepository;
    
    public MenuService(ISqlSugarRepository<MenuAggregateRoot, Guid> menuRepository) : base(menuRepository)
    {
        _menuRepository = menuRepository;
    }
    
    
}