using System.Text.Json.Serialization;

namespace Kida.Service.Client;

[Flags]
[JsonConverter(typeof(JsonNumberEnumConverter<KidaCatalogRegistrationStatus>))]
public enum KidaCatalogRegistrationStatus : int
{
    Pending = 1,
    Registering = 2,
    Available = 4,
    Unavailable = 8,
    Disabled = 16
}
