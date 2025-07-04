using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Guids;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using YayZent.Framework.Auth.Domain.Shared.Authorization;
using YayZent.Framework.Blog.Application.Contracts.Dtos.Comment;
using YayZent.Framework.Blog.Domain.Entities;
using YayZent.Framework.Ddd.Application.Contracts.Dtos;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Blog.Application.Services;

public class CommentService: ApplicationService
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly ISqlSugarRepository<CommentAggregateRoot, Guid> _commentRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IObjectMapper _objectMapper;

    public CommentService(IGuidGenerator guidGenerator, ICurrentUser currentUser, IObjectMapper objectMapper,
        ISqlSugarRepository<CommentAggregateRoot, Guid> commentRepository)
    {
        _objectMapper = objectMapper;
        _currentUser = currentUser;
        _guidGenerator = guidGenerator;
        _commentRepository = commentRepository;
    }
    
    [Authorize]
    [Permission("bbs")]
    public async Task<ApiResponse<AddCommentOutputDto>> AddCommentAsync(AddCommentInputDto input)
    {
        try
        {
            EnsureUserLoggedIn();
            var creationTime = DateTime.Now;
            var commentId = _guidGenerator.Create();
            var comment = new CommentAggregateRoot(commentId, _currentUser.Id, input.BlogPostId, 
                _currentUser.UserName, input.Content);
            comment.CreationTime = creationTime;
            await _commentRepository.InsertAsync(comment);
            
            return ApiResponse<AddCommentOutputDto>.Ok(new AddCommentOutputDto
            {
                Id = commentId, 
                ParentCommentId = null,
                UserId = _currentUser.Id,
                Content = input.Content,
                UserName = _currentUser.UserName,
                CreationTime = creationTime.ToString("yyyy-MM-dd HH:mm:ss"),
            });
        }
        catch (Exception ex)
        {
            return ApiResponse<AddCommentOutputDto>.FailWithData(ex.Message);
        }
    }

    [Authorize]
    [Permission("bbs")]
    public async Task<ApiResponse<ReplyCommentOutputDto>> ReplyCommentAsync(ReplyCommentInputDto input)
    {
        try
        {
            EnsureUserLoggedIn();
            var creationTime = DateTime.Now;
            var commentId = _guidGenerator.Create();
            var comment = new CommentAggregateRoot(commentId, _currentUser.Id, input.ParentCommentId,
                input.BlogPostId, _currentUser.UserName, input.Content);
            comment.CreationTime = creationTime;
            await _commentRepository.InsertAsync(comment);
            return ApiResponse<ReplyCommentOutputDto>.Ok(new ReplyCommentOutputDto()
            {
                Id = commentId, 
                ParentCommentId = input.ParentCommentId,
                UserId = _currentUser.Id,
                Content = input.Content,
                UserName = _currentUser.UserName,
                CreationTime = creationTime.ToString("yyyy-MM-dd HH:mm:ss"),
            });
        }
        catch (Exception ex)
        {
            return ApiResponse<ReplyCommentOutputDto>.FailWithData(ex.Message);
        }
    }

    public async Task<ApiResponse<List<List<GetCommentsOutputDto>>>> GetCommentsAsync(GetCommentsInputDto input)
    {
        var allComments = await _commentRepository.DbQueryable
            .Where(x => x.BlogPostId == input.BlogPostId)
            .OrderBy(x => x.CreationTime)
            .ToListAsync();

        if (allComments.Count == 0)
            return ApiResponse<List<List<GetCommentsOutputDto>>>.Ok(new List<List<GetCommentsOutputDto>>());

        // 构建 ID 字典方便查找
        var commentDict = allComments.ToDictionary(c => c.Id);

        // 构造返回列表
        var result = new List<List<GetCommentsOutputDto>>();

        foreach (var comment in allComments)
        {
            if (comment.ParentCommentId is Guid parentId && commentDict.TryGetValue(parentId, out var parentComment))
            {
                comment.ReplyUserName = parentComment.UserName;
            }
        }

        foreach (var root in allComments.Where(x => x.ParentCommentId is null))
        {
            var thread = allComments
                .Where(x => IsDescendantOf(commentDict, x, root.Id))
                .OrderBy(x => x.CreationTime)
                .ToList();
            
            result.Add(_objectMapper.Map<List<CommentAggregateRoot>,List<GetCommentsOutputDto>>(thread));
        }

        return ApiResponse<List<List<GetCommentsOutputDto>>>.Ok(result);
    }

    private bool IsDescendantOf(Dictionary<Guid, CommentAggregateRoot> dict, CommentAggregateRoot comment, Guid rootId)
    {
        while (comment.ParentCommentId.HasValue)
        {
            // 如果直接是 rootId 的子节点
            if (comment.ParentCommentId.Value == rootId)
                return true;

            // 往上查找父级评论，直到链断或找到 rootId
            if (!dict.TryGetValue(comment.ParentCommentId.Value, out comment!))
                return false; // 数据缺失或链断，无法判断属于 root
        }

        // 如果走完整条链后，最后一个父级正好是 rootId，也算是
        return comment.Id == rootId;
    }

    private void EnsureUserLoggedIn()
    {
        if (_currentUser == null || !_currentUser.Id.HasValue)
            throw new UserFriendlyException("用户未登录");
    }
}