using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace YayZent.Framework.Core.Helper;

public static class JsonHelper
{
    public static T ParseJson<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON string is null or empty.", nameof(json));

        var result = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result == null)
            throw new JsonException($"Failed to deserialize JSON to type {typeof(T).FullName}");

        return result;
    }

    public static string SerializeWithDateFormat<T>(T obj, string dateTimeFormat)
    {
        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter()
        {
            DateTimeFormat = dateTimeFormat
        };
        return JsonConvert.SerializeObject(obj, Formatting.Indented, timeConverter);
    }
}