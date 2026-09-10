using Haley.Abstractions;

namespace Kida.Models;
public sealed record CreateLocalUserRequest(string Username, string DisplayName, string Password, bool ActivateImmediately = false, bool RequirePasswordChange = true);
