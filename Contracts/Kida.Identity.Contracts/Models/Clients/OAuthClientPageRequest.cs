using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Models;
public sealed record OAuthClientPageRequest(int Page = 1, int PageSize = 20);
