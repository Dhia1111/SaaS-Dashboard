# 11 — Frontend Project Structure

```
frontend/
├── index.html                     # HTML entry point (title: BillFlow)
├── package.json                   # Dependencies & scripts
├── vite.config.js                 # Vite config + Tailwind/PostCSS
├── tailwind.config.js             # Tailwind theme (custom colors, spacing, fonts)
├── eslint.config.js               # ESLint flat config
├── nginx.conf                     # Nginx config for production container
├── Dockerfile                     # Multi-stage build (Node → Nginx)
├── public/
│   └── vite.png                   # Favicon
└── src/
    ├── main.jsx                   # React entry point (Redux Provider, Syncfusion license)
    ├── App.jsx                    # Root component with React Router
    ├── store.js                   # Redux store configuration
    ├── index.css                  # Tailwind directives + CSS custom properties
    ├── App.css                    # Global app styles
    ├── styles/
    │   └── GridCustom.css         # Custom grid/Syncfusion styles
    ├── assets/
    │   ├── Data/                  # Static data files
    │   ├── language/              # i18next translation files
    │   └── react.svg
    ├── globalStates/
    │   └── AccessToken.js         # Redux slice for auth state
    ├── Apis/                      # Axios API modules
    │   ├── GenralAuth.js          # Token refresh, JWT decode, shared logic
    │   ├── Platform.js            # Platform subscription endpoints
    │   ├── Subscriotions.js       # Subscription CRUD
    │   ├── Tenant.js              # Tenant endpoints
    │   ├── tenantAuth.js          # Tenant auth endpoints
    │   ├── TenantPermissions.js   # Permission CRUD
    │   ├── Users.js               # User CRUD
    │   ├── UserAuth.js            # User auth endpoints
    │   ├── PricingCycles.js       # Pricing cycle CRUD
    │   ├── BusinessAnalyses.js    # Analytics endpoints
    │   ├── EmployeesManagment.js  # Employee management
    │   ├── RedirectPolicy/        # Axios redirect interceptors
    │   └── RetryPolicy/           # Axios retry logic
    ├── Components/
    │   ├── DashBoard/
    │   │   ├── Dashboard.jsx      # Dashboard layout with sidebar
    │   │   └── AcountHub.jsx      # Account hub view
    │   ├── Platform/              # Platform-facing pages
    │   │   ├── PlatformLayout.jsx # Main layout wrapper
    │   │   ├── Main.jsx           # Landing/home page
    │   │   ├── NavBar.jsx         # Navigation bar
    │   │   ├── LogIn.jsx          # Login page
    │   │   ├── SignUp.jsx         # Sign up page
    │   │   ├── SignInOptions.jsx  # Sign-in method selection
    │   │   ├── Contact.jsx        # Contact page
    │   │   ├── Legal.jsx          # Terms/Privacy page
    │   │   ├── Subscription.jsx   # Subscription display
    │   │   ├── CheckoutForm.jsx   # Stripe Checkout form
    │   │   ├── PlanCheckOutWarpper.jsx  # Checkout wrapper
    │   │   ├── PaymentFlowOrchestrator.jsx # Payment flow
    │   │   ├── SubscriptionStatusChecker.jsx # Status check
    │   │   ├── UpgradePlanOrchestrator.jsx  # Upgrade flow
    │   │   └── MarketingDiscoveryStep.jsx   # Marketing platform selection
    │   ├── User/                  # User management components
    │   ├── UserAuth/              # User authentication components
    │   ├── Tenant/                # Tenant management components
    │   ├── TenantPayments/        # Payment history components
    │   ├── SubscriptionSettings/  # Subscription settings CRUD
    │   ├── PricingCycles/         # Pricing cycle management
    │   ├── EndPointsPermissionsManagement/ # Permission management
    │   ├── EmployeesManagment/    # Employee management
    │   └── BusinessAnalysis/      # Analytics dashboard
    └── test/
        └── setup.js              # Test configuration
```

## Key Dependencies

| Package | Category |
|---|---|
| `react`, `react-dom` | Core UI |
| `react-router-dom` | Routing |
| `@reduxjs/toolkit`, `react-redux` | State management |
| `axios` | HTTP client |
| `@stripe/react-stripe-js`, `@stripe/stripe-js` | Payment UI |
| `@syncfusion/ej2-react-*` | Data grids, charts, buttons, modals |
| `tailwindcss` | Styling |
| `i18next`, `react-i18next` | Internationalization |
| `lucide-react`, `@fortawesome/*` | Icons |
| `vitest`, `@testing-library/react` | Testing |
