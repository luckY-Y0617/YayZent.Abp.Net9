namespace YayZent.Framework.Core.File.Helpers;

public static class ObsPathHelper
{
    /// <summary>
    /// 从 OBS 访问 URL 提取对象键（ObjectKey）
    /// </summary>
    /// <param name="url">公开访问 URL</param>
    /// <returns>对象键</returns>
    public static string GetObjectKeyFromUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL 不能为空");

        Uri uri = new Uri(url);
        return uri.AbsolutePath.TrimStart('/');
    }
}