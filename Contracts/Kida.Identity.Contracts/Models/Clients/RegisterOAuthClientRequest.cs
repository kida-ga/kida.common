using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Models;
public sealed record RegisterOAuthClientRequest(string ClientIdentifier, string DisplayName, OAuthClientType ClientType, IReadOnlyCollection<string> AllowedScopes, IReadOnlyCollection<string>? GrantTypes = null, Guid? OwnerTenantId = null, Guid? ConnectedAppId = null, uint AccessTokenSeconds = 300, IReadOnlyCollection<OAuthClientResourceGrant>? ResourceGrants = null, IReadOnlyCollection<string>? RedirectUris = null, string? ManagedAudience = null, string? HostedAudience = null);
