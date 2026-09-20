using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Models;
public sealed record OAuthClientInfo(Guid ClientId, Guid? ServicePrincipalId, string ClientIdentifier, string DisplayName, OAuthClientType ClientType, IReadOnlyCollection<string> GrantTypes, IReadOnlyCollection<string> AllowedScopes, uint AccessTokenSeconds, [property: JsonConverter(typeof(JsonNumberEnumConverter<IdentityRecordStatus>))] IdentityRecordStatus Status, DateTimeOffset CreatedAt, DateTimeOffset ModifiedAt, Guid? OwnerTenantId = null, Guid? ConnectedAppId = null, IReadOnlyCollection<OAuthClientResourceGrant>? ResourceGrants = null, IReadOnlyCollection<string>? RedirectUris = null, string? ManagedAudience = null, IReadOnlyCollection<string>? HostedAudiences = null);
