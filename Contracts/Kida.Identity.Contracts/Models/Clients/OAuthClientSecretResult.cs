using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Models;
public sealed record OAuthClientSecretResult(Guid ClientId, string ClientSecret, string SecretHint, DateTimeOffset CreatedAt, DateTimeOffset? ExpiresAt);
