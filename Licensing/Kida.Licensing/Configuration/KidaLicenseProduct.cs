namespace Kida.Licensing;

/// <summary>
/// The product identity and catalogs compiled into the host. <c>appinfo.json</c> must name the same product and
/// advertise exactly these features and limits, so an edited file cannot widen what the product checks.
/// </summary>
public sealed class KidaLicenseProduct
{
    public KidaLicenseProduct(string product, IReadOnlyCollection<string> features, IReadOnlyCollection<string>? limits = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(product);
        ArgumentNullException.ThrowIfNull(features);
        Product = product.Trim();
        Features = features;
        Limits = limits;
    }

    public string Product { get; }
    public IReadOnlyCollection<string> Features { get; }
    public IReadOnlyCollection<string>? Limits { get; }
}
