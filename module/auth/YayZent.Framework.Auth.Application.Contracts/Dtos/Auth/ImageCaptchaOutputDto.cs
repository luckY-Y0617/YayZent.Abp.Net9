namespace YayZent.Framework.Auth.Application.Contracts.Dtos.Auth;

public class ImageCaptchaOutputDto
{
    public Guid Uuid { get; set; } = Guid.Empty;
    
    public byte[] Img { get; set; }

    public bool IsEnableCaptcha {  get; set; }
}

