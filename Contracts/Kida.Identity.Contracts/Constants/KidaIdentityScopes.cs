using Haley.Abstractions;
using System.Text.Json.Serialization;

namespace Kida.Constants;
public static class KidaIdentityScopes
{
    public const string Authenticate = "identity.authenticate";
    public const string SessionsRevoke = "identity.sessions.revoke";
    public const string PasswordResetRequest = "identity.password.reset.request";
    public const string UsersInvite = "identity.users.invite";
    public const string UsersBootstrap = "identity.users.bootstrap";
    public const string UsersMigrate = "identity.users.migrate";
    public const string UsersCreate = "identity.users.create";
    public const string UsersResolve = "identity.users.resolve";
    public const string UsersManage = "identity.users.manage";
    public const string FederationExchange = "identity.federation.exchange";
    public const string ClientsManage = "identity.clients.manage";
    public const string Administer = "identity.administer";
}
