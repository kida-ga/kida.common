using Haley.Abstractions;

namespace Kida.Models;
public enum UserActivityFilter
{
    All,
    NeverLoggedIn,
    HasLoggedIn,
    PasswordChangeRequired
}
