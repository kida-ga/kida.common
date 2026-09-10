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

Create the four distribution packages with:

```powershell
.\pack.ps1 -Version 0.1.0
```

Packages are written to `.artifacts\packages`. Publish Haley package versions first,
then `Kida.Contracts`, `Kida.Service.Client`, `Kida.ResourceServer`, and
`Kida.AuthEdge` in that order.

## Public boundary

This repository intentionally excludes Kida Service/Admin hosts, capability Kits,
DAL implementations, database schemas, migrations, management-control contracts,
private keys, client secrets, appsettings, and operational policy files. Public
source is an interoperability boundary, not an authorization boundary: Kida still
enforces signature, audience, token use, scope, tenant, and subject authorization at
runtime.
