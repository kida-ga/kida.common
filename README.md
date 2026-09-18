# Kida Common

Kida Common is the public, reusable integration surface for trusted product hosts.
It contains contracts and libraries needed to call Kida or validate Kida-issued
tokens without exposing Kida's private hosts, data access, SQL, signing logic,
management implementation, or deployment configuration.

## Repository layout

| Folder | Package | Purpose |
| --- | --- | --- |
| `Contracts` | `Kida.Contracts` | Public Identity, Access, Tenancy, Entitlements, and Apps DTOs, constants, interfaces, and the client-side Access evaluator |
| `Clients` | `Kida.Service.Client` | Haley-based server-to-Kida client for trusted product hosts |
| `ResourceServer` | `Kida.ResourceServer` | Local validation of Kida-signed tokens and authorization data |
| `AuthEdge` | `Kida.AuthEdge` | Narrow authentication endpoints embedded into a product host |
| `Tools` | `Kida.Tool` | Safe `kida-envelope` command for preparing deployment requests without starting a host |
| `Licensing` | `Kida.Licensing` | Deployment license validation for product hosts: one registration, feature and limit checks, license administration endpoints |
| `Tests` | — | `Kida.Licensing.Tests`, run with signed test grants in temporary directories |

The source contract projects under `Contracts` are bundled into the single
`Kida.Contracts` NuGet package. They are not published as separate packages.

Third-party vendors normally do not need these libraries. They call the public API
of the product they integrate with. Client credentials must remain in a trusted
server process and must never be shipped to a browser or public application.

## Product entitlement catalog

A trusted Product Host publishes the feature and limit vocabulary already present
in its `appinfo.json`; it does not maintain a second entitlement catalog:

```csharp
services.AddKidaClient(configuration);
services.AddKidaProductCatalog(configuration, options =>
{
    options.DisplayName = "Product display name";
    options.Description = "Product-host commercial catalog.";
});
```

The registration reads the stable deployment identity through Haley, publishes
`audience + product + semantic version`, and retains the returned immutable release
UUID through `IKidaProductCatalogStatus`. Ordinary feature entries become boolean
features. Typed `appinfo.json` limits become integer, decimal, text, boolean, or JSON
features so plans can carry their values without Kida interpreting their business
meaning.

The Product Host client needs `entitlements.catalog.register`. A host that exposes
its own superadmin plan/subscription UI additionally needs
`entitlements.plans.manage` and `entitlements.subscriptions.manage`. Evaluation uses
`entitlements.evaluate` and submits the exact release UUID from catalog status.

Kida stores one stable product per `(audience, product code)`. Re-publishing the
same version and content is idempotent; changed content under the same version is
rejected. Plans are product-level, tenant access is subscription-to-plan only, and
new release features never enter an existing plan automatically.

## Build

Use the package-reference solution on a machine that does not have Haley source:

```powershell
dotnet restore Kida.Common.sln
dotnet build Kida.Common.sln --no-restore
```

Kida developers with `Kida`, `Common`, and `HaleyProject` as sibling workspaces can
use `Kida.Common_Ref.sln`. It references the approved local Haley projects and does
not silently substitute Haley NuGet packages.

```powershell
dotnet restore Kida.Common_Ref.sln
dotnet build Kida.Common_Ref.sln --no-restore
```

Both solutions include `Kida.Licensing` and its tests. The normal solution always
uses centrally pinned Haley packages, even when a Haley checkout is present.
The `_Ref` solution selects Haley source explicitly and fails if a required source
project is missing.

Direct project builds default to package mode. Pass
`-p:KidaUseHaleyPackageReferences=false` to select source mode explicitly, and
override `HaleyProject` when the source checkout is outside the usual sibling layout.
The separate `KidaUsePackageReferences` switch still controls Kida's own public
package references for release packaging.

Package mode currently requires a compatible Haley package release: the pinned
`Haley.Helpers` 2.4.14 package lacks deployment APIs used by current Kida code,
including `DeploymentUtils.EnsureRequest` and typed deployment limits. Use `_Ref`
for current source development until compatible packages are approved and pinned.
Package builds do not substitute local source to conceal this release dependency.

## Product licensing

A product host validates its deployment license with one registration and never
reimplements the evaluation:

```csharp
services.AddKidaLicensing(
    configuration,
    new KidaLicenseProduct("plaintrack", PlainTrackLicensedFeatures.All, [PlainTrackLicenseLimits.TenantMaximum]));
```

