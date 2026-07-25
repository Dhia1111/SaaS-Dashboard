# 05 — Authentication & Authorization

## Overview

The platform implements a multi-scheme JWT authentication system with Google OAuth support. It uses a dynamic scheme selector pattern to route tokens to the correct validation handler based on the request context.

## Authentication Flow

```
                    ┌─────────────────────────┐
                    │   Client Request         │
                    │   (Authorization: Bearer)│
                    └──────────┬──────────────┘
                               │
                               ▼
                    ┌─────────────────────────┐
                    │   DynamicJwt Scheme      │
                    │   JwtSchemeSelector      │
                    └──────────┬──────────────┘
                               │
              ┌────────────────┼────────────────┐
              ▼                ▼                ▼
        ┌──────────┐   ┌──────────┐   ┌──────────┐
        │ UserJwt  │   │TenantJwt │   │    D     │
        └──────────┘   └──────────┘   └──────────┘
```

## JWT Authentication Schemes

The system registers three JWT bearer schemes with a dynamic forwarder:

### DynamicJwt Policy Scheme
- Acts as a router that delegates to the correct JWT scheme
- Uses `IJwtSchemeSelector` to inspect the request context and determine which scheme applies
- Registered as `DefaultAuthenticateScheme` and `DefaultChallengeScheme`

### UserJwt
- Intended for end-user authentication
- Validates: issuer, audience, signing key, lifetime

### TenantJwt
- Intended for tenant (business) authentication
- Validates: issuer, audience, signing key, lifetime

### D (Dashboard)
- Intended for dashboard/UI access
- Validates: issuer, audience, signing key, lifetime

All three schemes currently use **identical validation parameters** (same issuer, audience, and symmetric signing key from `JwtSettings`).

## Google OAuth

Google OAuth is configured alongside cookie authentication:

```csharp
options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
```

- Cookie name: `ExternalAuthCookie`
- SameSite mode: Lax (required for cross-port redirects)
- Correlation cookie: HttpOnly, Lax SameSite, SecurePolicy = SameAsRequest

## Token Configuration

### JWT Settings (from `JwtSettings` config section)
- `Key` — Symmetric signing key
- `Issuer` — Token issuer
- `Audience` — Token audience

### Token Handling
- `TokenHandler.cs` / `TokenHandlerService.cs` — JWT creation and validation
- `clsJwtSerivce.cs` — Business-level JWT operations
- `GenricRefreshTokenService.cs` — Refresh token workflow

## Authorization

### Permission System
- Bit-flag based authorization using `enTenantAccountManangerAutherization` enum
- Custom `RequiersdClaimAttribute` checks claims against bit-flag values
- Permissions are assigned to subscription plans dynamically

### Permission Enum
```csharp
[Flags]
enum enTenantAccountManangerAutherization
{
    // Bit-flag values for granular permission control
}
```

### Authorization Checks
- `[Authorize]` attributes on controllers
- Custom claim-based authorization for fine-grained access
- Permission loader (`PermissionsLoader.cs`) seeds permissions on startup
- `IPermissionLoader.ReloadAsync()` called during app startup

## Security Features

- Password hashing via `PasswordHashService.cs` / `clsPasswordHashService.cs`
- Email validation via `EmailValidation.cs`
- Password complexity validation via `StringComplexityValidator.cs`
- Session management through `TenantSession` and `UserSession` entities
- Domain exception hierarchy for security-related errors:
  - `AuthenticationFailedException`
  - `EmailNotVerifiedException`
  - `SessionExpiredException`
  - `SecurityBreachException`
  - `InvalidVerificationCodeException`
