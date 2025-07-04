using SqlSugar;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace YayZent.Framework.Blog.Domain.Entities
{
    [SugarTable("Comment")]
    public class CommentAggregateRoot : AuditedAggregateRoot<Guid>, ISoftDelete
    {
        /// <summary>
        /// 博客Id
        /// </summary>
        public Guid BlogPostId { get; set; }
        
        /// <summary>
        /// 父评论Id
        /// </summary>
        public Guid? ParentCommentId { get; set; }
        
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid? UserId { get; set; }
        
        /// <summary>
        /// 用户名
        /// </summary>
        public string? UserName { get; set; } = string.Empty;

        /// <summary>
        /// 回复人名
        /// </summary>
        public string? ReplyUserName { get; set; } 
        
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 点赞数
        /// </summary>
        public int Likes { get; set; } = 0;
        
        /// <summary>
        /// 软删除
        /// </summary>
        public bool IsDeleted { get; private set; } = false;

        [Navigate(NavigateType.ManyToOne, nameof(BlogPostId), nameof(BlogPostAggregateRoot.Id))]
        public BlogPostAggregateRoot? BlogPost { get; set; }

        public CommentAggregateRoot() {}

        public CommentAggregateRoot(Guid id,Guid? userId, Guid blogPostId,string? userName,string content):base(id)
        {
            UserId = userId;
            BlogPostId = blogPostId;
            Content = content;
            UserName = userName;
        }

        public CommentAggregateRoot(Guid id, Guid? userId, Guid parentCommentId, Guid blogPostId, string? userName, string content) : base(id)
        {
            Content = content;
            ParentCommentId = parentCommentId;
            BlogPostId = blogPostId;
            UserId = userId;
            UserName = userName;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }
    }
}
