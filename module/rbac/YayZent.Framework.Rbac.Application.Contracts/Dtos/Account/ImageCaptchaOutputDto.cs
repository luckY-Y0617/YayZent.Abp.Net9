namespace YayZent.Framework.Rbac.Application.Contracts.Dtos.Account;

public class ImageCaptchaOutputDto
{
    public Guid Uuid { get; set; } = Guid.Empty;
    
    public byte[] Img { get; set; }

    public bool IsEnableCaptcha {  get; set; }
}

