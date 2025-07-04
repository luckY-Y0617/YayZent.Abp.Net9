using System.Text.RegularExpressions;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using YayZent.Framework.Core.Helper;
using YayZent.Framework.Core.Interfaces;
using YayZent.Framework.Rbac.Domain.Entities.ValueObjects;
using YayZent.Framework.Rbac.Domain.Shared.Consts;
using YayZent.Framework.Rbac.Domain.Shared.Enums;

namespace YayZent.Framework.Rbac.Domain.Entities;

[SugarTable("User")]
public class UserAggregateRoot: AuditedAggregateRoot<Guid>, ISoftDelete, IOrderNum, IState
{
    
    /// <summary>
    /// 用户名（唯一）
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    
    /// <summary>
    /// 年龄
    /// </summary>
    public int? Age { get; set; }

    /// <summary>
    /// 加密密码
    /// </summary>
    [SugarColumn(IsOwnsOne = true)]
    public EncryPasswordValueObject EncryPassword { get; set; } = new EncryPasswordValueObject();
    
    /// <summary>
    /// 邮箱(注册时必须验证邮箱，不可为空)
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// 手机号(注册时必须验证手机号，不可为空)
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// IP地址
    /// </summary>
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// 介绍
    /// </summary>
    public string? Introduction { get; set; }
    
    /// <summary>
    /// 性别
    /// </summary>
    public GenderEnum Gender { get; set; } = GenderEnum.Unknown;
    
    /// <summary>
    /// 软删除
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 排序
    /// </summary>
    public int OrderNum { get; set; } = 0;
    
    /// <summary>
    /// 状态
    /// </summary>
    public bool State { get; set; }
    
    [Navigate(typeof(UserRoleEntity), nameof(UserRoleEntity.UserId), nameof(UserRoleEntity.RoleId))]
    public List<RoleAggregateRoot>? Roles { get; set; }
      
    [Navigate(typeof(UserPostEntity), nameof(UserPostEntity.UserId), nameof(UserPostEntity.PostId))]
    public List<PostAggregateRoot>? Posts { get; set; }
    
    public UserAggregateRoot() {}

    public static UserAggregateRoot RegisterByPhone(string userName, string password, string phone)
    {
        var user = new UserAggregateRoot();
        user.Initialize(userName, password);
        user.Phone = phone;
        return user;
    }

    public static UserAggregateRoot RegisterByEmail(string userName, string password, string email)
    {
        var user = new UserAggregateRoot();
        user.Initialize(userName, password);
        user.Email = email;
        return user;
    }

    private void Initialize( string userName, string password)
    {
        var (hash, salt) = PasswordSecurityHelper.HashPassword(password);

        UserName = userName;
        EncryPassword = new EncryPasswordValueObject(hash, salt);
        State = true;
    }
    

    private static void ValidateUserNameFormat(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new UserFriendlyException("Name is required");
        }

        if (userName == UserConst.Admin || userName == UserConst.TenantAdmin)
        {
            throw new UserFriendlyException("用户名无效注册！");
        }

        if (userName.Length < 2)
        {
            throw new UserFriendlyException("账号名需大于等于2位！");
        }

        if (!Regex.IsMatch(userName, @"^[a-zA-Z0-9_-]+$"))
        {
            throw new UserFriendlyException("用户名不能包含除【字母】与【数字】与【_】的其他字符");
        }
    }

    private static void ValidatePasswordFormat(string password)
    {
        if (password.Length < 6)
        {
            throw new UserFriendlyException(UserConst.CreatePassworldError);
        }
    }

    public bool IsPasswordMatch(string password)
    {
        var isValid = PasswordSecurityHelper.VerifyPassword(password, EncryPassword.PasswordHash, EncryPassword.Salt);
        return isValid;
    }

    public void UpdatePassword(string newPassword, string oldPassword)
    {
        if (!IsPasswordMatch(oldPassword))
        {
            throw new UserFriendlyException(UserConst.ResetPasswordError);
        }
        
        var (hash, salt) = PasswordSecurityHelper.HashPassword(newPassword);
        
        EncryPassword = new EncryPasswordValueObject(hash, salt);
    }
}