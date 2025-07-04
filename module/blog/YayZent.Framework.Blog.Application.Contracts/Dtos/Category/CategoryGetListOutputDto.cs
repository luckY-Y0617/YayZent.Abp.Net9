using Volo.Abp.Application.Dtos;

namespace YayZent.Framework.Blog.Application.Contracts.Dtos.Category;

public class CategoryGetListOutputDto: EntityDto<Guid>
{
    public string CategoryName { get; set; } = string.Empty;
}