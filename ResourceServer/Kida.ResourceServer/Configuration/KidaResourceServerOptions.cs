namespace Kida.ResourceServer;
public sealed class KidaResourceServerOptions : KidaTokenValidationOptions
{
    public const string DefaultSectionName = "Kida:ResourceServer";
    public const string DefaultScheme = "KidaBearer";
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public Uri? JwksUri { get; set; }
    public string[] AllowedTokenUses { get; set; } = ["client", "user"];
    public int JwksRefreshSeconds { get; set; } = 300;
}
