namespace Kida.Abstractions;
public interface IKidaScopeParameterResolver
{
    string Source { get; }

    ValueTask<IReadOnlyCollection<KidaScopeParameterOption>> ListOptionsAsync(CancellationToken cancellationToken = default);
}
