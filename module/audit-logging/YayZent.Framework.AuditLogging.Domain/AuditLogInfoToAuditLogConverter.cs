using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.Auditing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Json;
using YayZent.Framework.AuditLogging.Domain.Entities;
using YayZent.Framework.AuditLogging.Domain.Extensions;

namespace YayZent.Framework.AuditLogging.Domain;

public class AuditLogInfoToAuditLogConverter: IAuditLogInfoToAuditLogConverter, ITransientDependency
{
    private readonly IGuidGenerator _guidGenerator;

    private readonly IExceptionToErrorInfoConverter _exceptionToErrorInfoConverter;

    private readonly IJsonSerializer _jsonSerializer;

    private readonly AbpExceptionHandlingOptions _abpExceptionHandlingOptions;

    public AuditLogInfoToAuditLogConverter(IGuidGenerator guidGenerator,
        IExceptionToErrorInfoConverter exceptionToErrorInfoConverter,
        IJsonSerializer jsonSerializer, IOptions<AbpExceptionHandlingOptions> abpExceptionHandlingOptions)
    {
        _guidGenerator = guidGenerator;
        _exceptionToErrorInfoConverter = exceptionToErrorInfoConverter;
        _jsonSerializer = jsonSerializer;
        _abpExceptionHandlingOptions = abpExceptionHandlingOptions.Value;
    }
    
    public Task<AuditLogAggregateRoot> ConvertAsync(AuditLogInfo auditLogInfo)
    {
        var auditLogId = _guidGenerator.Create();

        var extraProperties = auditLogInfo.ExtraProperties.DeepClone(_jsonSerializer);
        
        var entityChanges = (auditLogInfo.EntityChanges ?? Enumerable.Empty<EntityChangeInfo>())
            .Select(change => new EntityChangeEntity(_guidGenerator, auditLogId, change, auditLogInfo.TenantId))
            .ToList();
        
        // 处理 Actions
        var actions = (auditLogInfo.Actions ?? Enumerable.Empty<AuditLogActionInfo>())
            .Select(action => new AuditLogActionEntity(_guidGenerator.Create(), auditLogId, action, auditLogInfo.TenantId))
            .ToList();
        
        var exceptionInfos = (auditLogInfo.Exceptions ?? Enumerable.Empty<Exception>())
            .Select(ex => _exceptionToErrorInfoConverter.Convert(ex, options =>
            {
                options.SendExceptionsDetailsToClients = _abpExceptionHandlingOptions.SendExceptionsDetailsToClients;
                options.SendStackTraceToClients = _abpExceptionHandlingOptions.SendStackTraceToClients;
            }))
            .ToList();
        
        var exceptions = exceptionInfos.Count > 0
            ? _jsonSerializer.Serialize(exceptionInfos, indented: true)
            : null;
        
        var comments = auditLogInfo.Comments?.JoinAsString(Environment.NewLine);
        
        // 构造聚合根
        var auditLog = new AuditLogAggregateRoot(
            auditLogId,
            auditLogInfo.ApplicationName,
            auditLogInfo.TenantId,
            auditLogInfo.TenantName,
            auditLogInfo.UserId,
            auditLogInfo.UserName,
            auditLogInfo.ExecutionTime,
            auditLogInfo.ExecutionDuration,
            auditLogInfo.ClientIpAddress,
            auditLogInfo.ClientName,
            auditLogInfo.ClientId,
            auditLogInfo.CorrelationId,
            auditLogInfo.BrowserInfo,
            auditLogInfo.HttpMethod,
            auditLogInfo.Url,
            auditLogInfo.HttpStatusCode,
            auditLogInfo.ImpersonatorUserId,
            auditLogInfo.ImpersonatorUserName,
            auditLogInfo.ImpersonatorTenantId,
            auditLogInfo.ImpersonatorTenantName,
            extraProperties,
            entityChanges,
            actions,
            exceptions,
            comments
        );
        
        return Task.FromResult(auditLog);
    }
}