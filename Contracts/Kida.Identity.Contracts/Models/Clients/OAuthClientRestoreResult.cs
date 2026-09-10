using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Models;
public sealed record OAuthClientRestoreResult(Guid ClientId, string? ClientSecret, string? SecretHint, DateTimeOffset RestoredAt);
