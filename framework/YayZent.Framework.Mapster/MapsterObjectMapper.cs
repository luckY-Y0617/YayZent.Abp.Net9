using Volo.Abp.ObjectMapping;

namespace YayZent.Framework.Mapster;

// 暴露给用户的主要映射接口, IObjectMapper 通常注入到应用服务里使用。
public class MapsterObjectMapper: IObjectMapper
{
    public TDestination Map<TSource, TDestination>(TSource source)
    {
        throw new NotImplementedException();
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        throw new NotImplementedException();
    }

    public IAutoObjectMappingProvider AutoObjectMappingProvider => throw new NotImplementedException();
}