using Haley.Abstractions;

namespace Kida.Models;
public sealed record FederatedIdentityResult(UserIdentity Identity, bool Created, bool Linked, bool PasswordEnrollmentRequired, AuthenticationResult Session);
