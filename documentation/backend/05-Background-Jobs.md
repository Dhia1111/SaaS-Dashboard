# 08 — Background Jobs (Quartz.NET)

## Overview

The application uses Quartz.NET 3.17.1 for background job scheduling with PostgreSQL persistence. The scheduler is configured with a dedicated thread pool and persistent job store.

## Configuration

```csharp
// Program.cs
builder.Services.AddQuartz(q =>
{
    // Persistent store with PostgreSQL
    q.UsePersistentStore(options =>
    {
        options.UsePostgres(PgServer =>
        {
            PgServer.ConnectionString = connString;
            PgServer.TablePrefix = "quartz.qrtz_";
        });
        options.UseProperties = true;
        options.UseNewtonsoftJsonSerializer();
        options.RetryInterval = TimeSpan.FromSeconds(30);
        options.PerformSchemaValidation = true;
    });

    // Thread pool
    q.UseDedicatedThreadPool(tp =>
    {
        tp.MaxConcurrency = 10;
    });

    // Register jobs...
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
    options.AwaitApplicationStarted = true;
});
```

## Scheduled Jobs

All jobs trigger every 1 minute using cron expression `0 */1 * ? * *`.

### 1. SendEmailJob
- **Group:** EmailGroup
- **Trigger:** EmailTrigger / EmailProcessing
- **Purpose:** Processes queued emails from the `Emails` table and sends them via SMTP (MailKit)
- **Data:** `ForceSend` (default: false)

### 2. SetPaymentToExpiredJob
- **Group:** SetPaymentToExpierGroup
- **Trigger:** SetPaymentToExpierJobKey / PaymentProcessing
- **Purpose:** Marks overdue payments as expired in the system
- **Data:** `ForceSet` (default: false)

### 3. ManageClientSubscriptionJob
- **Group:** ManageClientSubscriptionGroup
- **Trigger:** ManageClientSubscriptionTrigger / ManageClientSubscriptionProcessing
- **Purpose:** Manages client subscription lifecycle operations
- **Data:** `ForceSend` (default: false)

### 4. SetSubscriptionToExpireJob
- **Group:** SetSubscriptionToExpireGroup
- **Trigger:** SetSubscriptionToExpireTrigger / SetSubscriptionToExpireProcessing
- **Purpose:** Handles subscription expiration logic
- **Data:** `ForceSend` (default: false)

## Email Background Job Detail

The `EmailBackgroundJob` is registered as a scoped service (required for DbContext access):

```csharp
builder.Services.AddScoped<EmailBackgroundJob>();
```

The job:
1. Reads pending emails from the `Emails` table
2. Sends them via `clsExternalEmailService` (MailKit SMTP)
3. Updates email status in the database

## Persistence

Quartz.NET uses PostgreSQL as its job store via `AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL`. This means:
- Job definitions, triggers, and schedules survive application restarts
- Supports clustering for high availability (configurable)
- Schema prefix `quartz.qrtz_` keeps Quartz tables organized
