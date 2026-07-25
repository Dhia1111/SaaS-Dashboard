# 07 — Database Schema & Multi-Tenancy

## Database

- **Database Engine:** PostgreSQL 14
- **ORM:** Entity Framework Core 8.0.11
- **Connection:** Port 5431 (host) / 5432 (container), user `d1111`, database `Saas-Dashboard`

## Entity Model

### Core Entities

```
Person (1) ──── (1) Tenant
Person (1) ──── (1) User
Tenant (1) ──── (*) TenantPlan
Tenant (1) ──── (*) PlatformSubscription
Tenant (1) ──── (*) Payment
Tenant (1) ──── (*) Employee
Tenant (1) ──── (*) TenantSession
Tenant (1) ──── (*) ClientSubscription
Tenant (1) ──── (*) DiscoveryPlatform
Tenant (1) ──── (*) TenantFreePlan
User   (1) ──── (*) UserSession
```

### Entity List (20 entities)

| Entity | Key Fields | Description |
|---|---|---|
| `Tenant` | TenantId, Name, IsActive, HaveUsedFreeTry, PasswordHash, PersonId | Multi-tenant business accounts |
| `User` | Id, TenantId, PersonId, PasswordHash, Role, Authorization, IsActive | End users belonging to tenants |
| `Person` | PersonId, ... | Shared personal info for both tenants and users |
| `Payment` | Id, TenantId, Provider, ProviderPaymentId, PaymentStatus, Currency, Amount, SubscriptionId | Payment transactions |
| `PlatformSubscription` | Id, TenantId, TenantPlanPricingOptionId, StartedAt, EndsAt, IsActive, IsItFree | Subscription records |
| `ClientSubscription` | Id, TenantId, ... | Client subscription tracking |
| `TenantPlan` | Id, TenantId, Name, Description, ... | Subscription plan definitions |
| `TenantPlanPricingOption` | Id, TenantPlanId, Amount, TenantPricingCycleId | Plan pricing options |
| `TenantPricingCycle` | Id, Name, Period, ... | Billing cycle definitions (hourly, daily, weekly, monthly, yearly) |
| `TenantPlanPermission` | Id, TenantPlanId, TenantPermissionId | Plan-permission mapping |
| `TenantPlanBenefit` | Id, TenantPlanId, ... | Plan benefits (max users, storage, etc.) |
| `TenantPermission` | Id, Name, Description, ... | Permission definitions |
| `TenantFreePlan` | Id, TenantId, ... | Free trial tracking |
| `DiscoveryPlatform` | Id, TenantId, Platform Name | Marketing platform selections |
| `Employee` | Id, TenantId, ... | Tenant employees |
| `PlatformAdmine` | Id, ... | Platform administrators |
| `Email` | Id, To, Subject, Body, Status, ... | Email queue |
| `TenantSession` | Id, TenantId, Token, ... | Tenant auth sessions |
| `UserSession` | Id, UserId, Token, ... | User auth sessions |
| Quartz tables | (auto-managed by AppAny.Quartz) | Background job scheduling tables |

## Multi-Tenancy Implementation

Tenant data isolation is enforced at the database query level using EF Core global query filters:

```csharp
// SaasDashboardContext.cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Apply global tenant filter to all tenant-scoped entities
    modelBuilder.Entity<Tenant>().HasQueryFilter(e => e.TenantId == _tenantIdProvider.TenantId);
    modelBuilder.Entity<User>().HasQueryFilter(e => e.TenantId == _tenantIdProvider.TenantId);
    // ... applied to all entities implementing IEntityWithTenantId
}
```

### Key Interfaces

- `IEntityWithTenantId` — Applied to all tenant-scoped entities
- `IEntity` — Base entity marker interface

### Tenant ID Provider

`ITenantIdProvider` resolves the current tenant ID from the JWT claims, ensuring all queries automatically filter by the correct tenant without developer intervention.

## Migrations

The project uses EF Core migrations with a design-time factory (`SaasDashboardContextContextFactory`) that configures the DbContext for migration commands. The migration history includes 8 migrations covering:

- Initial schema creation
- Tenant plan and pricing configuration
- Payment and subscription tables
- Permission and benefit systems
- Session management
- Free plan tracking
- Quartz scheduling schema
