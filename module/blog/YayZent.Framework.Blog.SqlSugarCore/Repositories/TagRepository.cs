using Volo.Abp.DependencyInjection;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Blog.Domain.Repositories;
using YayZent.Framework.SqlSugarCore.Abstractions;
using YayZent.Framework.SqlSugarCore.Repositories;

namespace YayZent.Framework.Blog.SqlSugarCore.Repositories;

public class TagRepository: SqlSugarRepository<TagAggregateRoot, Guid>, ITagRepository, ITransientDependency
{
    public TagRepository(ISugarDbContextProvider<ISqlSugarDbContext> dbContextProvider) : base(dbContextProvider){}

    public async Task<List<TagAggregateRoot>?> AddOrUpdateTagsAsync(List<string>? tags)
    {
        return null;
    }
    
    public async Task<List<string>> GetExistingTagNamesAsync(List<string> tagNames)
    {
        return await DbContext.Queryable<TagAggregateRoot>()
            .Where(x => tagNames.Contains(x.TagName))
            .Select(x => x.TagName)
            .ToListAsync();
    }

}   