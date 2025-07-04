namespace YayZent.Framework.Core.Options;

public class JwtOptions
{
    public string Issuer { get; set; } = "YayZent";
    public string Audience { get; set; } = "YayZent";
    public string SecurityKey { get; set; } = "892u4j1803qj23jroadajdoahjdao92834u23jdf923jrnmvasbceqwt347562tgdhdnsv9wevbnop";
    public long Expiration { get; set; } = 36;
}