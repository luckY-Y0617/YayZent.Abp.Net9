using SqlSugar;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace YayZent.Framework.Blog.Domain.Entities
{
    [SugarTable("Comment")]
    public class CommentAggregateRoot : AuditedAggregateRoot<Guid>, ISoftDelete
    {
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; } = string.Empty;
        
        /// <summary>
        /// 博客Id
        /// </summary>
        public Guid BlogPostId { get; set; }
        
        /// <summary>
        /// 父评论Id
        /// </summary>
        public Guid ParentCommentId { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public int Likes { get; set; } = 0;
        
        public bool IsDeleted { get; private set; } = false;

        [Navigate(NavigateType.ManyToOne, nameof(BlogPostId), nameof(BlogPostAggregateRoot.Id))]
        public BlogPostAggregateRoot? BlogPost { get; set; }

        protected CommentAggregateRoot() {}

        public CommentAggregateRoot(Guid id, string content) : base(id)
        {
            Content = content;
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
