# Kida Common

Kida Common is the public, reusable integration surface for trusted product hosts.
It contains contracts and libraries needed to call Kida or validate Kida-issued
tokens without exposing Kida's private hosts, data access, SQL, signing logic,
management implementation, or deployment configuration.

## Repository layout

| Folder | Package | Purpose |
| --- | --- | --- |
| `Contracts` | `Kida.Contracts` | Public Identity, Access, and Tenancy DTOs, constants, interfaces, and the client-side Access evaluator |
| `Clients` | `Kida.Service.Client` | Haley-based server-to-Kida client for trusted product hosts |
| `ResourceServer` | `Kida.ResourceServer` | Local validation of Kida-signed tokens and authorization data |
| `AuthEdge` | `Kida.AuthEdge` | Narrow authentication endpoints embedded into a product host |
| `Tools` | `Kida.Tool` | Safe `kida-envelope` command for preparing deployment requests without starting a host |

The source contract projects under `Contracts` are bundled into the single
`Kida.Contracts` NuGet package. They are not published as separate packages.

Third-party vendors normally do not need these libraries. They call the public API
of the product they integrate with. Client credentials must remain in a trusted
server process and must never be shipped to a browser or public application.

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

## Deployment request lifecycle and fallback tool

Product startup should call Haley's `DeploymentUtils.EnsureRequest` using its validated
`appinfo.json`, before evaluating the grant. This automatically creates missing
deployment state and renews a stale request while retaining the deployment ID and
keypair. It does not silently replace corrupt, incomplete, or mismatched state.

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

The command name is `kida-envelope` on both Windows and Linux.

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
