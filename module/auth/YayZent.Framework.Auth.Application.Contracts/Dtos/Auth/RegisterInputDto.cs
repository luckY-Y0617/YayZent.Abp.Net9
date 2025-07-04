namespace YayZent.Framework.Auth.Application.Contracts.Dtos.Auth;

public class RegisterInputDto
{
    //电话号码，根据code的表示来获取

    /// <summary>
    /// 账号
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    public string Code { get; set; }
}