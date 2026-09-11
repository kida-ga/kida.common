namespace Kida.Models;

public sealed record TenantInvitationReceipt(TenantInvitationInfo Invitation, string OneTimeToken);
