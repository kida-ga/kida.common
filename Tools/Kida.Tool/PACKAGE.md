# Kida.Tool

Kida.Tool is a standalone .NET operator tool. Do not reference it from an application project or copy it into application build and publish output.

```powershell
dotnet tool install --global Kida.Tool --version 0.1.0
kida-envelope request
kida-envelope renew
kida-envelope show
kida-envelope verify
```

The command name is `kida-envelope` on both Windows and Linux. A repository may instead install it into a local .NET tool manifest.

Normal product startup uses Haley's `DeploymentUtils.EnsureRequest`. This tool is the operator fallback for pre-provisioning, manual renewal, inspection, and recovery diagnostics when the host cannot start.

`appinfo.json` contains only `product`, `version`, the product's available `features`, and its typed `limits` catalog. Each limit advertises a description and suggested boolean, non-negative whole-number, or string default. The default guides issuance; it does not grant the value. The tool reads the top-level `deployinfo-location` setting from `appsettings.json` or `Config/appsettings.json`. When configured, that exact relative or absolute directory is created and used exclusively; otherwise `<base>/.deployinfo` is used. The deployment-information directory is deployment-specific state and must be persisted. Never send `deploy.pem`; send only the generated `<product>.request` envelope to the restricted issuer.

This package does not contain issuer private keys and cannot issue a deployment grant.
