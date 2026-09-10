using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Models;
public sealed record OAuthClientRegistrationResult(OAuthClientInfo Client, string? ClientSecret, string? SecretHint);
