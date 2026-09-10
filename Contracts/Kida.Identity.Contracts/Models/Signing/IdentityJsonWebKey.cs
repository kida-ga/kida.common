using Haley.Abstractions;

namespace Kida.Models;
public sealed record IdentityJsonWebKey(string KeyId, string KeyType, string Use, string Algorithm, string Modulus, string Exponent);
