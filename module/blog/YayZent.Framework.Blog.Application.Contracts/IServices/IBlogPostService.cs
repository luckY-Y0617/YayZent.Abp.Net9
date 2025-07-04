using Volo.Abp.Application.Services;
using YayZent.Framework.Blog.Application.Contracts.Dtos;
using YayZent.Framework.Blog.Application.Contracts.Dtos.BlogPost;
using YayZent.Framework.Ddd.Application.Contracts.Dtos;

namespace YayZent.Framework.Blog.Application.Contracts.IServices;

public interface IBlogPostService: IApplicationService
{
    Task<ApiResponse<CreateBlogPostOutputDto>> CreateAsync(CreateBlogPostInputDto input);

    Task<ApiResponse<GetBlogPostDetailOutputDto>> GetBlogPostDetailAsync(GetBlogPostDetailInputDto input);

    Task<ApiResponse<List<GetSortedBlogPostsOutputDto>?>> GetSortedBlogPostsAsync(
        GetSortedBlogPostsInputDto input);
}