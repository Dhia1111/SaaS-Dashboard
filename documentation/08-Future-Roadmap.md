# 18 — Future Roadmap

## Planned Improvements

### Payment Providers
- Additional payment providers beyond Stripe (PayPal, Paddle, Braintree)
- Multi-currency support
- Usage-based billing models

### Billing & Invoicing
- Automated invoice generation
- PDF invoice download
- Tax calculation and reporting

### Audit & Logging
- Comprehensive audit logging for all tenant operations
- Change history for subscription and permission modifications
- Admin activity tracking

### Public API
- RESTful public API for third-party integrations
- API key management for tenants
- Rate limiting and usage tracking

### Webhooks
- Outgoing webhooks for tenant events
- Subscription lifecycle events (created, renewed, expired, upgraded)
- Payment events

### Reporting
- Advanced reporting with custom date ranges
- Export to CSV/Excel
- Scheduled report delivery via email

### White-Label
- Custom domain support for tenants
- Branded email templates
- Customizable UI themes per tenant

### Role-Based Administration
- Enhanced RBAC with granular permissions
- Custom role creation per tenant
- Department-level access control

## Known Technical Improvements

- **Secrets management** — Move all secrets to environment variables or a vault (currently exposed in config files)
- **CI/CD automation** — Create GitHub Actions workflows (currently only local Taskfile)
- **Test coverage** — Expand unit and integration tests
- **API versioning** — Implement proper API versioning strategy
- **Rate limiting** — Add API rate limiting middleware
- **Health checks** — Add health check endpoints for container orchestration

## License

This project is currently intended as a portfolio and learning project. Licensing terms may be updated in the future.
