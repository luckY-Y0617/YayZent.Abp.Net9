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

namespace YayZent.Framework.SqlSugarCore;

public class DefaultSqlSugarDbContext(IAbpLazyServiceProvider lazyServiceProvider): SqlSugarDbContext(lazyServiceProvider)
{
    protected DbConnOptions DbConnOptions => LazyServiceProvider.LazyGetRequiredService<IOptions<DbConnOptions>>().Value;
    protected ICurrentUser CurrentUser => LazyServiceProvider.LazyGetRequiredService<ICurrentUser>();
    protected IGuidGenerator GuidGenerator => LazyServiceProvider.LazyGetRequiredService<IGuidGenerator>();
    protected ILoggerFactory Logger => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();
    protected IDataFilter DataFilter => LazyServiceProvider.LazyGetRequiredService<IDataFilter>();
    protected IEntityChangeEventHelper EntityChangeEventHelper => LazyServiceProvider.LazyGetService<IEntityChangeEventHelper>(NullEntityChangeEventHelper.Instance);
    public IUnitOfWorkManager UnitOfWorkManager => LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>();
    protected virtual bool IsSoftDeleteFilterEnabled => DataFilter?.IsEnabled<ISoftDelete>() ?? false;

    protected override void CustomDataFilter(ISqlSugarClient sqlSugarClient)
    {
        if (IsSoftDeleteFilterEnabled)
        {
            sqlSugarClient.QueryFilter.AddTableFilter<ISoftDelete>(u => u.IsDeleted == false);
        }
    }

    public override void OnDataExecuting(object oldValue, DataFilterModel entityInfo)
    {
        if (entityInfo.OperationType is DataFilterType.UpdateByObject or DataFilterType.InsertByObject)
        {
            HandleAuditProperties(oldValue, entityInfo);
        }
        
        HandleEntityChangeEvent(entityInfo);
    }

    public override void OnSqlExecuting(string sql, SugarParameter[] sqlParams)
    {
        if (!DbConnOptions.EnableSqlLog)
        {
            return;
        }
        
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("===========SQL执行:============");
        sb.AppendLine(UtilMethods.GetSqlString(DbType.SqlServer, sql, sqlParams));
        sb.AppendLine("===============================");
        Logger.CreateLogger<DefaultSqlSugarDbContext>().LogDebug(sb.ToString());
    }

    public override void AfterSqlExecuted(string sql, SugarParameter[] sqlParams)
    {
        if (!DbConnOptions.EnableSqlLog)
        {
            return;
        }
        
        var sqlLog = $"\"=========SQL耗时{SqlSugarClient?.Ado.SqlExecutionTime.TotalMilliseconds}毫秒=====";
        Logger.CreateLogger<SqlSugarDbContext>().LogDebug(sqlLog.ToString());
    }

    public override void EntityService(PropertyInfo propertyInfo, EntityColumnInfo entityColumnInfo)
    {
        if (propertyInfo.Name == nameof(IHasConcurrencyStamp.ConcurrencyStamp))
        {
            entityColumnInfo.IsEnableUpdateVersionValidation = true;
        }

        if (propertyInfo.PropertyType == typeof(ExtraPropertyDictionary))
        {
            entityColumnInfo.IsIgnore = true;
        }

        if (propertyInfo.Name == nameof(Entity<object>.Id))
        {
            entityColumnInfo.IsPrimarykey = true;
        }
    }

