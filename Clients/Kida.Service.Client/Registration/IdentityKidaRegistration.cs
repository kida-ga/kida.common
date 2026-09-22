using Haley.Abstractions;
using Haley.Extensions;
using Haley.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
namespace Kida.Service.Client;
public static class IdentityKidaRegistration
{
    public static global::Haley.Models.IdentityBuilder UseKida(this global::Haley.Models.IdentityBuilder builder, IConfiguration configuration,
        Action<KidaClientOptions>? configure = null, string sectionName = KidaClientOptions.DefaultSectionName)
    {
        builder.SelectBackend("kida");
        if (!builder.Services.Any(service => service.ServiceType == typeof(IKidaClientTokenProvider)))
            builder.Services.AddKidaClient(configuration, sectionName, configure);
        else if (configure is not null) builder.Services.Configure(configure);
        global::Haley.Extensions.IdentityRemoteRegistration.RegisterRemote(builder.Services);
        builder.Services.AddOptions<global::Haley.Models.IdentityOptions>().Configure<IOptions<KidaClientOptions>>((options, kida) =>
        {
            options.ApplicationId = kida.Value.ClientId;
            options.Url = kida.Value.Url;
            options.ApiPath = "kida/identity/foundation";
            options.SessionKeyId = string.Empty;
            options.SessionBindingSecret = string.Empty;
        });
        builder.Services.Replace(ServiceDescriptor.Singleton<global::Haley.Abstractions.IIdentityRemoteAuthentication, IdentityKidaAuthentication>());
        return builder;
    }
}
