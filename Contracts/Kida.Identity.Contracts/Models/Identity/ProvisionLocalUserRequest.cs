using Haley.Abstractions;

namespace Kida.Models;
public sealed record ProvisionLocalUserRequest(string Username, string DisplayName, string? InitialPassword = null, bool ActivateImmediately = true, bool RequirePasswordChange = true, Guid? ClientId = null, string Resource = "");
