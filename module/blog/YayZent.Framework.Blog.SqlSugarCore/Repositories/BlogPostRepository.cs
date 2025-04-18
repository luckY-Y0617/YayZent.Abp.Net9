using Volo.Abp.DependencyInjection;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Blog.Domain.Repositories;
using YayZent.Framework.SqlSugarCore.Abstractions;
using YayZent.Framework.SqlSugarCore.Repositories;

namespace YayZent.Framework.Blog.SqlSugarCore.Repositories;

public class BlogPostRepository: SqlSugarRepository<BlogPostAggregateRoot, Guid>, IBlogPostRepository, ITransientDependency
{
    public BlogPostRepository(ISugarDbContextProvider<ISqlSugarDbContext> dbContextProvider) : base(dbContextProvider){}
    
    
}