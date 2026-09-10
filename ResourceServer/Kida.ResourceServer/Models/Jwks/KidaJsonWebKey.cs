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
internal sealed record KidaJsonWebKey([property: JsonPropertyName("kid")] string Kid, [property: JsonPropertyName("kty")] string Kty, [property: JsonPropertyName("use")] string Use, [property: JsonPropertyName("alg")] string Alg, [property: JsonPropertyName("n")] string N, [property: JsonPropertyName("e")] string E);
