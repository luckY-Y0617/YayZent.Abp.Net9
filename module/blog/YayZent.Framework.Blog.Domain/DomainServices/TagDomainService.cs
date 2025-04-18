using OBS.Model;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Users;
using YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Blog.Domain.Repositories;

namespace YayZent.Framework.Blog.Domain.DomainServices;

public class TagDomainService: DomainService, ITagDomainService
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentUser _currentUser;
    private readonly ITagRepository _tagRepository;

    public TagDomainService(IGuidGenerator guidGenerator, ICurrentUser currentUser, ITagRepository tagRepository)
    {
        _guidGenerator = guidGenerator;
        _currentUser = currentUser;
        _tagRepository = tagRepository;
    }

    public async Task<List<TagAggregateRoot>?> GetTagListByIdsAsync(List<Guid>? tagIds)
    {
        if (tagIds == null)
        {
            return null;
        }
        
        var tagAggregateRoots = await _tagRepository.DbQueryable
            .Where(x => tagIds.Contains(x.Id))
            .ToListAsync();
        
        return tagAggregateRoots;
    }
    
    public async Task<List<Guid>?> CreateTagsAsync(List<string>? tags)
    {
        if (tags == null || tags.Count == 0)
        {
            return null;
        }

        // 查询数据库中已存在的标签
        var existingTagNames = await _tagRepository.GetExistingTagNamesAsync(tags);

        // 过滤出数据库中不存在的 Tag
        var newTagEntities = tags
            .Where(t => !existingTagNames.Contains(t)) // 这里正确地匹配字符串
            .Select(t => new TagAggregateRoot { TagName = t }) // 转换成实体
            .ToList();

        // 批量插入新标签
        if (newTagEntities.Count > 0)
        {
            await _tagRepository.InsertManyAsync(newTagEntities);
        }

        // 返回所有的标签（新插入的 + 旧的）
        var tagIds = await _tagRepository.DbQueryable
            .Where(x => tags.Contains(x.TagName))
            .Select(x => x.Id)
            .ToListAsync();

        return tagIds;
    }
}