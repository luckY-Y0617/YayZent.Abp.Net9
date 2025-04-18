using System.Reflection;
using SqlSugar;

namespace YayZent.Framework.SqlSugarCore.Abstractions;

public interface ISqlSugarDbContextInterceptor
{
    int ExecutionOrder { get; }
    
    /// <summary>
    /// 在sqlsugarclient配置前触发
    /// </summary>
    /// <param name="sqlSugarClient"></param>
    void OnSqlSugarClientConfig(ISqlSugarClient sqlSugarClient);
    
    /// <summary>
    /// 在数据变更后触发
    /// </summary>
    /// <param name="oldValue">变更前的数据</param>
    /// <param name="entityInfo">即将更新的数据</param>
    void AfterDataExecuted(object oldValue, DataAfterModel entityInfo);
    
    /// <summary>
    /// 在数据变更前触发
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="entityInfo"></param>
    void OnDataExecuting(object oldValue, DataFilterModel entityInfo);
    
    /// <summary>
    /// 在sql语句执行后触发
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="sqlParams"></param>
    void AfterSqlExecuted(string sql, SugarParameter[] sqlParams);
    
    /// <summary>
    /// 在sql语句执行前触发
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="sqlParams"></param>
    void OnSqlExecuting(string sql, SugarParameter[] sqlParams);
    
    /// <summary>
    /// 处理实体类的属性信息，可用于自定义属性映射规则。
    /// </summary>
    /// <param name="propertyInfo">实体类的属性信息</param>
    /// <param name="entityColumnInfo">SqlSugar 实体列信息</param>
    void EntityService(PropertyInfo propertyInfo, EntityColumnInfo entityColumnInfo);
}