namespace Kida.Models;

public sealed record HeartbeatLicenseRequest(Guid LeaseId, string Audience, int LeaseSeconds = 300);
