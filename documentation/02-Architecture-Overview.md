# 02 — Architecture Overview

## High-Level Architecture

```
┌─────────────────────────────────────────────────────┐
│                   Frontend (React)                   │
│              http://localhost:5173                    │
└──────────────────┬──────────────────────────────────┘
                   │ HTTP / JSON
                   ▼
┌─────────────────────────────────────────────────────┐
│            Backend API (ASP.NET Core 8)              │
│              http://localhost:7073                    │
│                                                      │
│  ┌──────────┐ ┌──────────┐ ┌──────────────────┐     │
│  │ Business │ │External  │ │ SharedDto&Enum   │     │
│  │  Layer   │ │  API     │ │                   │     │
│  └────┬─────┘ └────┬─────┘ └──────────────────┘     │
│       │             │                                │
│  ┌────▼─────────────▼─────┐                          │
│  │   Connection (EF Core) │                          │
│  └────────────────────────┘                          │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│              PostgreSQL 14 Database                   │
│              Port: 5431 (host) / 5432 (container)    │
└─────────────────────────────────────────────────────┘
```

## Project Structure

```
SaaS-Dashboard/
├── backend/
│   ├── APIs/                  # ASP.NET Core Web API (entry point)
│   ├── Business/              # Business logic layer
│   ├── Connection/            # EF Core DbContext, repositories, migrations
│   ├── ExternalAPI/           # Stripe, MailKit integrations
│   └── SharedDtoAndEnum/      # Shared DTOs and enums
├── frontend/                  # React + Vite SPA
├── db/                        # PostgreSQL Docker image
├── docker-compose.yaml        # Multi-container orchestration
└── Taskfile.yaml              # Task runner (dev/build/test/ci)
```

## Design Principles

- **Multi-tenant architecture** — Data isolation via tenant ID filtering at the database query level
- **Separation of concerns** — Platform administration and tenant management are distinct domains
- **Configurable subscriptions** — Plans, pricing, and permissions are data-driven, not hardcoded
- **Permission-driven feature access** — Bit-flag based authorization system
- **Flexible pricing** — Fully configurable pricing cycles and options
- **Secure authentication** — Multi-scheme JWT with dynamic scheme selection + Google OAuth
- **Extensible business rules** — Domain-driven exception handling and validation services
