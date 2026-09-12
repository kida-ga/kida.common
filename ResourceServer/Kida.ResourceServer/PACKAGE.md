# Kida.ResourceServer

Use this package in an ASP.NET Core product host to validate Kida-signed tokens locally
from the configured JWKS source, audience, lifetime, token use, and authorization data.

`KidaResourceAuthorization.HasScope` accepts a concrete scope normally. It treats `*`
as all scopes only when the signed principal also contains `scope_mode=all`. This
marker remains confined by the token's exact audience and does not replace current
client-grant, subject-entitlement, tenant/resource, or product business checks.
