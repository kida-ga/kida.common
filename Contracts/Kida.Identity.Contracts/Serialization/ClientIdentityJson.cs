using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Haley.Models;

namespace Kida.Utils;

/// <summary>Preserves legacy Kida client field names without duplicating shared identity models.</summary>
public static class ClientIdentityJson
{
    public static JsonSerializerOptions CreateOptions(JsonSerializerOptions defaults)
    {
        var options = new JsonSerializerOptions(defaults);
        options.TypeInfoResolver = (options.TypeInfoResolver ?? new DefaultJsonTypeInfoResolver())
            .WithAddedModifier(info =>
            {
                if (info.Type != typeof(UserLoginAttemptInfo)) return;
                foreach (var property in info.Properties)
                {
                    if (property.AttributeProvider is MemberInfo { Name: nameof(UserLoginAttemptInfo.ApplicationId) })
                        property.Name = options.PropertyNamingPolicy?.ConvertName("ClientId") ?? "ClientId";
                }
            });
        return options;
    }
}
