namespace Kida.Models;

public sealed record ReleaseLicenseRequest(Guid LeaseId, string Audience);
