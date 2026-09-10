# Kida.Contracts

This package supplies the public request, response, constant, service-contract, and
client-side Access policy assemblies used by trusted Kida product hosts. It contains
no database access, SQL, private signing material, host implementation, or
management-control implementation.

Third-party vendors normally do not reference this package. They call the public HTTP
API exposed by the product they integrate with.
