using OBS;
using OBS.Model;

namespace YayZent.Framework.Core.File.Extensions;

public static class HuaWeiYunExtension
{
    public static Task<PutObjectResponse> PutObjectAsync(this ObsClient client, PutObjectRequest request)
    {
        var tcs = new TaskCompletionSource<PutObjectResponse>();

        client.BeginPutObject(request, ar =>
        {
            try
            {
                var response = client.EndPutObject(ar);
                tcs.TrySetResult(response);
            }
            catch (ObsException ex)
            {
                tcs.SetException(new Exception($"ErrorCode: {ex.ErrorCode}, ErrorMessage: {ex.ErrorMessage}"));
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }, null);
        return tcs.Task;
    }
}