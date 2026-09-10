using Haley.Abstractions;

namespace Kida.Models;
public sealed record UpdateUserProfileRequest(string DisplayName, string? GivenName = null, string? FamilyName = null, string? PreferredName = null, string? Locale = null, string? TimeZone = null, string? AvatarUri = null);