    protected virtual void HandleAuditProperties(object oldValue, DataFilterModel entityInfo)
    {
        var propName = entityInfo.PropertyName;
        var propType = entityInfo.EntityColumnInfo.PropertyInfo.PropertyType;
        switch (entityInfo.OperationType)
        {
            case DataFilterType.UpdateByObject:
                if (propName == nameof(IAuditedObject.LastModificationTime))
                {
                    entityInfo.SetValue(oldValue is DateTime dt && dt > DateTime.MinValue ? dt : DateTime.MinValue);
                }
                else if (propName == nameof(IAuditedObject.CreatorId) && propType == typeof(Guid))
                {
                    entityInfo.SetValue(oldValue is Guid creatorId && creatorId == Guid.Empty ? Guid.Empty : CurrentUser.Id);
                }
                break;
            
            case DataFilterType.InsertByObject:
                if (propName == nameof(IEntity<Guid>.Id) && propType == typeof(Guid))
                {
                    entityInfo.SetValue(oldValue is Guid id && id == Guid.Empty ? GuidGenerator.Create() : oldValue);
                }
                else if (propName == nameof(IAuditedObject.CreationTime))
                {
                    entityInfo.SetValue(oldValue is DateTime dt && dt > DateTime.MinValue ? dt : DateTime.MinValue);
                }
                else if(propName == nameof(IAuditedObject.CreatorId) && propType == typeof(Guid))
                {
                     entityInfo.SetValue(oldValue is Guid creatorId && creatorId == Guid.Empty ? Guid.Empty : CurrentUser.Id);
                }
                break;
        }
    }

    protected virtual void HandleEntityChangeEvent(DataFilterModel entityInfo)
    {
        if(entityInfo.PropertyName != nameof(IEntity<Guid>.Id)) return;

        switch (entityInfo.OperationType)
        {
            case DataFilterType.InsertByObject:
                EntityChangeEventHelper.PublishEntityCreatedEvent(entityInfo.EntityValue);
                break;
            
            case DataFilterType.UpdateByObject:
                if (entityInfo.EntityValue is ISoftDelete { IsDeleted: true })
                {
                    EntityChangeEventHelper.PublishEntityDeletedEvent(entityInfo.EntityValue);
                }
                else
                {
                    EntityChangeEventHelper.PublishEntityUpdatedEvent(entityInfo.EntityValue);
                }   
                break;
            case DataFilterType.DeleteByObject:
                if (entityInfo.EntityValue is IEnumerable entityValues)
                {
                    foreach (var entityValue in entityValues)
                    {
                        EntityChangeEventHelper.PublishEntityDeletedEvent(entityInfo.EntityValue);
                    }
                }
                break;
        }

        var entityReport = CreateEntityEventReport(entityInfo.EntityValue);
        PublishEntityEvents(entityReport);
    }

    protected virtual EntityEventReport? CreateEntityEventReport(object entity)
    {
        var eventReport = new EntityEventReport();

        if (entity is not IGeneratesDomainEvents generatesDomainEvents)
        {
            return eventReport;
        }
        
        ProcessDomainEvents(eventReport.DomainEvents,generatesDomainEvents.GetLocalEvents(), entity);
        generatesDomainEvents.ClearLocalEvents();
        
        ProcessDomainEvents(eventReport.DistributedEvents, generatesDomainEvents.GetDistributedEvents(), entity);
        generatesDomainEvents.ClearDistributedEvents();

        return eventReport;
    }

    private void ProcessDomainEvents(List<DomainEventEntry> targets, IEnumerable<DomainEventRecord>? events,  object entity)
    {
        var domainEvents = events?.ToList();
        if (domainEvents?.Count > 0)
        {
            targets.AddRange(domainEvents.Select(x => new DomainEventEntry(entity, x.EventData, x.EventOrder)));
        }
    }

    private void PublishEntityEvents(EntityEventReport? entityEventReport)
    {
        if (entityEventReport == null) return;
        
        foreach (var localEvent in entityEventReport.DomainEvents)
        {
            UnitOfWorkManager.Current?.AddOrReplaceLocalEvent(
                new UnitOfWorkEventRecord(localEvent.EventData.GetType(), localEvent.EventData, localEvent.EventOrder));
        }

        foreach (var distributedEvent in entityEventReport.DistributedEvents)
        {
            UnitOfWorkManager.Current?.AddOrReplaceDistributedEvent(
                new UnitOfWorkEventRecord(distributedEvent.EventData.GetType(), distributedEvent.EventData, distributedEvent.EventOrder));
        }
    }
}