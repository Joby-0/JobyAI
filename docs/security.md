# JobyAI security

This document describes security boundaries for implementation. For reporting
a vulnerability in the repository, follow the GitHub-recognized policy in
[SECURITY.md](../SECURITY.md).

- Keep credentials and local configuration out of source control.
- Validate and authorize every API request before invoking tools.
- Require explicit approval for sensitive or externally visible tool actions.
- Limit file and Gmail tool access to the minimum required permissions.
- Do not expose provider credentials, internal exceptions, or sensitive data in
  API responses or logs.
- Configure and test a restrictive Content Security Policy before enabling
  desktop web content beyond the local frontend.

The current API does not yet provide authentication or authorization, and the
listed boundaries are requirements for features as they are implemented, not
claims that those protections are already active.
