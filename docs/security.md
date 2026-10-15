# JobyAI security

This document records the security boundaries to implement as API features are
added. The current project is a structural scaffold and does not yet provide
authenticated API features.

- Keep credentials and local configuration out of source control.
- Validate and authorize every API request before invoking tools.
- Require explicit approval for sensitive or externally visible tool actions.
- Limit file and Gmail tool access to the minimum required permissions.
- Do not expose provider credentials, internal exceptions, or sensitive data in
  API responses or logs.
- Configure and test a restrictive Content Security Policy before enabling
  desktop web content beyond the local frontend.
