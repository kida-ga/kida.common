using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Constants;
public static class KidaClaimTypes
{
    public const string ClientId = "client_id";
    public const string ClientIdentifier = "client_identifier";
    public const string AuthorizedParty = "azp";
    public const string Scope = "scope";
    public const string TokenUse = "token_use";
    public const string OwnerTenantId = "owner_tenant_id";
}
