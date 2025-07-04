using Volo.Abp.Application.Dtos;

namespace YayZent.Framework.Blog.Application.Contracts.Dtos.Tag;

public class TagGetListOutputDto: EntityDto<Guid>
{
    public string TagName { get; set; } = string.Empty;
}