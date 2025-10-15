using System.Reflection;
using SqlSugar;
using Volo.Abp.DependencyInjection;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.SqlSugarCore.Interceptors;

public abstract class SqlSugarDataInterceptor(IAbpLazyServiceProvider lazyServiceProvider): ISqlSugarDbContextInterceptor
{
    public int ExecutionOrder => 0;
    
    protected ISqlSugarClient? SqlSugarClient { get; private set; }
    
    protected IAbpLazyServiceProvider LazyServiceProvider { get; } = lazyServiceProvider;
    
    public void OnSqlSugarClientConfig(ISqlSugarClient sqlSugarClient)
    {
        SqlSugarClient = sqlSugarClient;
        CustomDataFilter(sqlSugarClient);
    }

    protected virtual void CustomDataFilter(ISqlSugarClient sqlSugarClient){ }

    public virtual void AfterDataExecuted(object oldValue, DataAfterModel entityInfo) { }

    public virtual void OnDataExecuting(object oldValue, DataFilterModel entityInfo) { }

    public virtual void AfterSqlExecuted(string sql, SugarParameter[] sqlParams) { }

    public virtual void OnSqlExecuting(string sql, SugarParameter[] sqlParams) { }

    public virtual void EntityService(PropertyInfo propertyInfo, EntityColumnInfo entityColumnInfo) { }
}