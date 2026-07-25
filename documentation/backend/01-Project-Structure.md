# 04 — Backend Project Structure

## Solution Architecture

The backend is a .NET 8 solution with 5 projects:

```
APIs.sln
├── APIs.csproj               # ASP.NET Core Web API (entry point)
├── Business.csproj            # Business logic layer
├── Connection.csproj          # EF Core DbContext, repositories, migrations
├── ExternalAPI.csproj         # External service integrations
└── SharedDtoAndEnum.csproj    # Shared DTOs and enums
```

## APIs Project (Web API)

```
APIs/
├── Program.cs                    # App startup, DI, middleware, auth config
├── APIDependencyInjection.cs     # DI registrations for API layer
├── Controllers/
│   ├── Platform.cs               # Platform subscription management
│   ├── Tenant.cs                 # Tenant info & client subscriptions
│   ├── TenantAuth.cs             # Tenant login/register/refresh/verify/logout
│   ├── UserAuth.cs               # User login/register/verify/invite/logout
│   ├── User.cs                   # User CRUD, roles, authorization
│   ├── Subscriptions.cs          # Subscription plan management
│   ├── Permissions.cs            # Permission CRUD
│   ├── TenantPricingCycle.cs     # Pricing cycle CRUD
│   ├── EmployeesManagment.cs     # Employee management
│   ├── BusinessAnalyses.cs       # Business analytics
│   ├── PaymentWebHooks.cs        # Stripe webhook handling
│   ├── CentrelaziedAuthentication.cs  # Google OAuth
│   └── test.cs                   # Test endpoint
├── TokenHandler/
│   ├── TokenHandler.cs           # JWT generation and validation
│   └── JwtSchemeSelector.cs      # Dynamic JWT scheme selection
├── ConfigClasses/
│   ├── Identity.cs               # User/Tenant identity helpers
│   ├── InfoClasses.cs            # Configuration POCOs
│   ├── PermissionsLoader.cs      # Startup permission seeding
│   ├── TenantIdProvider.cs       # Multi-tenant ID provider
│   └── AccessTokenReader.cs      # JWT access token parsing
├── BackGroundJobs/
│   ├── SendEmail.cs              # Email sending job
│   ├── SetToExperiedPayment.cs   # Payment expiry job
│   ├── ManageClientSubscription.cs  # Client subscription management
│   └── SetSubscriptionToExpire.cs   # Subscription expiry job
├── Responses/
│   ├── ApiResult.cs              # Standard API response wrapper
│   ├── ApiProblemDetails.cs      # Validation error response
│   └── ExceptionMiddleWare.cs    # Global exception handler
├── Hashing/
│   ├── GenralHashingService.cs   # General hashing utilities
│   └── PasswordHashService.cs    # Password hashing (bcrypt-like)
├── AssetHandler/
│   └── EmailTemplateHandler.cs   # Email template rendering
└── wwwroot/
    ├── EmailTemplate.html        # Email template
    └── EmailTemplateUsers.html   # User email template
```

## Business Layer

```
Business/
├── ServicesDependencyInjection.cs    # DI registrations
├── clsTenantService.cs               # Tenant CRUD operations
├── clsUserService.cs                 # User CRUD operations
├── clsTenantPlanServices.cs          # Subscription plan management
├── clsPlatformSubscriptionService.cs # Subscription orchestration
├── clsPaymentService.cs              # Payment processing logic
├── clsJwtSerivce.cs                  # JWT token creation/validation
├── clsEmailService.cs                # Email queuing
├── clsEmployeeService.cs             # Employee management
├── clsTenantPermissionServices.cs    # Permission management
├── clsTenantPricingCycleServices.cs  # Pricing cycle management
├── clsDiscoveryPlatformService.cs    # Marketing platform discovery
├── clsClientSubscriptionService.cs   # Client subscription management
├── clsTenantFreePlanService.cs       # Free trial management
├── clsTenantPlanBenifestServices.cs  # Plan benefits management
├── clsTenantPlanPermissionServices.cs # Plan-permission mapping
├── clsTenantPricingOptionServices.cs # Pricing option management
├── clsTenantSessionService.cs        # Tenant session management
├── UserSessionService.cs             # User session management
├── clsGeneric.cs                     # Generic business utilities
├── clsPlatformAdmineService.cs       # Platform admin operations
├── TokenHandlerService.cs            # Token handling service
├── GenralHashService.cs              # Hashing service
├── PasswordHashService.cs            # Password hashing
├── AssetsHandler.cs                  # Asset management
├── clsEmailSettingAndEnvirment.cs    # Email config
├── EndToEndService/
│   ├── TenantAuthService.cs          # Full tenant auth flow
│   ├── UserAuthService.cs            # Full user auth flow
│   └── GenricRefreshTokenService.cs  # Token refresh logic
├── Config/
│   ├── PlatformInfo.cs               # Platform configuration POCO
│   └── NamingCookies.cs             # Cookie name constants
├── Exceptions/
│   └── DomainException.cs            # Domain exception hierarchy
└── Validations/
    ├── EmailValidation.cs            # Email format validation
    └── StringComplexityValidator.cs  # Password strength validation
```

## Connection Layer (Data Access)

```
Connection/
├── ConnectionDependencyInjection.cs    # DI registrations
├── Data/
│   ├── SaasDashboardContext.cs         # EF Core DbContext
│   └── SaasDashboardContextContextFactory.cs  # Design-time factory
├── Migrations/                         # EF Core migrations
└── models/
    ├── Entites/                        # Entity models (20 files)
    ├── clsGeneric.cs                   # Generic repository (IGenericRepo<T>)
    ├── clsTanent.cs                    # Tenant repository
    ├── clsUser.cs                      # User repository
    ├── clsPerson.cs                    # Person repository
    ├── clsPaymentRepo.cs               # Payment repository
    ├── clsPlatformSubscription.cs      # Platform subscription repo
    ├── clsClientSubscriptionRepo.cs    # Client subscription repo
    ├── TenantPlanRepository.cs         # Tenant plan repo
    ├── TenantPlanPermissionRepository.cs  # Plan-permission repo
    ├── TenantPlanBenifestRepository.cs # Plan benefits repo
    ├── TenantPricingOptionRepository.cs # Pricing option repo
    ├── TenantPricingCycleRepository.cs # Pricing cycle repo
    ├── TenantPermissionRepository.cs   # Permission repo
    ├── TenantFreePlanRepo.cs           # Free plan repo
    ├── clsDiscoveryPlatformRepo.cs     # Discovery platform repo
    ├── clsEmployeeRepo.cs              # Employee repo
    ├── clsTenantSession.cs             # Tenant session repo
    ├── clsUserSessions.cs              # User session repo
    ├── clsEmail.cs                     # Email queue repo
    └── clsPlatformAdmineRepo.cs        # Platform admin repo
```

## ExternalAPI Layer

```
ExternalAPI/
├── ExternalAPIDependencyInjection.cs   # DI registrations
├── clsExternalEmailService.cs          # SMTP email via MailKit
├── clsExternalPaymentService.cs        # Stripe payment integration
└── PaymentProvidersConfig/
    └── StripeInfo.cs                   # Stripe configuration POCO
```

## SharedDtoAndEnum

```
SharedDtoAndEnum/
├── Enum.cs                        # All system enums
└── SharedDtoAndEnum.csproj
```
