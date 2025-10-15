using System.Linq.Expressions;
using Nito.AsyncEx;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using YayZent.Framework.Core.Helper;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.SqlSugarCore.Repositories;

public class SqlSugarRepository<TEntity>(ISugarDbContextProvider<ISqlSugarDbContext> 
    sugarDbContextProvider) : ISqlSugarRepository<TEntity> where TEntity : class, IEntity, new()
{
    public bool? IsChangeTrackingEnabled  => false;

    public IAsyncQueryableExecuter AsyncExecuter { get; }
    // AsyncContext.Run 可能引起死锁（尤其是在 ASP.NET Core 中）？？？。
    
    public ISqlSugarClient DbContext => AsyncContext.Run(async () =>  await GetDbContextAsync());
    
    public ISugarQueryable<TEntity> DbQueryable => DbContext.Queryable<TEntity>();

    private readonly ISugarDbContextProvider<ISqlSugarDbContext> _sugarDbContextProvider = sugarDbContextProvider;
    
    
    /// <summary>
    /// 获取db
    /// </summary>
    /// <returns></returns>
    public virtual async Task<ISqlSugarClient> GetDbContextAsync()
    {
        var db = (await _sugarDbContextProvider.GetDbContextAsync()).SqlSugarClient;
        return db;
    }
    
    public async Task<SimpleClient<TEntity>> GetSimpleDbClientAsync()
    {
        var db = await GetDbContextAsync();
        return new SimpleClient<TEntity>(db);
    }

    public async Task<ISqlSugarClient> AsDbContextAsync()
    {
        return (await GetSimpleDbClientAsync()).AsSugarClient();
    }
    
    public Task<IQueryable<TEntity>> GetCustomQueryableAsync()
    {
        throw new NotImplementedException();
    }

    #region abp模块

    public Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<long> GetCountAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public async Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, bool includeDetails = false,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return await GetPageListAsync(_ => true, skipCount, maxResultCount);
    }

    protected virtual async Task<List<TEntity>> GetPageListAsync(Expression<Func<TEntity, bool>> whereExpression, int pageNum, int pageSize)
    {
        return await (await GetSimpleDbClientAsync()).GetPageListAsync(whereExpression, new PageModel() { PageIndex = pageNum, PageSize = pageSize });
    }
    
    public IQueryable<TEntity> WithDetails()
    {
        throw new NotImplementedException();
    }

    public IQueryable<TEntity> WithDetails(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        throw new NotImplementedException();
    }

    public Task<IQueryable<TEntity>> WithDetailsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IQueryable<TEntity>> WithDetailsAsync(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        throw new NotImplementedException();
    }

    public Task<IQueryable<TEntity>> GetQueryableAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = false,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public async Task<TEntity> InsertAsync(TEntity entity, bool autoSave, CancellationToken cancellationToken = new CancellationToken())
    {
        return await InsertReturnEntityAsync(entity);
    }

    public async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var entitiesList = entities.ToList();
        if (entities == null || !entitiesList.Any())
        {
            throw new ArgumentException("Entities cannot be null or empty", nameof(entities));
        }

        // 添加所有实体
        await DbContext.Insertable(entitiesList).ExecuteCommandAsync(cancellationToken);

        // 如果 autoSave 为 true，立即保存更改
        if (autoSave)
        {
            // 在 SqlSugar 中，Insertable 本身会自动保存，不需要额外调用 SaveChanges
            // 如果需要事务控制，可以通过事务来确保操作的原子性
            await DbContext.Ado.CommitTranAsync();
        }
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = new CancellationToken())
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // 使用 SqlSugar 异步更新
        var result = await DbContext.Updateable(entity)
            .ExecuteCommandAsync(cancellationToken);

        // result 是受影响的行数，理论上应该是1
        if (result == 0)
            throw new Exception("更新失败，没有匹配到记录");

        // 如果autoSave的语义是指事务提交或者其他什么操作
        // SqlSugar默认每次ExecuteCommandAsync都会提交，这里不需要额外处理
        return entity;
    }

    public async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await UpdateRangeAsync(entities.ToList());
    }

    public Task DeleteAsync(TEntity entity, bool autoSave, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
    
    public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await DeleteAsync(entities.ToList());
    }

    public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = true,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = true,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task DeleteDirectAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
    #endregion

    #region sqlsugar模块
    public Task<IDeleteable<TEntity>> GetDbDeleteableAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<IInsertable<TEntity>> GetDbInsertableAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<IInsertable<TEntity>> GetDbInsertableAsync(TEntity[] entities)
    {
        throw new NotImplementedException();
    }

    public Task<IInsertable<TEntity>> GetDbInsertableAsync(List<TEntity> entities)
    {
        throw new NotImplementedException();
    }

    public Task<IUpdateable<TEntity>> GetDbUpdateableAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IUpdateable<TEntity>> GetDbUpdateableAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<IUpdateable<TEntity>> GetDbUpdateableAsync(TEntity[] entities)
    {
        throw new NotImplementedException();
    }

    public Task<IUpdateable<TEntity>> GetDbUpdateableAsync(List<TEntity> entities)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> GetByIdAsync(dynamic id)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        throw new NotImplementedException();
    }

    public async Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        var entity = await DbQueryable.FirstAsync(whereExpression);
        return entity;
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return await DbQueryable.AnyAsync(whereExpression);
    }

    public Task<long> CountAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> GetAllListAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderByExpression,
        OrderByType orderByType = OrderByType.Asc)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<bool> InsertAsync(TEntity entity)
    {
        return await( await GetSimpleDbClientAsync()).InsertAsync(entity);
    }

    public Task<bool> InsertOrUpdateAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> InsertRangeAsync(TEntity[] entities)
    {
        throw new NotImplementedException();
    }

    public Task<bool> InsertRangeAsync(List<TEntity> entities)
    {
        throw new NotImplementedException();
    }

    public Task<int> InsertReturnIdentityAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<long> InsertReturnBigIdentityAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<long> InsertReturnSnowflakeIdAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public async Task<TEntity> InsertReturnEntityAsync(TEntity insertObj)
    {
        return await (await GetSimpleDbClientAsync()).InsertReturnEntityAsync(insertObj);
    }

    public Task<bool> DeleteAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(List<TEntity> entities)
    {
        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            entities.ForEach(e => ReflexHelper.SetModelValue(nameof(ISoftDelete.IsDeleted), true, e));
            return await (await GetSimpleDbClientAsync()).UpdateRangeAsync(entities);
        }
        else
        {
            return await (await GetSimpleDbClientAsync()).DeleteAsync(entities);
        }
    }

    public async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> deleteExpression)
    {
        try
        {
            var result = await DbContext.Deleteable<TEntity>().Where(deleteExpression).ExecuteCommandAsync();
        }
        catch (Exception ex)
        {
            return false;
        }
        return true;
    }

    public Task<bool> DeleteByIdAsync(dynamic id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteByIdsAsync(dynamic[] ids)
    {
        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            var simpleClient = (await GetSimpleDbClientAsync());
            var entities = await simpleClient.AsQueryable().In(ids).ToListAsync();
            if (entities.Count == 0)
            {
                return false;
            }
            //反射赋值
            entities.ForEach(e => ReflexHelper.SetModelValue(nameof(ISoftDelete.IsDeleted), true, e));
            return await UpdateRangeAsync(entities);
        }
        else
        {
            return await (await GetSimpleDbClientAsync()).DeleteByIdAsync(ids);
        }
    }
    
    public virtual async Task<bool> UpdateAsync(TEntity entity)
    {
        if (typeof(TEntity).IsAssignableTo<IHasConcurrencyStamp>())//带版本号乐观锁更新
        {
            try
            {
                int num =  await (await GetSimpleDbClientAsync())
                    .Context.Updateable(entity).ExecuteCommandWithOptLockAsync(true);
                return num>0;
            }
            catch (VersionExceptions ex)
            {
 
                throw new AbpDbConcurrencyException($"{ex.Message}[更新失败：ConcurrencyStamp不是最新版本],entityInfo：{entity}", ex);
            }
        }
        return await (await GetSimpleDbClientAsync()).UpdateAsync(entity);
    }

    public virtual async Task<bool> UpdateRangeAsync(List<TEntity> entities)
    {
        return await (await GetSimpleDbClientAsync()).UpdateRangeAsync(entities);
    }

    public virtual async Task<bool> UpdateAsync(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> predicate)
    {
        return await (await GetSimpleDbClientAsync()).UpdateAsync(columns, predicate);
    }


    #endregion


        
}

public class SqlSugarRepository<TEntity, TKey>(ISugarDbContextProvider<ISqlSugarDbContext> sugarDbContextProvider) : 
    SqlSugarRepository<TEntity>(sugarDbContextProvider), ISqlSugarRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>, new()
{
    public async Task<TEntity> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = new CancellationToken())
    {
        var entity = await DbQueryable.InSingleAsync(id);

        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TEntity), id);
        }
        
        return entity;
    }

    public Task<TEntity?> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public async Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await DeleteByIdsAsync(ids.Select(x => (object)x).ToArray());
    }
}