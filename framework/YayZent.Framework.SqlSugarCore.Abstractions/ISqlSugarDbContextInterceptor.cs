using System.Reflection;
using SqlSugar;

namespace YayZent.Framework.SqlSugarCore.Abstractions;

/// <summary>
/// SqlSugar 数据上下文拦截器接口，用于拦截数据库操作过程中的关键事件，以实现审计、日志、加解密等功能。
/// </summary>
public interface ISqlSugarDbContextInterceptor
{
    /// <summary>
    /// 拦截器的执行顺序。数值越小越先执行。
    /// </summary>
    int ExecutionOrder { get; }

    /// <summary>
    /// SqlSugar客户端配置完成后调用，可用于配置全局行为、初始化插件等。
    /// </summary>
    /// <param name="sqlSugarClient">当前 SqlSugar 客户端实例</param>
    void OnSqlSugarClientConfig(ISqlSugarClient sqlSugarClient);

    /// <summary>
    /// 数据操作（新增、修改、删除）执行后触发。
    /// 可用于记录操作日志、事件发布等。
    /// </summary>
    /// <param name="oldValue">操作前的原始数据对象</param>
    /// <param name="entityInfo">操作后实体的数据模型</param>
    void AfterDataExecuted(object oldValue, DataAfterModel entityInfo);

    /// <summary>
    /// 数据操作（新增、修改、删除）执行前触发。
    /// 可用于动态数据过滤、自动赋值（如创建时间、操作人）等。
    /// </summary>
    /// <param name="oldValue">原始数据对象</param>
    /// <param name="entityInfo">当前实体的数据过滤模型</param>
    void OnDataExecuting(object oldValue, DataFilterModel entityInfo);

    /// <summary>
    /// 执行 SQL 语句后触发。
    /// 可用于记录执行日志、性能监控等。
    /// </summary>
    /// <param name="sql">执行的 SQL 语句</param>
    /// <param name="sqlParams">SQL 参数</param>
    void AfterSqlExecuted(string sql, SugarParameter[] sqlParams);

    /// <summary>
    /// 执行 SQL 语句前触发。
    /// 可用于 SQL 审查、SQL 替换、参数加密等。
    /// </summary>
    /// <param name="sql">将要执行的 SQL 语句</param>
    /// <param name="sqlParams">SQL 参数</param>
    void OnSqlExecuting(string sql, SugarParameter[] sqlParams);

    /// <summary>
    /// 实体属性解析时触发。
    /// 可用于自定义列映射规则，如驼峰转下划线、忽略字段等。
    /// </summary>
    /// <param name="propertyInfo">实体的属性信息</param>
    /// <param name="entityColumnInfo">对应的列配置信息</param>
    void EntityService(PropertyInfo propertyInfo, EntityColumnInfo entityColumnInfo);
}
