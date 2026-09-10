namespace Kida.Models;

public sealed record MfaDomainRule(
    Guid RuleId,
    string Domain,
    string Requirement,
    bool IncludeSubdomains = true,
    int Priority = 0);
