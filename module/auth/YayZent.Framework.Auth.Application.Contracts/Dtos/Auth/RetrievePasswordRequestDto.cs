namespace YayZent.Framework.Auth.Application.Contracts.Dtos.Auth;

public class RetrievePasswordRequestDto
{
    /// <summary>
    /// 密码
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 唯一标识码
    /// </summary>
    public string? Uuid { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    public string? Code { get; set; }
    
    public string? OldPassword { get; set; }
}