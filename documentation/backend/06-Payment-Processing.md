# 09 — Payment Processing (Stripe)

## Overview

Payment processing is handled through Stripe's Checkout Sessions API. The system supports:
- One-time payments and subscription-based billing
- Webhook-based payment confirmation
- Automatic subscription activation on successful payment

## Configuration

Stripe credentials are configured via the `StripeInfo` configuration class:

```csharp
public class StripeInfo
{
    public string SecretKey { get; set; }
    public string WebhookSecret { get; set; }
}
```

These values are loaded from `appsettings.Development.json` or environment variables.

## Payment Flow

```
┌──────────┐     ┌──────────┐     ┌──────────┐     ┌──────────┐
│  Client  │────▶│ Backend  │────▶│  Stripe  │────▶│  Client  │
│  Browser │     │   API    │     │          │     │ (Return) │
└──────────┘     └──────────┘     └──────────┘     └──────────┘
                     │                                │
                     │    ┌──────────────────┐        │
                     └───▶│ PaymentWebHooks  │◀───────┘
                          │   Controller     │
                          └──────────────────┘
```

### Step-by-Step

1. **Client initiates checkout** — Frontend calls the backend to create a Stripe Checkout Session
2. **Backend creates session** — `clsExternalPaymentService` calls Stripe API with plan details
3. **Client redirected to Stripe** — Browser redirects to Stripe Checkout page
4. **Customer completes payment** — Stripe handles payment collection
5. **Stripe sends webhook** — Stripe sends `checkout.session.completed` event to backend
6. **Backend processes webhook** — `PaymentWebHooksController.HandleStripeWebhook` verifies and activates subscription
7. **Client redirected back** — Customer returns to the application

## Stripe Checkout Session Creation

```csharp
// clsExternalPaymentService.cs
public async Task<string> CreateCheckoutSession(decimal amount, string currency,
    string successUrl, string cancelUrl, string metadata)
{
    var options = new SessionCreateOptions
    {
        PaymentMethodTypes = new List<string> { "card" },
        LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(amount * 100),  // Convert to cents
                    Currency = currency,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Subscription"
                    }
                },
                Quantity = 1
            }
        },
        Mode = "payment",
        SuccessUrl = successUrl,
        CancelUrl = cancelUrl,
        Metadata = new Dictionary<string, string>
        {
            { "tenantId", metadata }
        }
    };

    var session = await _stripeService.Session.CreateAsync(options);
    return session.Id;
}
```

## Webhook Handling

The `PaymentWebHooksController` listens for Stripe events at `POST /api/PaymentWebHooks/stripe`:

```csharp
[HttpPost("stripe")]
public async Task<IActionResult> HandleStripeWebhook()
{
    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

    var stripeEvent = EventUtility.ConstructEvent(
        json,
        Request.Headers["Stripe-Signature"],
        _stripeInfo.WebhookSecret
    );

    if (stripeEvent.Type == "checkout.session.completed")
    {
        var session = stripeEvent.Data.Object as Session;
        // Activate subscription based on session metadata
        await _paymentService.ProcessSuccessfulPayment(session);
    }

    return Ok();
}
```

## Payment Tracking

Payments are recorded in the `Payments` table with:
- `Provider` — Payment provider (Stripe)
- `ProviderPaymentId` — Stripe session/payment intent ID
- `PaymentStatus` — Current status (Pending, Success, Failed, Refunded)
- `Amount` — Payment amount
- `Currency` — Currency code
- `SubscriptionId` — Associated subscription
- `TenantId` — Owning tenant

## Frontend Integration

The frontend uses `@stripe/react-stripe-js` and `@stripe/stripe-js` packages:
- `CheckoutForm.jsx` — Stripe Elements-based checkout form
- `PlanCheckOutWarpper.jsx` — Checkout session wrapper
- `PaymentFlowOrchestrator.jsx` — Full payment flow orchestration
- `UpgradePlanOrchestrator.jsx` — Upgrade payment flow
