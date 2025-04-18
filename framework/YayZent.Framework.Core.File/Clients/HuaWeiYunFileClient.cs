using System.Net;
using Microsoft.Extensions.Options;
using OBS;
using OBS.Model;
using YayZent.Framework.Core.File.Abstractions;
using YayZent.Framework.Core.File.Enums;
using YayZent.Framework.Core.File.Options;
using YayZent.Framework.Core.File.Extensions;

namespace YayZent.Framework.Core.File.Clients;

public class HuaWeiYunFileClient: IFileClient
{
    private readonly IOptionsSnapshot<HuaWeiYunStorageOptions> options;

    public string Provider => "HuaWeiYun";
    public StorageType StorageType => StorageType.Obs;
    
    public ObsClient ObsClient { get; private set; }

    public HuaWeiYunFileClient(IOptionsSnapshot<HuaWeiYunStorageOptions> options)
    {
        this.options = options;
        string bucketName = options.Value.BucketName;
        string accessKeyId = options.Value.AccessKeyId;
        string secretAccessKey = options.Value.SecretAccessKey;
        string endPoint = options.Value.Endpoint;
        string workingDir = options.Value.WorkingDir;

        ObsConfig config = new ObsConfig() { Endpoint = endPoint };
        ObsClient = new ObsClient(accessKeyId, secretAccessKey, config);

    }

    private static string ConcatUrl(params string[] segments)
    {
        for(int i = 0; i < segments.Length; i++)
        {
            string s = segments[i];
            if (s.Contains(".."))
            {
                throw new ArgumentException("路径中不能包含..");
            }
            segments[i] = s.Trim('/');
        }
        return string.Join("/", segments);
    }

    public async Task<Uri> SaveFileAsync(string key, Stream content, CancellationToken cancellationToken = default)
    {
        if (key.StartsWith("/"))
        {
            throw new ArgumentException("Key should not start with /");
        }

        if (content.Length < 0)
        {
            throw new ArgumentException("Content is null");
        }

        string url = string.Concat(options.Value.UrlRoot, options.Value.WorkingDir, key);
        string fullPath = ConcatUrl(options.Value.WorkingDir, key);

        PutObjectRequest request = new PutObjectRequest()
        {
            BucketName = options.Value.BucketName,
            ObjectKey = key,
            InputStream = content
        };

        var res = await ObsClient.PutObjectAsync(request);
        if(res.StatusCode !=HttpStatusCode.OK)
        {
            throw new HttpRequestException("Upload Failed");
        }

        return new Uri(res.ObjectUrl);
    }

    public Task<Stream> ReadFileAsync(string url, CancellationToken cancellationToken = default)
    {
        GetObjectRequest request = new GetObjectRequest()
        {
            BucketName = options.Value.BucketName,
            ObjectKey = url
        };
        GetObjectResponse response = ObsClient.GetObject(request);

        return Task.FromResult<Stream>(response.OutputStream);
    }
}