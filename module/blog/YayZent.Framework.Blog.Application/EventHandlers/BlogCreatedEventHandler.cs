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

public class BlogCreatedEventHandler: ILocalEventHandler<BlogCreatedEventArgs>, ITransientDependency
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ISqlSugarRepository<BlogFileEntity> _repository;
    private readonly IFileClientResolver _fileClientResolver;
    private readonly ILogger<BlogCreatedEventHandler> _logger;

    public BlogCreatedEventHandler(ISqlSugarRepository<BlogFileEntity> repository, IFileClientResolver fileClientResolver,  
        IFileStorageService fileStorageService,ILogger<BlogCreatedEventHandler> logger)
    {
        _repository = repository;
        _fileClientResolver = fileClientResolver;
        _logger = logger;
        _fileStorageService = fileStorageService;
    }
    
    public async Task HandleEventAsync(BlogCreatedEventArgs eventData)
    {
        DateTime today = DateTime.Now;
        string? imageKey = eventData.ImagePath != null ? $"{today:yyyy/MM/dd}/{eventData.BlogFileId}/{eventData.ImagePath}" : null;
        string? fileKey = eventData.ContentPath != null ? $"{today:yyyy/MM/dd}/{eventData.BlogFileId}/{eventData.ContentPath}" : null;
        
        try
        {
            var file = await _repository.GetFirstAsync(x => x.Id == eventData.BlogFileId);

            if (file == null)
            {
                throw new ArgumentException("Can not find file");
            }
            
            var remoteClient = _fileClientResolver.Resolve(StorageType.Obs, "HuaWeiYun");
            var localClinet = _fileClientResolver.Resolve(StorageType.Local, "Local");

            Uri? uploadedFileUrl = null;
            Uri? backupFileUrl = null;
            Uri? uploadedImageUrl = null;
            Uri? backupImageUrl = null;

            if (!string.IsNullOrWhiteSpace(fileKey) && eventData.ContentPath != null)
            {
                await using var contentTempStream = await _fileStorageService.GetTempFileAsync(eventData.ContentPath);
                var contentBytes = contentTempStream.ToArray();
                await contentTempStream.DisposeAsync();
                await using (var contentStreamForRemote = new MemoryStream(contentBytes, false))
                {
                    uploadedFileUrl = await remoteClient.SaveFileAsync(fileKey!, contentStreamForRemote);
                }
                await using (var contentStreamForLocal = new MemoryStream(contentBytes, false))
                {
                    backupFileUrl = await localClinet.SaveFileAsync(fileKey!, contentStreamForLocal);
                }
                await _fileStorageService.DeleteTempFileAsync(eventData.ContentPath);
            }

            if (!string.IsNullOrWhiteSpace(imageKey) && eventData.ImagePath != null)
            {
                await using var imageTempStream = await _fileStorageService.GetTempFileAsync(eventData.ImagePath);
                var imageBytes = imageTempStream.ToArray();
                await imageTempStream.DisposeAsync();
                await using (var imageStreamForRemote = new MemoryStream(imageBytes, false))
                {
                    uploadedImageUrl = await remoteClient.SaveFileAsync(imageKey!, imageStreamForRemote);
                }
                await using (var imageStreamForLocal = new MemoryStream(imageBytes, false))
                {
                    backupImageUrl = await localClinet.SaveFileAsync(imageKey!, imageStreamForLocal);
                }
                await _fileStorageService.DeleteTempFileAsync(eventData.ImagePath);
            }

            file.SetUrl(uploadedFileUrl, backupFileUrl, uploadedImageUrl, backupImageUrl);

            await _repository.UpdateAsync(file);

            _logger.LogInformation("博客文件及资源创建成功，FileId: {FileId}", eventData.BlogFileId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清理博客文件失败，FileId: {FileId}", eventData.BlogFileId);
        }
        
    }
}