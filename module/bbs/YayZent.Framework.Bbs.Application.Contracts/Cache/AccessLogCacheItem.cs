namespace YayZent.Framework.Bbs.Application.Contracts.Cache;

public class AccessLogCacheItem
{
    public AccessLogCacheItem(long number)
    {
        Number = number;
    }

    public long Number { get; set; }
    public DateTime LastModificationTime { get; set; }=DateTime.Now;
    
    public DateTime LastInsertTime { get; set; }=DateTime.Now;
}