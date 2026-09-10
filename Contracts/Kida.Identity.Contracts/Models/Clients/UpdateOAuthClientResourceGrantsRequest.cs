using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Models;
public sealed record UpdateOAuthClientResourceGrantsRequest(IReadOnlyCollection<OAuthClientResourceGrant> ResourceGrants);
