using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Constants;
public static class KidaIdentityHeaders
{
    public const string Challenge = "X-Kida-Challenge";
    public const string ClientIdentifier = "X-Kida-Client-Identifier";
    public const string Username = "X-Kida-Username";
}
