using Haley.Abstractions;

namespace Kida.Models;
/// <summary>
/// Validated assertion data accepted only across Kida's private Auth Edge to
/// Service boundary. Subject is the immutable provider subject, never email.
/// </summary>
public sealed record ExchangeFederatedIdentityRequest(Guid ClientId, string Resource, string ProviderCode, string Subject, string? Email, string DisplayName, string ClaimsPayload, string AssertionId, DateTimeOffset AuthenticatedAt, string AuthenticationMethod = "saml");
