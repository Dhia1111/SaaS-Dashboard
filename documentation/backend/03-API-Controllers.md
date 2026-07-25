# 06 — API Controllers & Routes

## Controller Overview

| Controller | Base Route | Auth | Purpose |
|---|---|---|---|
| Platform | `/api/Platform` | Yes | Subscription management, pricing, checkout |
| Tenant | `/api/Tenant` | Yes | Tenant info, client subscriptions |
| TenantAuth | `/api/TenantAuth` | No | Tenant registration, login, verification |
| UserAuth | `/api/UserAuth` | No | User registration, login, invitation |
| User | `/api/User` | Yes | User CRUD, roles, permissions |
| Subscriptions | `/api/Subscriptions` | Yes | Subscription plan CRUD |
| Permissions | `/api/Permissions` | Yes | Permission CRUD |
| TenantPricingCycle | `/api/TenantPricingCycle` | Yes | Pricing cycle CRUD |
| EmployeesManagment | `/api/EmployeesManagment` | Yes | Employee management |
| BusinessAnalyses | `/api/BusinessAnalyses` | Yes | Analytics data |
| PaymentWebHooks | `/api/PaymentWebHooks` | No* | Stripe webhook events |
| CentrelaziedAuthentication | `/api/CentrelaziedAuthentication` | No | Google OAuth callback |
| test | `/api/test` | No | Health check / test |

## Platform Controller

| Method | Route | Description |
|---|---|---|
| GetSubscriptionOptions | `GET /api/Platform/subscription-options` | Available subscription plans |
| GetPricingCycles | `GET /api/Platform/pricing-cycles` | Configurable billing cycles |
| Subscribe | `POST /api/Platform/subscribe` | Create new subscription |
| Upgrade | `POST /api/Platform/upgrade` | Upgrade existing plan |
| GetActiveSubscription | `GET /api/Platform/active-subscription` | Current active subscription |
| GetDiscoveryPlatforms | `GET /api/Platform/discovery` | Marketing discovery options |
| GetPaymentProviders | `GET /api/Platform/payment-providers` | Available payment providers |
| CheckSubscriptionStatus | `GET /api/Platform/subscription-status` | Subscription status check |

## Tenant Controller

| Method | Route | Description |
|---|---|---|
| GetTenantInfo | `GET /api/Tenant/info` | Tenant profile information |
| GetClientSubscriptions | `GET /api/Tenant/client-subscriptions` | Tenant's client subscriptions |

## TenantAuth Controller

| Method | Route | Description |
|---|---|---|
| Register | `POST /api/TenantAuth/register` | Tenant registration |
| Login | `POST /api/TenantAuth/login` | Tenant login |
| RefreshToken | `POST /api/TenantAuth/refresh` | Token refresh |
| VerifyEmail | `POST /api/TenantAuth/verify` | Email verification |
| Logout | `POST /api/TenantAuth/logout` | Session logout |

## UserAuth Controller

| Method | Route | Description |
|---|---|---|
| Register | `POST /api/UserAuth/register` | User registration |
| Login | `POST /api/UserAuth/login` | User login |
| Invite | `POST /api/UserAuth/invite` | Send user invitation |
| VerifyEmail | `POST /api/UserAuth/verify` | Email verification |
| Logout | `POST /api/UserAuth/logout` | Session logout |

## User Controller

| Method | Route | Description |
|---|---|---|
| GetUsers | `GET /api/User` | List users |
| GetUser | `GET /api/User/{id}` | Get user by ID |
| CreateUser | `POST /api/User` | Create user |
| UpdateUser | `PUT /api/User/{id}` | Update user |
| DeleteUser | `DELETE /api/User/{id}` | Delete user |
| GetRoles | `GET /api/User/roles` | Available roles |
| GetAuthorizationOptions | `GET /api/User/authorization-options` | Permission options |

## Subscriptions Controller

| Method | Route | Description |
|---|---|---|
| GetPlans | `GET /api/Subscriptions/plans` | List subscription plans |
| GetPlan | `GET /api/Subscriptions/plans/{id}` | Get plan details |
| CreatePlan | `POST /api/Subscriptions/plans` | Create plan |
| UpdatePlan | `PUT /api/Subscriptions/plans/{id}` | Update plan |
| DeletePlan | `DELETE /api/Subscriptions/plans/{id}` | Delete plan |

## Permissions Controller

| Method | Route | Description |
|---|---|---|
| GetPermissions | `GET /api/Permissions` | List permissions |
| CreatePermission | `POST /api/Permissions` | Create permission |
| UpdatePermission | `PUT /api/Permissions/{id}` | Update permission |
| DeletePermission | `DELETE /api/Permissions/{id}` | Delete permission |

## PaymentWebHooks Controller

| Method | Route | Description |
|---|---|---|
| HandleStripeWebhook | `POST /api/PaymentWebHooks/stripe` | Stripe event processing |

## Response Format

All API responses follow the `ApiResult<T>` wrapper pattern:

```json
// Success
{
  "success": true,
  "data": { ... },
  "message": "Operation completed"
}

// Error
{
  "success": false,
  "data": null,
  "message": "Error description"
}
```

Validation errors use `ApiProblemDetails` with per-field error arrays:

```json
{
  "status": 400,
  "title": "Validation failed",
  "type": "https://api.yourapp.com/errors/validation",
  "instance": "/api/endpoint",
  "traceId": "...",
  "errors": {
    "fieldName": ["Error message 1", "Error message 2"]
  }
}
```
