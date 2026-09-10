using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Models;
public sealed record OAuthClientResourceGrant(string Audience, IReadOnlyCollection<string> AllowedScopes);
