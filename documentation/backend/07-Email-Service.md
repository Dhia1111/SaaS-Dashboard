# 10 — Email Service

## Overview

The email service uses **MailKit** for SMTP-based email delivery with a database-backed queue system. Emails are created in the application, stored in the `Emails` table, and processed asynchronously by a Quartz background job.

## Architecture

```
┌──────────────┐     ┌──────────┐     ┌──────────────┐     ┌──────────┐
│  Business    │────▶│  Emails  │────▶│  SendEmail    │────▶│  SMTP    │
│  Logic       │     │  Table   │     │  Background   │     │  Server  │
│              │     │  (DB)    │     │  Job          │     │          │
└──────────────┘     └──────────┘     └──────────────┘     └──────────┘
```

## Email Queue

The `Emails` database table stores pending email messages with:
- Recipient address (`To`)
- Subject and body
- Status tracking (pending/sent/failed)
- Timestamps

## Email Sending

`clsExternalEmailService` handles actual SMTP delivery via MailKit:

```csharp
public class clsExternalEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // Reads EMAIL_HOST, EMAIL_PORT, EMAIL, EMAIL_PASSWORD from environment
        // Creates MimeMessage with from/to/subject/body
        // Sends via SmtpClient using SSL/STARTTLS
    }
}
```

Environment variables required:
- `EMAIL_HOST` — SMTP server hostname
- `EMAIL_PORT` — SMTP server port
- `EMAIL` — SMTP username / from address
- `EMAIL_PASSWORD` — SMTP password

## Email Templates

HTML email templates are stored in `APIs/wwwroot/`:
- `EmailTemplate.html` — General email template
- `EmailTemplateUsers.html` — User-specific email template

`EmailTemplateHandler.cs` handles template rendering with dynamic content injection.

## Email Configuration

Two email setting configurations are registered for different contexts:

```csharp
// API-level email settings (external)
builder.Services.Configure<ExternalAPI.EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// Business-level email settings
builder.Services.Configure<Business.EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
```

## Usage in Controllers

Email is triggered from various business operations:
- User/Tenant registration verification
- Password reset flows
- Invitation emails
- Subscription notifications
- Payment confirmations
