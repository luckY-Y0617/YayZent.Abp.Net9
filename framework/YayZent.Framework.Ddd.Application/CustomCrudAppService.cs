using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace YayZent.Framework.Ddd.Application;

public abstract class CustomCrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInputDto, TCreateDto, TUpdateDto>
    : CrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInputDto, TCreateDto, TUpdateDto>
    where TEntity : class, IEntity<TKey>
    where TGetOutputDto : IEntityDto<TKey>
    where TGetListOutputDto : IEntityDto<TKey>
    where TGetListInputDto: PagedAndSortedResultRequestDto
{
    protected CustomCrudAppService(IRepository<TEntity, TKey> repository): base(repository) {}
    
    #region 列表查询

    public override async Task<PagedResultDto<TGetListOutputDto>> GetListAsync(TGetListInputDto request)
    {
        List<TEntity> entities;
        long totalCount;

        if (request is IPagedResultRequest paged)
        {
            entities = await GetPagedEntitiesAsync(paged.SkipCount, paged.MaxResultCount, request.Sorting!);
            totalCount = entities.Count;
        }
        else
        {
            entities = await Repository.GetListAsync();
            totalCount = entities.Count;
        }
        
        var dtoList = ObjectMapper.Map<List<TEntity>, List<TGetListOutputDto>>(entities);
        return new PagedResultDto<TGetListOutputDto>(totalCount, dtoList);
    }

    protected virtual Task<List<TEntity>> GetPagedEntitiesAsync(int skipCount, int maxResultCount, string sorting)
    {
        return Repository.GetPagedListAsync(skipCount, maxResultCount, sorting);
    }
    
    #endregion
    
    #region —— 单条创建 & 更新 & 批量删除 ——

    /// <summary>
    /// 子类重写此方法对 TCreateDto 做业务校验，例如字段唯一性等。
    /// 默认不做校验。
    /// </summary>
    protected virtual Task CheckCreateInputDtoAsync(TCreateDto request)
    {
        return Task.CompletedTask;
    }

    public override async Task<TGetOutputDto> CreateAsync(TCreateDto request)
    {
        // 1. 验证 ABP 内置权限（CreatePolicyName）
        await CheckCreatePolicyAsync();
        // 2. 自定义校验
        await CheckCreateInputDtoAsync(request);
        var entity = await MapToEntityAsync(request);
        TryToSetTenantId(entity);
        await Repository.InsertAsync(entity, true);
        return await MapToGetOutputDtoAsync(entity);
    }

    /// <summary>
    /// 子类重写此方法对 TUpdateDto 做业务校验，例如不允许更新某些字段等。
    /// 默认不做校验。
    /// </summary>
    protected virtual Task CheckUpdateInputDtoAsync(TEntity entity, TUpdateDto request)
    {
        return Task.CompletedTask;
    }

    public override async Task<TGetOutputDto> UpdateAsync(TKey id,TUpdateDto request)
    {
        await CheckUpdatePolicyAsync();
        var entity = await GetEntityByIdAsync(id);
        await CheckUpdateInputDtoAsync(entity, request);
        await MapToEntityAsync(request, entity);
        await Repository.UpdateAsync(entity, true);
        return await MapToGetOutputDtoAsync(entity);
    }

    /// <summary>
    /// 批量删除接口，建议客户端调用此接口进行多条删除。
    /// </summary>
    [RemoteService(IsEnabled = true)]
    public virtual async Task DeleteManyAsync(IEnumerable<TKey> ids)
    {
        await CheckDeletePolicyAsync();
        await Repository.DeleteManyAsync(ids);
    }

    /// <summary>
    /// 禁用单条 DeleteAsync，以强制使用批量删除。
    /// </summary>
    [RemoteService(IsEnabled = false)]
    public override Task DeleteAsync(TKey id)
    {
        return base.DeleteAsync(id);
    }
    
    #endregion
    
    #region —— Excel 导出 & 导入 ——

    public virtual async Task<FileContentResult> ExportToExcelAsync(TGetListInputDto request)
    {
        // 1. 如果前端传了分页参数，先获取总数，再导出全部
        if (request is IPagedResultRequest paged)
        {
            paged.SkipCount = 0;
            paged.MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount;
        }

        // 2. 重用 GetListAsync 拿到完整列表 DTO
        var pagedResult = await GetListAsync(request);
        var dtos = pagedResult.Items;
        
        // 3. 在内存中生成 Excel
        await using var ms = new MemoryStream();
        await ms.SaveAsAsync(dtos);
        
        var fileBytes = ms.ToArray();
        var fileName =  $"{typeof(TEntity).Name}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        
        return new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"){FileDownloadName = fileName};
    }

    /// <summary>
    /// 导入 Excel，将 IFormFile 解析成 List&lt;TCreateDto&gt;，再映射为实体批量插入。
    /// 子类可重写以增加校验或去重逻辑。
    /// </summary>
    public virtual Task ImportToExcelAsync(IFormFile? file)
    {
        throw new NotImplementedException();
    }
    #endregion
    
    
}

#region —— 可选的泛型简化重载 —— 

/// <summary>
/// 如果想少写泛型参数，可以再做一层简化：
/// TEntity: 实体类型。
/// TKey: 主键类型。
/// （此时单条/列表 DTO、Create/Update DTO 都用同一个 DTO 类型 IEntityDto<TKey/>，仅示例，如有更细化需求可自行添加更多重载）
/// </summary>
public abstract class CustomCrudAppService<TEntity, TKey>
    : CustomCrudAppService<
        TEntity,
        IEntityDto<TKey>,
        IEntityDto<TKey>,
        TKey,
        PagedAndSortedResultRequestDto,
        IEntityDto<TKey>,
        IEntityDto<TKey>>
    where TEntity : class, IEntity<TKey>
{
    protected CustomCrudAppService(IRepository<TEntity, TKey> repository)
        : base(repository)
    {
    }
}

/// <summary>
/// 如果希望查询单条 DTO 与列表 DTO 相同，只需要填两个泛型：
/// TEntity    : 实体类型
/// TEntityDto : 同时作为 TGetDto/TGetListDto/TCreateDto/TUpdateDto
/// TKey       : 主键类型
/// </summary>
public abstract class CustomCrudAppService<TEntity, TEntityDto, TKey>
    : CustomCrudAppService<
        TEntity,
        TEntityDto,
        TEntityDto,
        TKey,
        PagedAndSortedResultRequestDto,
        TEntityDto,
        TEntityDto>
    where TEntity : class, IEntity<TKey>
    where TEntityDto : IEntityDto<TKey>
{
    protected CustomCrudAppService(IRepository<TEntity, TKey> repository)
        : base(repository)
    {
    }
}

#endregion