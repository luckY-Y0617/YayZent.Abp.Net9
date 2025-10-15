using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Blog.Domain.Shared.Etos;
using YayZent.Framework.Core.File.Abstractions;
using YayZent.Framework.Core.File.Enums;
using YayZent.Framework.Core.File.Storage;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Blog.Application.EventHandlers;

public class BlogUpdatedEventHandler: ILocalEventHandler<BlogUpdatedEventArgs>, ITransientDependency
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ISqlSugarRepository<BlogFileEntity> _repository;
    private readonly IFileClientResolver _fileClientResolver;
    private readonly ILogger<BlogUpdatedEventHandler> _logger;
    
    public BlogUpdatedEventHandler(ISqlSugarRepository<BlogFileEntity> repository, IFileClientResolver fileClientResolver,  
        IFileStorageService fileStorageService,ILogger<BlogUpdatedEventHandler> logger)
    {
        _repository = repository;
        _fileClientResolver = fileClientResolver;
        _logger = logger;
        _fileStorageService = fileStorageService;
    }
    
    public async Task HandleEventAsync(BlogUpdatedEventArgs eventData)
    {
        var file = await _repository.GetFirstAsync(x => x.Id == eventData.BlogFileId);
        try
        {
            var remoteClient = _fileClientResolver.Resolve(StorageType.Obs, "HuaWeiYun");
            var localClinet = _fileClientResolver.Resolve(StorageType.Local, "Local");
            
            if (!string.IsNullOrWhiteSpace(eventData.ImagePath))
            {
                await using var imageTempStream = await _fileStorageService.GetTempFileAsync(eventData.ImagePath!);
                var imageBytes = imageTempStream.ToArray();
                await imageTempStream.DisposeAsync();
                Uri? imageUploadUrl;
                Uri? imageBackupUrl;
                await using (var imageStreamForRemote = new MemoryStream(imageBytes, false))
                {
                    imageUploadUrl = await remoteClient.UpdateFileAsync(file.ImageUploadUrl, imageStreamForRemote);
                }
                await using (var imageStreamForLocal = new MemoryStream(imageBytes, false))
                {
                    imageBackupUrl = await localClinet.UpdateFileAsync(file.ImageBackUpUrl, imageStreamForLocal);
                }
                file.ImageUploadUrl = imageUploadUrl?.ToString() ?? file.ImageUploadUrl;
                file.ImageBackUpUrl = imageBackupUrl?.LocalPath ?? file.ImageBackUpUrl;
                await _fileStorageService.DeleteTempFileAsync(eventData.ImagePath!);
            }
            
            if (!string.IsNullOrWhiteSpace(eventData.ContentPath))
            {
                await using var contentTempStream = await _fileStorageService.GetTempFileAsync(eventData.ContentPath!);
                var contentBytes = contentTempStream.ToArray();
                await contentTempStream.DisposeAsync();
                Uri? fileUploadUrl;
                Uri? fileBackupUrl;
                await using (var contentStreamForRemote = new MemoryStream(contentBytes, false))
                {
                    fileUploadUrl = await remoteClient.UpdateFileAsync(file.FileUploadUrl, contentStreamForRemote);
                }
                await using (var contentStreamForLocal = new MemoryStream(contentBytes, false))
                {
                    fileBackupUrl = await localClinet.UpdateFileAsync(file.FileBackUpUrl, contentStreamForLocal);
                }
                file.FileUploadUrl = fileUploadUrl?.ToString() ?? file.FileUploadUrl;
                file.FileBackUpUrl = fileBackupUrl?.LocalPath ?? file.FileBackUpUrl;
                await _fileStorageService.DeleteTempFileAsync(eventData.ContentPath!);
            }
            
            await _repository.UpdateAsync(file);
            
            _logger.LogInformation("博客文件及资源更新成功，FileId: {FileId}", eventData.BlogFileId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新博客文件失败，FileId: {FileId}", eventData.BlogFileId);
        }
        
    }
}