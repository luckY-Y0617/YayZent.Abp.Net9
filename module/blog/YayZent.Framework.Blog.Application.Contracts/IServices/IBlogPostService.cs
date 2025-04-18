using Volo.Abp.Application.Services;
using YayZent.Framework.Blog.Application.Contracts.Dtos;
using YayZent.Framework.Blog.Application.Contracts.Dtos.BlogPost;
using YayZent.Framework.Ddd.Application.Contracts.Dtos;

namespace YayZent.Framework.Blog.Application.Contracts.IServices;

public interface IBlogPostService: IApplicationService
{
    Task<ApiResponse<CreateBlogPostResponse>> CreateAsync(CreateBlogPostRequest input);

    Task<ApiResponse<GetBlogPostDetailResponse>> GetBlogPostContentAsync(GetBlogPostDetailRequest request);
}