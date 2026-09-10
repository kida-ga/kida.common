namespace Kida.Models;

public sealed class KidaAuthEdgeOptions
{
    public const string SectionName = "Kida:AuthEdge";

    public string RoutePrefix { get; set; } = "/api/auth";
    public string BrowserRoutePrefix { get; set; } = "/auth";
    public int MinimumPasswordLength { get; set; } = 9;
    public bool MapClientTokenExchange { get; set; } = true;
    public bool MapPasswordCeremonies { get; set; } = true;
    public bool MapOnboardingCeremonies { get; set; } = true;
    public bool MapMfaCeremonies { get; set; } = true;
    public bool MapSamlCeremonies { get; set; } = true;

    /// <summary>
    /// Raw Kida access and refresh tokens are returned when enabled. Product browser
    /// applications should normally keep this false and expose their own HTTP-only-cookie BFF session.
    /// </summary>
    public bool MapRawSessionEndpoints { get; set; }
}
