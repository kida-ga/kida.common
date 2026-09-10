using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Models;
public sealed record OAuthClientPage(IReadOnlyCollection<OAuthClientInfo> Clients, int Page, int PageSize, long TotalCount);
