using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Blog.Domain.Repositories;

public interface ITagRepository : ISqlSugarRepository<TagAggregateRoot>
{
    Task<List<TagAggregateRoot>?> AddOrUpdateTagsAsync(List<string> tags);

    Task<List<string>> GetExistingTagNamesAsync(List<string> tagNames);
}