using Volo.Abp.Application.Dtos;

namespace YayZent.Framework.Blog.Application.Contracts.Dtos.Tag;

public class TagGetOutputDto: EntityDto<Guid>
{
    public string TagName { get; set; }
}