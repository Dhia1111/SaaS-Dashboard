# 12 — Routing & Navigation

## Router Structure

React Router v7 manages all client-side routing from `App.jsx`:

```
/                              → PlatformLayout
  /                            → Main (Landing page)
  /contact                     → Contact page
  /terms                       → Terms of service
  /privacy                     → Privacy policy
  /signin-options              → Sign-in method selection
  /login                       → Login
  /signup                      → Sign up
  /user-auth/login             → User-specific login
  /dashboard                   → DashboardLayout
    /                          → AccountHub
    /user                      → UsersList / AddUser / VerifyUser
    /permissions               → PermissionsList / AddNewPermission
    /analytics                 → BusinessAnalysis
    /payments                  → TenantPayments
    /subscriptions             → SubscriptionSettings / Add / Edit
    /pricing-cycles            → PricingCyclesSettings / Add
    /employees-managment       → EmployeesList / AddEmployee
  /payment-process             → PaymentFlowOrchestrator
  /check-subscription-status   → SubscriptionStatusChecker
  /upgrade-subscription        → UpgradePlanOrchestrator
```

## Layout Hierarchy

```
PlatformLayout (public pages)
├── NavBar
├── Main / Contact / Legal / Auth pages
└── Footer (implicit)

DashboardLayout (authenticated pages)
├── Sidebar navigation
└── Content area (AccountHub, Users, etc.)
```

## Route Definitions (App.jsx)

The routes are defined using React Router's `<Routes>` and `<Route>` components with nested layouts:

```jsx
<Routes>
  <Route element={<PlatformLayout />}>
    <Route index element={<Main />} />
    <Route path="contact" element={<Contact />} />
    <Route path="terms" element={<Legal />} />
    <Route path="privacy" element={<Legal />} />
    <Route path="signin-options" element={<SignInOptions />} />
    <Route path="login" element={<LogIn />} />
    <Route path="signup" element={<SignUp />} />
    <Route path="user-auth/login" element={<UserLogin />} />
  </Route>

  <Route path="dashboard" element={<DashboardLayout />}>
    <Route index element={<AccountHub />} />
    <Route path="user" element={<UsersList />} />
    <Route path="user/add" element={<AddUser />} />
    <Route path="permissions" element={<PermissionsList />} />
    <Route path="analytics" element={<BusinessAnalysis />} />
    <Route path="payments" element={<TenantPayments />} />
    <Route path="subscriptions" element={<SubscriptionSettings />} />
    <Route path="pricing-cycles" element={<PricingCyclesSettings />} />
    <Route path="employees-managment" element={<EmployeesList />} />
  </Route>

  <Route path="payment-process" element={<PaymentFlowOrchestrator />} />
  <Route path="check-subscription-status" element={<SubscriptionStatusChecker />} />
  <Route path="upgrade-subscription" element={<UpgradePlanOrchestrator />}
</Routes>
```

## Navigation Components

### NavBar (Platform)
Top navigation bar for public-facing pages with links to:
- Home, Contact, Legal pages
- Sign-in options, Login, Signup
- Conditional rendering based on auth state

### Dashboard Sidebar
Left sidebar navigation for authenticated dashboard including:
- Account Hub
- User Management
- Permissions
- Analytics
- Payments
- Subscriptions
- Pricing Cycles
- Employee Management
