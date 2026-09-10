using System.Net;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Haley.Abstractions;
using Haley.Rest;
using Haley.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Kida.ResourceServer;
internal sealed record KidaJsonWebKeySet([property: JsonPropertyName("keys")] IReadOnlyCollection<KidaJsonWebKey> Keys);
