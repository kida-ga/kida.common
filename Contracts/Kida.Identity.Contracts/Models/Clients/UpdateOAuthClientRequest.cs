using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Models;
public sealed record UpdateOAuthClientRequest(string ClientIdentifier, string DisplayName, Guid? OwnerTenantId = null, Guid? ConnectedAppId = null, uint AccessTokenSeconds = 300, IReadOnlyCollection<string>? RedirectUris = null, string? ManagedAudience = null, string? HostedAudience = null);
