using Haley.Abstractions;

namespace Kida.Models;
public sealed record ProvisionedUser(UserIdentity Identity, bool Created);
