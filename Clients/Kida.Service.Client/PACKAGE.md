# Kida.Service.Client

Use this Haley-based client inside a trusted server-side product host that must call
Kida Service. Client identifiers and secrets must remain on the server and must never
be shipped to a browser or third-party application.

Call `AddKidaClient` once for machine-mediated Kida operations. Product Hosts should
also call `AddKidaProductCatalog`; it reads `appinfo.json` through Haley, binds the
stable deployment UUID to an immutable product release, and publishes features and
typed limits at startup. `IKidaProductCatalogStatus.Receipt` exposes the resulting
product and release UUIDs for exact-release entitlement evaluation.

The reusable client also exposes product-host plan and subscription operations.
They are audience-bound by Kida Service and require the explicit
`entitlements.plans.manage` or `entitlements.subscriptions.manage` machine scope.
