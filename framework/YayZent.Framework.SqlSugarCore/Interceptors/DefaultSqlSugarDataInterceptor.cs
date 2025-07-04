using System.Collections;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.SqlSugarCore.Interceptors
{
    public class DefaultSqlSugarDataInterceptor : SqlSugarDataInterceptor
    {
        public DefaultSqlSugarDataInterceptor(IAbpLazyServiceProvider lazyServiceProvider): base(lazyServiceProvider)
        {
        }

        protected DbConnOptions DbConnOptions => LazyServiceProvider.LazyGetRequiredService<IOptions<DbConnOptions>>().Value;

        protected ICurrentUser CurrentUser => LazyServiceProvider.LazyGetRequiredService<ICurrentUser>();
        protected IGuidGenerator GuidGenerator => LazyServiceProvider.LazyGetRequiredService<IGuidGenerator>();
        protected ILoggerFactory Logger => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();
        protected IDataFilter DataFilter => LazyServiceProvider.LazyGetRequiredService<IDataFilter>();

        protected IEntityChangeEventHelper EntityChangeEventHelper => LazyServiceProvider.LazyGetService<IEntityChangeEventHelper>(NullEntityChangeEventHelper.Instance);

        public IUnitOfWorkManager UnitOfWorkManager => LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>();
        protected virtual bool IsSoftDeleteFilterEnabled => DataFilter?.IsEnabled<ISoftDelete>() ?? false;

        #region 数据操作之前执行

        /// <summary>
        /// 在数据操作执行前调用，处理审计属性和实体变更事件。
        /// </summary>
        /// <param name="oldValue">实体属性的旧值。</param>
        /// <param name="entityInfo">包含操作类型和实体值的实体信息。</param>
        public override void OnDataExecuting(object oldValue, DataFilterModel entityInfo)
        {
            if (entityInfo.OperationType == DataFilterType.UpdateByObject || entityInfo.OperationType == DataFilterType.InsertByObject)
            {
                HandleAuditProperties(oldValue, entityInfo);
            }

            HandleEntityChangeEvent(entityInfo);
        }
        
        /// <summary>
        /// 处理数据操作中的审计属性。
        /// </summary>
        /// <param name="oldValue">实体的旧值。</param>
        /// <param name="entityInfo">包含操作类型和属性名称的实体信息。</param>
        protected virtual void HandleAuditProperties(object oldValue, DataFilterModel entityInfo)
        {
            var propName = entityInfo.PropertyName;
            var propType = entityInfo.EntityColumnInfo.PropertyInfo.PropertyType;

            if (entityInfo.OperationType == DataFilterType.UpdateByObject)
            {
                HandleUpdateAuditProperties(oldValue, entityInfo, propName, propType);
            }
            else if (entityInfo.OperationType == DataFilterType.InsertByObject)
            {
                HandleInsertAuditProperties(oldValue, entityInfo, propName, propType);
            }
        }

        /// <summary>
        /// 处理更新操作中的审计属性。
        /// </summary>
        /// <param name="oldValue">实体属性的旧值。</param>
        /// <param name="entityInfo">实体信息，包含属性名称和类型。</param>
        /// <param name="propName">正在更新的属性名称。</param>
        /// <param name="propType">正在更新的属性类型。</param>
        private void HandleUpdateAuditProperties(object oldValue, DataFilterModel entityInfo, string propName,
            Type propType)
        {
            switch (propName)
            {
                case nameof(IAuditedObject.LastModificationTime):
                    entityInfo.SetValue(oldValue is DateTime dt && dt > DateTime.MinValue ? dt : DateTime.Now);
                    break;

                case nameof(IAuditedObject.CreatorId) when propType == typeof(Guid):
                    entityInfo.SetValue(oldValue is Guid creatorId && creatorId == Guid.Empty
                        ? Guid.Empty
                        : CurrentUser.Id);
                    break;
            }
        }

        /// <summary>
        /// 处理插入操作中的审计属性。
        /// </summary>
        /// <param name="oldValue">实体属性的旧值。</param>
        /// <param name="entityInfo">实体信息，包含属性名称和类型。</param>
        /// <param name="propName">正在插入的属性名称。</param>
        /// <param name="propType">正在插入的属性类型。</param>
        private void HandleInsertAuditProperties(object oldValue, DataFilterModel entityInfo, string propName, Type propType)
        {
            switch (propName)
            {
                case nameof(IEntity<Guid>.Id) when propType == typeof(Guid):
                    entityInfo.SetValue(oldValue is Guid id && id == Guid.Empty ? GuidGenerator.Create() : oldValue);
                    break;

                case nameof(IAuditedObject.CreationTime):
                    entityInfo.SetValue(oldValue is DateTime dt && dt > DateTime.MinValue ? dt : DateTime.Now);
                    break;

                case nameof(IAuditedObject.CreatorId) when propType == typeof(Guid):
                    entityInfo.SetValue(oldValue is Guid creatorId && creatorId == Guid.Empty
                        ? Guid.Empty
                        : CurrentUser.Id);
                    break;
            }
        }

        /// <summary>
        /// 在数据操作前处理实体变更事件。
        /// </summary>
        /// <param name="entityInfo">包含操作类型和实体值的实体信息。</param>
        protected virtual void HandleEntityChangeEvent(DataFilterModel entityInfo)
        {
            if (entityInfo.PropertyName != nameof(IEntity<Guid>.Id)) return;

            switch (entityInfo.OperationType)
            {
                case DataFilterType.InsertByObject:
                    EntityChangeEventHelper.PublishEntityCreatedEvent(entityInfo.EntityValue);
                    break;

                case DataFilterType.UpdateByObject:
                    PublishUpdateEntityChangeEvent(entityInfo);
                    break;

                case DataFilterType.DeleteByObject:
                    PublishDeleteEntityChangeEvent(entityInfo);
                    break;
            }

            var entityReport = CreateEntityEventReport(entityInfo.EntityValue);
            PublishEntityEvents(entityReport);
        }

        /// <summary>
        /// 发布更新实体变更事件。
        /// </summary>
        /// <param name="entityInfo">包含更新实体值的实体信息。</param>
        private void PublishUpdateEntityChangeEvent(DataFilterModel entityInfo)
        {
            if (entityInfo.EntityValue is ISoftDelete { IsDeleted: true })
                EntityChangeEventHelper.PublishEntityDeletedEvent(entityInfo.EntityValue);
            else
                EntityChangeEventHelper.PublishEntityUpdatedEvent(entityInfo.EntityValue);
        }

        /// <summary>
        /// 发布删除实体变更事件。
        /// </summary>
        /// <param name="entityInfo">包含删除实体值的实体信息。</param>
        private void PublishDeleteEntityChangeEvent(DataFilterModel entityInfo)
        {
            if (entityInfo.EntityValue is IEnumerable entityValues)
            {
                foreach (var entityValue in entityValues)
                {
                    EntityChangeEventHelper.PublishEntityDeletedEvent(entityValue);
                }
            }
        }

        /// <summary>
        /// 创建实体事件报告，包括该实体的领域事件。
        /// </summary>
        /// <param name="entity">需要生成事件报告的实体。</param>
        /// <returns>包含领域事件的实体事件报告。</returns>
        protected virtual EntityEventReport? CreateEntityEventReport(object entity)
        {
            var eventReport = new EntityEventReport();

            if (entity is IGeneratesDomainEvents generatesDomainEvents)
            {
                ProcessDomainEvents(eventReport.DomainEvents, generatesDomainEvents.GetLocalEvents(), entity);
                generatesDomainEvents.ClearLocalEvents();

                ProcessDomainEvents(eventReport.DistributedEvents, generatesDomainEvents.GetDistributedEvents(),
                    entity);
                generatesDomainEvents.ClearDistributedEvents();
            }

            return eventReport;
        }

        /// <summary>
        /// 处理领域事件并将其添加到目标事件列表中。
        /// </summary>
        /// <param name="targets">将事件添加到的目标事件列表。</param>
        /// <param name="events">需要处理的领域事件。</param>
        /// <param name="entity">生成事件的实体。</param>
        private void ProcessDomainEvents(List<DomainEventEntry> targets, IEnumerable<DomainEventRecord>? events,
            object entity)
        {
            var domainEvents = events?.ToList();
            if (domainEvents?.Count > 0)
            {
                targets.AddRange(domainEvents.Select(x => new DomainEventEntry(entity, x.EventData, x.EventOrder)));
            }
        }

        #endregion

        #region Sql执行之前调用

        /// <summary>
        /// 在 SQL 查询执行前调用，若启用了 SQL 日志，则记录 SQL。
        /// </summary>
        /// <param name="sql">正在执行的 SQL 查询。</param>
        /// <param name="sqlParams">SQL 查询的参数。</param>
        public override void OnSqlExecuting(string sql, SugarParameter[] sqlParams)
        {
            if (!DbConnOptions.EnableSqlLog) return;

            var sb = new StringBuilder();
            sb.AppendLine("===========SQL执行:============");
            sb.AppendLine(UtilMethods.GetSqlString(DbType.SqlServer, sql, sqlParams));
            sb.AppendLine("===============================");
            Logger.CreateLogger<DefaultSqlSugarDataInterceptor>().LogDebug(sb.ToString());
        }

        #endregion

        #region SQL 执行后处理
        
        /// <summary>
        /// 在 SQL 查询执行后调用，若启用了 SQL 日志，则记录 SQL 执行时间。
        /// </summary>
        /// <param name="sql">已执行的 SQL 查询。</param>
        /// <param name="sqlParams">SQL 查询的参数。</param>
        public override void AfterSqlExecuted(string sql, SugarParameter[] sqlParams)
        {
            if (!DbConnOptions.EnableSqlLog) return;

            var sqlLog = $"=========SQL耗时{SqlSugarClient?.Ado.SqlExecutionTime.TotalMilliseconds}毫秒=====";
            Logger.CreateLogger<SqlSugarDataInterceptor>().LogDebug(sqlLog);
        }

        #endregion

        #region 实体服务

        /// <summary>
        /// 配置实体列属性映射到数据库前的设置。
        /// </summary>
        /// <param name="propertyInfo">实体属性信息。</param>
        /// <param name="entityColumnInfo">实体列信息。</param>
        public override void EntityService(PropertyInfo propertyInfo, EntityColumnInfo entityColumnInfo)
        {
            if (propertyInfo.Name == nameof(IHasConcurrencyStamp.ConcurrencyStamp))
                entityColumnInfo.IsEnableUpdateVersionValidation = true;

            if (propertyInfo.PropertyType == typeof(ExtraPropertyDictionary))
                entityColumnInfo.IsIgnore = true;

            if (propertyInfo.Name == nameof(Entity<object>.Id))
                entityColumnInfo.IsPrimarykey = true;
        }

        #endregion

        #region 发布实体事件

        /// <summary>
        /// 发布实体事件，包括领域事件和分布式事件。
        /// </summary>
        /// <param name="entityEventReport">包含领域事件和分布式事件的实体事件报告。</param>
        private void PublishEntityEvents(EntityEventReport? entityEventReport)
        {
            if (entityEventReport == null) return;

            foreach (var localEvent in entityEventReport.DomainEvents)
            {
                UnitOfWorkManager.Current?.AddOrReplaceLocalEvent(
                    new UnitOfWorkEventRecord(localEvent.EventData.GetType(), localEvent.EventData,
                        localEvent.EventOrder));
            }

            foreach (var distributedEvent in entityEventReport.DistributedEvents)
            {
                UnitOfWorkManager.Current?.AddOrReplaceDistributedEvent(
                    new UnitOfWorkEventRecord(distributedEvent.EventData.GetType(), distributedEvent.EventData,
                        distributedEvent.EventOrder));
            }
        }

        #endregion
    }
}
