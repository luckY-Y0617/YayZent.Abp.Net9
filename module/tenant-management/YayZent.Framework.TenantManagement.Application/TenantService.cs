using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Uow;
using Volo.Abp.Modularity;
using YayZent.Framework.SqlSugarCore.Abstractions;
using YayZent.Framework.TenantManagement.Application.Contracts;
using YayZent.Framework.TenantManagement.Application.Contracts.Dtos;
using YayZent.Framework.TenantManagement.Domain;

namespace YayZent.Framework.TenantManagement.Application;

public class TenantService(ISqlSugarRepository<TenantAggregateRoot, Guid> repository, IDataSeeder dataSeeder):CrudAppService<TenantAggregateRoot, TenantGetOutputDto, TenantGetListOutputDto, Guid, TenantGetListInputDto,
    TenantCreateDto, TenantUpdateDto>(repository), ITenantService
{
    private readonly ISqlSugarRepository<TenantAggregateRoot, Guid> _repository = repository;
    private readonly IDataSeeder _dataSeeder = dataSeeder;
    
    public override Task<TenantGetOutputDto> GetAsync(Guid id)
    {
        return base.GetAsync(id);
    }

    public override async Task<PagedResultDto<TenantGetListOutputDto>> GetListAsync(TenantGetListInputDto inputDto)
    {
        RefAsync<int> total = 0;

        var entities = await _repository.DbQueryable
            .WhereIF(!string.IsNullOrEmpty(inputDto.Name), x => x.Name.Contains(inputDto.Name!))
            .WhereIF(inputDto.StartTime is not null && inputDto.EndTime is not null,
                x => x.CreationTime >= inputDto.StartTime && x.CreationTime <= inputDto.EndTime)
            .ToPageListAsync(inputDto.SkipCount, inputDto.MaxResultCount, total);
        return new PagedResultDto<TenantGetListOutputDto>(total, await MapToGetListOutputDtosAsync(entities));
    }

    
    public override async Task<TenantGetOutputDto> CreateAsync(TenantCreateDto dto)
    {
        if (await _repository.DbQueryable.AnyAsync(x => x.Name == dto.Name))
        {
            throw new UserFriendlyException("创建失败，当前租户已存在");
        }

        return await base.CreateAsync(dto);
    }

    public override async Task<TenantGetOutputDto> UpdateAsync(Guid id, TenantUpdateDto dto)
    {
        if (await _repository.AnyAsync(x => x.Name == dto.Name && x.Id != id))
        {
            throw new UserFriendlyException("更新后租户名已经存在");
        }

        return await base.UpdateAsync(id, dto);
    }

    public override Task DeleteAsync(Guid id)
    {
        return base.DeleteAsync(id);
    }
    
    /// <summary>
    /// 初始化租户
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("tenant/init/{id}")]
    public async Task InitAsync([FromRoute] Guid id)
    {
        // 在切换租户（CurrentTenant.Change(id)）之前，显式保存当前租户上下文中的挂起更改。
        await CurrentUnitOfWork.SaveChangesAsync();
        using (CurrentTenant.Change(id))
        {
            await CodeFirstAsync(this.LazyServiceProvider);
            await _dataSeeder.SeedAsync(id);
        }
    }

    private async Task CodeFirstAsync(IServiceProvider serviceProvider)
    {
        var moudleContainer = serviceProvider.GetRequiredService<IModuleContainer>();

        using (var uow = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
        {
            ISqlSugarClient db = _repository.DbContext;

            db.DbMaintenance.CreateDatabase();
            
            List<Type> types = new List<Type>();
            foreach (var module in moudleContainer.Modules)
            {
                types.AddRange(module.Assembly.GetTypes()
                    .Where(x => x.GetCustomAttribute<IgnoreCodeFirstAttribute>() == null)
                    .Where(x => x.GetCustomAttribute<SugarTable>() != null)
                    .Where(x => x.GetCustomAttribute<DefaultTenantTableAttribute>() is null)
                    .Where(x => x.GetCustomAttribute<SplitTableAttribute>() is null)); 
            }
            
            if (types.Count > 0)
            {
                db.CodeFirst.InitTables(types.ToArray());
            }

            await uow.CompleteAsync();
        }
    }
}