using SqlSugar;
using Volo.Abp.Application.Services;
using YayZent.Framework.Blog.Application.Contracts.Dtos;
using YayZent.Framework.Blog.Application.Contracts.Dtos.Tag;
using YayZent.Framework.Blog.Application.Contracts.IServices;
using YayZent.Framework.Blog.Domain.DomainServices.IDomainServices;
using YayZent.Framework.Blog.Domain.Repositories;
using YayZent.Framework.Ddd.Application.Contracts.Dtos;

namespace YayZent.Framework.Blog.Application.Services;

public class TagService: ApplicationService, ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly ITagDomainService _tagDomainService;

    public TagService(ITagRepository tagRepository, ITagDomainService tagDomainService)
    {
        _tagRepository = tagRepository;
        _tagDomainService = tagDomainService;
    }

    public async Task<ApiResponse<List<Guid>>> CreateTagsAsync(CreateTagsRequest request)
    {
        var tagIds = await _tagDomainService.CreateTagsAsync(request.TagNames);
        return ApiResponse<List<Guid>>.Ok(tagIds) ;
    }
    
    public async Task<ApiResponse<GetTagsResponse>> GetTagsAsync()
    {
        var rs = await _tagRepository.DbQueryable
            .OrderBy(x => SqlFunc.GetRandom())
            .Take(6)
            .ToListAsync();
        
        return ApiResponse<GetTagsResponse>.Ok(new GetTagsResponse(){Tags = rs.Select(x => x.TagName).ToList()});
    }
}