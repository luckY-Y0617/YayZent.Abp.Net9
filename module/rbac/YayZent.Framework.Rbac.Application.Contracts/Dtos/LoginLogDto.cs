namespace YayZent.Framework.Rbac.Application.Contracts.Dtos;

public class LoginLogDto
{
    public string LoginUser { get; set; }
    public string Browser { get; set; }
    public string Os { get; set; }
    public string LoginIp { get; set; }
    public string LoginLocation { get; set; }
}