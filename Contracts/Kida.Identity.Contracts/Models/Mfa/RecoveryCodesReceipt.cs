using Haley.Abstractions;

namespace Kida.Models;
public sealed record RecoveryCodesReceipt(IReadOnlyCollection<string> Codes);