The product supplies only its identity: `appinfo.json` in the content root (product,
version, features, limits) and the same catalogs compiled into the host. Kida owns the
common path, trusted-key, trial, expiry-warning and evidence-recovery defaults. An optional
`Kida:Licensing` section exists for exceptional deployment overrides; normal products do
not repeat it. Trial overrides are capped at 90 days. Grant grace is capped at 45 days by
both Kida.Sanction and runtime evaluation.
Haley's `LicenseRuntime` prepares the deployment request, evaluates `license.lic` and
`features.fea`, and keeps a `LicenseSnapshot` in memory; startup fails when the grant is
unusable. `IKidaLicenseService` answers `HasFeature`, `TryGetLimit` and `GetStatus` from
that snapshot against the current time, so valid, expiring, grace and expiry follow the
clock without rereading files:

- trial grants every compiled feature until it ends;
- trial, valid, expiring, grace and active recovery allow term features;
- perpetual features remain after the term ends, unless the license is tampered, invalid,
  or bound to another deployment or machine;
- limits from `features.fea` remain after the term; limits carried by the license or the
  trial apply only during the term.

### Glass break

Licensing can be switched off for one deployment, in code only:

```csharp
var flags = new AppFlags().BreakGlass("Issuer unreachable during migration");
services.AddKidaLicensing(configuration, product, flags);
```

`AppFlags.GlassBreak` has no public setter, so it can never arrive from `appsettings`, an environment variable or a
configuration binder. With the glass broken no licence file is read, every compiled feature is available, limits are
not enforced, and the request, replace and reload operations are refused with `license.glass_break`. The host logs it
as critical once at startup with the stated reason. Kida's common `WarnGlassBreak` policy defaults to visible;
products decide how that status is presented.

`endpoint.RequireLicensedFeatures("feature")` refuses unlicensed calls with a 403 problem
(`code = license.feature_unavailable`). `MapKidaLicenseEndpoints` maps status, request,
replace and reload onto a route group the host has already secured, optionally projecting
the status into the product's own response contract.

## Deployment request lifecycle and fallback tool

Product startup should call Haley's `DeploymentUtils.EnsureRequest` using its validated
`appinfo.json`, before evaluating the grant. This automatically creates missing
deployment state and renews a stale request while retaining the deployment ID and
keypair. It does not silently replace corrupt, incomplete, or mismatched state.

Issuance produces two deployment-bound artifacts: a featureless `license.lic` owns the
commercial term, while the canonical `features.fea` owns term features, perpetual
features, an optional perpetual major-version cap, and typed limits. Products use
Haley's evaluation result to enforce the effective intersection. Feature updates may
reuse the signed binding inside a current license and do not require a new deployment
request.

Kida.Tool is the standalone operator fallback when the host cannot start, provisioning
must happen before startup, or a request must be inspected manually. It is not referenced
or copied by product applications. Install it independently as a global or manifest-local
.NET tool:

```xml
dotnet tool install --global Kida.Tool --version 0.1.0
```

```powershell
kida-envelope request
kida-envelope renew
kida-envelope show
kida-envelope verify
```

The command name is `kida-envelope` on both Windows and Linux. It honors the top-level `deployinfo-location` setting from the application's `appsettings.json` or `Config/appsettings.json`; a configured directory supersedes the default `<base>/.deployinfo` location.

The command reads `appinfo.json` from the current directory by default. That file
contains exactly `product`, `version`, `features`, and `limits`. A limit definition
advertises a stable code, description, and suggested primitive default:

```json
{
  "product": "sample.product",
  "version": "1.4.0",
  "features": ["documents.read", "documents.write"],
  "limits": {
    "tenant.max": { "default": 5, "description": "Maximum active tenants" },
    "support.enabled": { "default": true, "description": "Enable support integration" },
    "edition": { "default": "standard", "description": "Configured product edition" }
  }
}
```

Defaults help the issuer choose values and declare whether each value is a boolean,
non-negative whole number, or string; they do not grant anything. Kida and Haley treat
limit meanings as opaque. The generated `.deployinfo`
directory contains the random deployment identity, keypair, machine fingerprints,
and `<product>.request`; request version 4 signs the typed limit catalog and exposes fingerprints only under a generic
`proof` object as opaque Lite and Strong arrays and does not disclose hardware-source names. Persist the directory and
send only the request to the issuer. The tool
does not contain issuer keys and cannot create a grant.

Release packaging and publishing are owned by the sibling `kida.project`
repository. With `Project` and `Common` cloned at the same level, use the scripts
under `..\Project\NuGet Packaging`. This repository deliberately contains package
source only; it does not contain release credentials or publishing operations.

## Public boundary

This repository intentionally excludes Kida Service/Admin hosts, capability Kits,
DAL implementations, database schemas, migrations, management-control contracts,
private keys, client secrets, appsettings, and operational policy files. Public
source is an interoperability boundary, not an authorization boundary: Kida still
enforces signature, audience, token use, scope, tenant, and subject authorization at
runtime.
