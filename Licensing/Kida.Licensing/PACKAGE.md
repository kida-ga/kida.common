# Kida.Licensing

Use this package in an ASP.NET Core product host to validate its Kida-issued deployment license. The process
comes from Haley's Licensing utils (`LicenseRuntime`, `LicenseSnapshot`, `LicenseStatus`); the product supplies
only its identity and catalogs.

```csharp
services.AddKidaLicensing(
    configuration,
    new KidaLicenseProduct("plaintrack", ProductFeatures.All, [ProductLimits.TenantMaximum]));
```

- `appinfo.json` (content root) supplies the product, version, features and limits. It must match the compiled
  `KidaLicenseProduct`.
- Kida owns the common defaults: `.deployinfo/license.lic`, its trusted public-key registry, a 14-day trial,
  a 30-day expiry warning and seven-day evidence recovery. Hosts do not repeat these values.
- Trial configuration is capped at 90 days. Signed grant grace is capped at 45 days by both issuance and runtime
  evaluation, including grants created by older tooling.
- The optional `Kida:Licensing` section can override common defaults for an exceptional deployment. The top-level
  `deployinfo-location` setting selects the deployment-information directory.
- Startup prepares the deployment request, evaluates `license.lic` and `features.fea`, and fails when the grant is
  unusable.

Inject `IKidaLicenseService` to check `HasFeature`, `TryGetLimit` or `GetStatus`. Checks are answered from memory
against the current time: trial, valid, expiring, grace and recovery allow term features; perpetual features
remain after expiry.

Protect endpoints with `endpoint.RequireLicensedFeatures("feature")` (403, `code = license.feature_unavailable`).
Map administration onto a group the host has already secured:

```csharp
endpoints.MapGroup("/api/product/license")
    .RequireAuthorization("Product.ManageLicense")
    .MapKidaLicenseEndpoints(status => ProductLicenseDetails.From(status), "Product");
```

A license grant never creates roles or bypasses user, tenant or business authorization.
