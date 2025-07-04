using Volo.Abp.Application.Services;

namespace YayZent.Framework.Ddd.Application.Contracts;


public interface ICustomCrudAppService<TEntityDto, in TKey> : ICrudAppService<TEntityDto, TKey>
{
}

public interface ICustomCrudAppService<TEntityDto, in TKey, in TGetListInput> : ICrudAppService<TEntityDto, TKey, TGetListInput>
{
}

public interface ICustomCrudAppService<TEntityDto, in TKey, in TGetListInput, in TCreateInput> : ICrudAppService<TEntityDto, TKey, TGetListInput, TCreateInput>
{
}

public interface ICustomCrudAppService<TEntityDto, in TKey, in TGetListInput, in TCreateInput, in TUpdateInput> : ICrudAppService<TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
{
}

public interface ICustomCrudAppService<TGetOutputDto, TGetListOutputDto, in TKey, in TGetListInput, in TCreateInput, in TUpdateInput> : ICrudAppService<TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
{

}