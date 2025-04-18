using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Volo.Abp.DependencyInjection;

namespace YayZent.Framework.Core.Serialization
{
    // 1. 自定义 ContractResolver，支持私有属性
    public class NonPublicPropertiesResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (member is System.Reflection.PropertyInfo pi)
            {
                prop.Readable = pi.GetMethod != null; // 有 Get 方法，表示可读
                prop.Writable = pi.SetMethod != null; // 有 Set 方法，表示可写
            }
            return prop;
        }
    }

    // 2. 实现 ISerializeService 接口的序列化器
    
}