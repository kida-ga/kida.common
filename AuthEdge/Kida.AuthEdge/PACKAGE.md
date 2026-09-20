# Kida.AuthEdge

Use this package to expose the deliberately limited Kida authentication edge through a
trusted product host. It does not expose the complete Kida Service API.

Authentication failures preserve the upstream HTTP status, error code, diagnostic
detail and trace ID. The included password pages display returned details when
available. Unexpected endpoint failures are logged through the product host's
logger and return HTTP 500; connection failures return 503 and timeouts return 504.
Raw exception messages and stack traces stay in server logs.
