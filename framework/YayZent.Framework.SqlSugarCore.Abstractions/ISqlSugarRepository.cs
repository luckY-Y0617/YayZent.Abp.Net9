using System.Linq.Expressions;
using SqlSugar;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace YayZent.Framework.SqlSugarCore.Abstractions;

public interface ISqlSugarRepository<TEntity, TKey> : ISqlSugarRepository<TEntity>, IRepository<TEntity, TKey>  where 
    TEntity : class, IEntity<TKey>, new(){}

public interface ISqlSugarRepository<TEntity> : IRepository<TEntity>, IUnitOfWorkEnabled where TEntity : class, IEntity, new()
{
    ISqlSugarClient DbContext { get; }
    ISugarQueryable<TEntity> DbQueryable { get; }

    Task<ISqlSugarClient> GetDbContextAsync();
    Task<ISqlSugarClient> AsDbContextAsync();
    
    Task<IQueryable<TEntity>> GetCustomQueryableAsync();
    
    Task<IDeleteable<TEntity>> GetDbDeleteableAsync(TEntity entity);
    
    Task<IInsertable<TEntity>> GetDbInsertableAsync(TEntity entity);
    Task<IInsertable<TEntity>> GetDbInsertableAsync(TEntity[] entities);
    Task<IInsertable<TEntity>> GetDbInsertableAsync(List<TEntity> entities);
    
    Task<IUpdateable<TEntity>> GetDbUpdateableAsync();
    Task<IUpdateable<TEntity>> GetDbUpdateableAsync(TEntity entity);
    Task<IUpdateable<TEntity>> GetDbUpdateableAsync(TEntity[] entities);
    Task<IUpdateable<TEntity>> GetDbUpdateableAsync(List<TEntity> entities);

    #region 单查
    //单查
    Task<TEntity> GetByIdAsync(dynamic id);
    Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> whereExpression);
    Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> whereExpression);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> whereExpression);
    Task<long> CountAsync(Expression<Func<TEntity, bool>> whereExpression);
    #endregion

    #region 多查
    //多查
    Task<List<TEntity>> GetAllListAsync();
    Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> whereExpression);
    #endregion

    #region 分页查
    //分页查
    Task<List<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize);

    Task<List<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize, 
        Expression<Func<TEntity, object>> orderByExpression, OrderByType orderByType = OrderByType.Asc);
    #endregion

    #region 插入
    //插入
    Task<bool> InsertAsync(TEntity entity);
    Task<bool> InsertOrUpdateAsync(TEntity entity);
    Task<bool> InsertRangeAsync(TEntity[] entities);
    Task<bool> InsertRangeAsync(List<TEntity> entities);
    Task<int> InsertReturnIdentityAsync(TEntity entity);
    Task<long> InsertReturnBigIdentityAsync(TEntity entity);
    Task<long> InsertReturnSnowflakeIdAsync(TEntity entity);
    Task<Entity> InsertReturnEntityAsync(TEntity entity);
    #endregion
    
    #region 删除
    //删除
    Task<bool> DeleteAsync(TEntity entity);
    Task<bool> DeleteAsync(List<TEntity> entities);
    Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> deleteExpression);
    Task<bool> DeleteByIdAsync(dynamic id);
    Task<bool> DeleteByIdsAsync(dynamic ids);
    #endregion
}