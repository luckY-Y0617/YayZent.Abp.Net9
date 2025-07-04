using Volo.Abp.Application.Dtos;

namespace YayZent.Framework.Blog.Application.Contracts.Dtos.Category;

public class CategoryGetOutputDto: EntityDto<Guid>
{
    public Guid CategoryId { get; set; }
    
}