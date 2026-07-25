# 15 — UI Components Overview

## Platform Components (Public-Facing)

### Layout
- **`PlatformLayout.jsx`** — Main layout wrapper with header and footer for all public pages
- **`NavBar.jsx`** — Responsive navigation with auth-aware links (login/logout conditional)

### Landing & Static Pages
- **`Main.jsx`** — Landing/home page with platform overview and CTAs
- **`Contact.jsx`** — Contact information page
- **`Legal.jsx`** — Terms of service and privacy policy (shared component)

### Authentication Pages
- **`SignInOptions.jsx`** — Selection screen for sign-in method (Google OAuth / email)
- **`LogIn.jsx`** — Tenant login form
- **`SignUp.jsx`** — Tenant registration form
- **`UserAuth/LogIn.jsx`** — User-specific login

### Subscription & Payment
- **`Subscription.jsx`** — Displays available subscription plans
- **`CheckoutForm.jsx`** — Stripe Elements payment form
- **`PlanCheckOutWarpper.jsx`** — Wraps checkout with plan context
- **`PaymentFlowOrchestrator.jsx`** — Full payment flow orchestration
- **`SubscriptionStatusChecker.jsx`** — Checks and displays subscription status
- **`UpgradePlanOrchestrator.jsx`** — Upgrade plan flow
- **`MarketingDiscoveryStep.jsx`** — Marketing platform selection during signup

## Dashboard Components (Authenticated)

### Layout
- **`Dashboard.jsx`** — Dashboard layout with sidebar navigation and content area
- **`AcountHub.jsx`** — Account overview hub

### Management Views
- **User/** — User list, add user, verify user
- **UserAuth/** — User authentication management
- **Tenant/** — Tenant profile and settings
- **TenantPayments/** — Payment history and receipts
- **SubscriptionSettings/** — Subscription plan CRUD (list, add, edit)
- **PricingCycles/** — Pricing cycle management (list, add)
- **EndPointsPermissionsManagement/** — Permission CRUD
- **EmployeesManagment/** — Employee list, add employee
- **BusinessAnalysis/** — Analytics dashboard with charts

## UI Libraries

### Syncfusion EJ2 Components
- `@syncfusion/ej2-react-grids` — Data grids for tabular data (users, subscriptions, payments)
- `@syncfusion/ej2-react-charts` — Charts for analytics (revenue trends, subscription growth)
- `@syncfusion/ej2-react-buttons` — Styled buttons
- `@syncfusion/ej2-react-popups` — Modals, dialogs, tooltips

### Stripe Components
- `Elements` — Stripe Elements provider
- `PaymentElement` — Secure payment form
- Integration with `@stripe/react-stripe-js`

### Custom Styling
- **Tailwind CSS** — Utility-first styling with custom theme variables
- **CSS Custom Properties** — Theming for colors, spacing, fonts (see `index.css`)
- **`GridCustom.css`** — Overrides for Syncfusion grid styling

## Theme System

The `index.css` defines CSS custom properties for theming:

```css
:root {
  --color-primary: ...;
  --color-secondary: ...;
  --color-success: ...;
  --color-danger: ...;
  --color-warning: ...;
  --spacing-section: ...;
  --spacing-card: ...;
  --font-heading: ...;
  --font-body: ...;
}
```

Tailwind extends these values so they can be used as utility classes:
- `bg-primary`, `text-secondary`, `font-heading`, etc.
- Supports alpha transparency via `<alpha-value>` syntax
