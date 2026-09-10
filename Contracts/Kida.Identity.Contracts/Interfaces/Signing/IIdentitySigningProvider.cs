using Haley.Abstractions;

namespace Kida.Abstractions;
/// <summary>
/// Host-supplied signing boundary. Implementations should delegate to an approved external key store or HSM.
/// </summary>
public interface IIdentitySigningProvider
{
    ValueTask<SigningKeyDescriptor> GetDescriptorAsync(CancellationToken cancellationToken);
    ValueTask<byte[]> SignAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken);
}
