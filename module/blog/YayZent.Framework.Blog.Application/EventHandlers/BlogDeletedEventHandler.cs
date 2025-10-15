using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Blog.Domain.Shared.Etos;
using YayZent.Framework.Core.File.Abstractions;
using YayZent.Framework.Core.File.Enums;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Blog.Application.EventHandlers;

public class BlogDeletedEventHandler: ILocalEventHandler<BlogDeletedEventArgs>, ITransientDependency
{
    private readonly ISqlSugarRepository<BlogFileEntity> _repository;
    private readonly IFileClientResolver _fileClientResolver;
    private readonly ILogger<BlogDeletedEventHandler> _logger;

    public BlogDeletedEventHandler(ISqlSugarRepository<BlogFileEntity> repository,  IFileClientResolver fileClientResolver, ILogger<BlogDeletedEventHandler> logger)
    {
        _repository = repository;
        _fileClientResolver = fileClientResolver;
        _logger = logger;
    }
    
    public async Task HandleEventAsync(BlogDeletedEventArgs eventData)
    {
        var file = await _repository.GetFirstAsync(x => x.Id == eventData.BlogFileId);

        if (file == null)
        {
            return;
        }
        
        var remoteClient = _fileClientResolver.Resolve(StorageType.Obs, "HuaWeiYun");
        var localClient = _fileClientResolver.Resolve(StorageType.Local, "Local");

        try
        {
            await remoteClient.DeleteFileAsync(file.ImageUploadUrl);
            await localClient.DeleteFileAsync(file.ImageBackUpUrl);
            await remoteClient.DeleteFileAsync(file.FileUploadUrl);
            await localClient.DeleteFileAsync(file.FileBackUpUrl);

            await _repository.DeleteAsync(x => x.Id == eventData.BlogFileId);
            _logger.LogInformation("博客文件及资源删除成功，FileId: {FileId}", eventData.BlogFileId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除博客文件失败，FileId: {FileId}", eventData.BlogFileId);
        }
;    }
}