using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Models;
public sealed record RotateOAuthClientSecretRequest(bool RevokeExisting = true, DateTimeOffset? ExpiresAt = null);
