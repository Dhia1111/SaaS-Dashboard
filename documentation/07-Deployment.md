# 17 — Deployment Guide

## Prerequisites

- Docker & Docker Compose
- .NET 8 SDK (for local development)
- Node.js 20+ (for local development)
- Stripe account (for payment processing)
- Google OAuth credentials (for social login)
- SMTP server credentials (for email)

## Environment Configuration

### Required Configuration Sections

| Config Section | File | Purpose |
|---|---|---|
| `JwtSettings` | `appsettings.json` / env vars | JWT signing key, issuer, audience |
| `Google:ClientID` | `appsettings.json` | Google OAuth client ID |
| `Google:ClientSecret` | env vars | Google OAuth client secret |
| `StripeInfo` | env vars | Stripe secret + webhook keys |
| `EmailSettings` | env vars | SMTP host, port, credentials |
| `PlatformInfo` | `appsettings.json` | Tenant name, default currency |
| `ClientInfo` | `appsettings.json` | Frontend URL for redirects |

### Docker Compose Deployment

```sh
# Build and start all services
docker compose up --build -d

# Verify running containers
docker compose ps

# View logs
docker compose logs -f backend
docker compose logs -f frontend
```

### Service Ports

| Service | Internal Port | External Port |
|---|---|---|
| PostgreSQL | 5432 | 5431 |
| Backend API | 7073 | 7073 |
| Frontend | 80 | 5173 |

### Environment Variables (Docker)

```yaml
backend:
  environment:
    DB_HOST: db
    DB_PORT: 5432
    DB_USER: d1111
    DB_PASSWORD: mypassword
    DB_NAME: Saas-Dashboard
    # Add via .env or docker-compose.override.yml:
    # ASPNETCORE_ENVIRONMENT: Production
    # JwtSettings__Key: <your-jwt-key>
    # Google__ClientSecret: <your-google-secret>
    # StripeInfo__SecretKey: <your-stripe-secret>
    # StripeInfo__WebhookSecret: <your-webhook-secret>
    # EMAIL_HOST: <smtp-host>
    # EMAIL_PORT: 587
    # EMAIL: <smtp-user>
    # EMAIL_PASSWORD: <smtp-password>
```

## Production Considerations

1. **Secrets Management** — Use a secrets manager (Azure Key Vault, HashiCorp Vault) instead of plaintext config files
2. **HTTPS** — Configure TLS termination at the reverse proxy level
3. **Database** — Use a managed PostgreSQL service (RDS, Cloud SQL) for production
4. **Scaling** — The backend is stateless; scale horizontally behind a load balancer
5. **Session Affinity** — Not required (JWT-based auth is stateless)
6. **Monitoring** — Integrate application monitoring (Application Insights, Datadog)
7. **Backups** — Configure automated PostgreSQL backups
8. **CORS** — Update `WithOrigins()` in Program.cs to match the production frontend URL

## Manual Deployment Steps

```sh
# 1. Build backend
dotnet publish backend/APIs/APIs.csproj -c Release -o out/backend

# 2. Build frontend
cd frontend && npm run build && cd ..

# 3. Copy to server
# 4. Configure environment variables
# 5. Run database migrations
# 6. Start services
```
