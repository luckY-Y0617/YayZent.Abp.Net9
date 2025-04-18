using SqlSugar;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Blog.Domain.Repositories;

public interface IBlogPostRepository : ISqlSugarRepository<BlogPostAggregateRoot, Guid>
{
    
}