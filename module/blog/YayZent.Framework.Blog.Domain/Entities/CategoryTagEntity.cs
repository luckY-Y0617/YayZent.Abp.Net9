using Volo.Abp.Domain.Entities;

namespace YayZent.Framework.Blog.Domain.Entities;

public class CategoryTagEntity: Entity<Guid>
{
    public Guid CategoryId { get; set; }
    
    public Guid TagId { get; set; }

    public CategoryTagEntity(){}
    
    public CategoryTagEntity(Guid id, Guid categoryId, Guid tagId):base(id)
    {
        CategoryId = categoryId;
        TagId = tagId;
    }
}