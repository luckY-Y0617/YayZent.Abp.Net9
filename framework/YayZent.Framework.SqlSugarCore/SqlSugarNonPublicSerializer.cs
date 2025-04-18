using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SqlSugar;
using Volo.Abp.DependencyInjection;

namespace YayZent.Framework.SqlSugarCore;

public class SqlSugarNonPublicSerializer : ISerializeService, ITransientDependency
{
    private readonly JsonSerializerSettings _jsonSettings;

    public SqlSugarNonPublicSerializer(IOptions<JsonSerializerSettings> jsonSettings)
    {
        _jsonSettings = jsonSettings.Value;
    }

    // 3. 序列化方法
    public string SerializeObject(object? value)
    {
        if (value == null) return string.Empty;
        return JsonConvert.SerializeObject(value, _jsonSettings); // 序列化时，私有属性也会参与
    }

    // 4. 反序列化方法
    public T? DeserializeObject<T>(string value)
    {
        if (string.IsNullOrEmpty(value)) return default;
        return JsonConvert.DeserializeObject<T>(value, _jsonSettings); // 反序列化时，私有属性也会被赋值
    }

    // 5. SqlSugar 序列化方法（与原来保持一致）
    public string SugarSerializeObject(object value)
    {
        return SerializeObject(value);
    }
}