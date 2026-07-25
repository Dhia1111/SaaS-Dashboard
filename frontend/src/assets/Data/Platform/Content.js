export const footerContent = {
  copyright: `© ${new Date().getFullYear()} SubFlow. The subscription infrastructure for SaaS platforms.`,
  contact: {
    email: "hello@subflow.io",
    phone: "+1 (555) 000-SUBF",
    address: "123 Dev Lane, San Francisco, CA"
  },
  links: [
    { label: "Terms", href: "/terms" },
    { label: "Privacy", href: "/privacy" },
    { label: "Contact", href: "/contact" }
  ],
  socials: [
    { label: "Twitter", href: "https://twitter.com/subflow" },
    { label: "GitHub", href: "https://github.com/subflow" }
  ]
};


export const termsContent = {
  lastUpdated: "February 22, 2026",
  sections: [
    {
      title: "1. The Service",
      content: "SubFlow provides subscription infrastructure (the 'Service'). You (the 'Tenant') use the Service to manage subscriptions and enable recurring billing for your end-users ('End-Users')."
    },
    {
      title: "2. Usage & Limits",
      content: "You are responsible for all activities occurring under your API keys. Fair use limits apply to webhook delivery and API requests based on your selected plan."
    }
  ]
};

export const privacyContent = {
  lastUpdated: "February 22, 2026",
  sections: [
    {
      title: "Data Collection",
      content: "We collect Tenant information (email, account details) and End-User metadata (subscription status, feature access) necessary to provide the Service."
    },
    {
      title: "Third-Party Processing",
      content: "Payment processing is handled via Stripe. We do not store full credit card numbers on our servers."
    }
  ]
};