using YayZent.Framework.Blog.Application.Contracts.Dtos.Tag;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Ddd.Application;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Blog.Application.Services;

public class TagService: CustomCrudAppService<TagAggregateRoot, TagGetOutputDto, TagGetListOutputDto, Guid, TagGetListInputDto, TagCreateDto, TagUpdateDto>
{
    private readonly ISqlSugarRepository<TagAggregateRoot> _tagRepository;
    public TagService(ISqlSugarRepository<TagAggregateRoot, Guid> repository) : base(repository)
    {
        _tagRepository = repository;
    }
}

