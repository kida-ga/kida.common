namespace Kida.ResourceServer;
public class KidaTokenValidationOptions
{
    public bool ValidateIssuer { get; set; }
    public bool ValidateAudience { get; set; } = true;
    public bool ValidateLifetime { get; set; } = true;
    public int ClockSkewSeconds { get; set; } = 60;
}
