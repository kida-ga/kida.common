using Kida.Models;
using Microsoft.Extensions.Options;

namespace Kida.Service.Client;

internal sealed class KidaScopeCatalogStatus
    : IKidaScopeCatalogStatus
{
    private readonly object _sync = new();
    private KidaScopeCatalogStatusSnapshot _snapshot;

    public KidaScopeCatalogStatus(IOptions<KidaScopeCatalogOptions> options)
    {
        var enabled = options.Value.RegisterOnStartup;
        _snapshot = new(
            enabled,
            enabled ? "pending" : "disabled",
            null,
            null,
            null);
    }

    public KidaScopeCatalogStatusSnapshot GetSnapshot()
    {
        lock (_sync)
        {
            return _snapshot;
        }
    }

    internal void MarkAttempt(DateTimeOffset attemptedUtc)
    {
        lock (_sync)
        {
            _snapshot = _snapshot with
            {
                Status = "registering",
                LastAttemptUtc = attemptedUtc,
                FailureType = null
            };
        }
    }

    internal void MarkAvailable(DateTimeOffset succeededUtc)
    {
        lock (_sync)
        {
            _snapshot = _snapshot with
            {
                Status = "available",
                LastAttemptUtc = succeededUtc,
                LastSuccessUtc = succeededUtc,
                FailureType = null
            };
        }
    }

    internal void MarkUnavailable(DateTimeOffset attemptedUtc, Exception exception)
    {
        lock (_sync)
        {
            _snapshot = _snapshot with
            {
                Status = "unavailable",
                LastAttemptUtc = attemptedUtc,
                FailureType = exception.GetType().Name
            };
        }
    }
}
