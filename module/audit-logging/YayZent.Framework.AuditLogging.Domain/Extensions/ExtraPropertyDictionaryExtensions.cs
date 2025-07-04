using Volo.Abp.Data;
using Volo.Abp.Json;

namespace YayZent.Framework.AuditLogging.Domain.Extensions;

public static class ExtraPropertyDictionaryExtensions
{
    public static ExtraPropertyDictionary DeepClone(
        this ExtraPropertyDictionary? source,
        IJsonSerializer serializer)
    {
        var result = new ExtraPropertyDictionary();

        if (source == null || source.Count == 0)
            return result;

        var json = serializer.Serialize(source);
        var cloned = serializer.Deserialize<Dictionary<string, object>>(json);

        foreach (var kv in cloned!)
        {
            result[kv.Key] = kv.Value;
        }

        return result;
    }
}