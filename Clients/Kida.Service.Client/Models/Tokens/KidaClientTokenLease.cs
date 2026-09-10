using Haley.Abstractions;
using Haley.Models;
using Haley.Utils;
using Kida.Abstractions;
using Kida.Constants;
using Kida.Models;
using Kida.Utils;
using Microsoft.Extensions.Options;

namespace Kida.Service.Client;
public sealed record KidaClientTokenLease(string AccessToken, DateTimeOffset ExpiresAt, IReadOnlySet<string> Scopes);
