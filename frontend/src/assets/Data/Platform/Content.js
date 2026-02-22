export const footerContent = {
  copyright: `© ${new Date().getFullYear()} BillFlow. The subscription engine for SaaS founders.`,
  contact: {
    email: "hello@billflow.io",
    phone: "+1 (555) 000-BILL",
    address: "123 Dev Lane, San Francisco, CA"
  },
  links: [
    { label: "Terms", href: "/terms" },
    { label: "Privacy", href: "/privacy" },
    { label: "Contact", href: "/contact" }
  ],
  socials: [
    { label: "Twitter", href: "https://twitter.com/billflow" },
    { label: "GitHub", href: "https://github.com/billflow" }
  ]
};


export const termsContent = {
  lastUpdated: "February 22, 2026",
  sections: [
    {
      title: "1. The Service",
      content: "BillFlow provides subscription infrastructure (the 'Service'). You (the 'Tenant') use the Service to manage your own billing and enable billing for your end-users ('End-Users')."
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
      content: "We collect Tenant information (email, payment details) and End-User metadata (subscription status, feature access) necessary to provide the Service."
    },
    {
      title: "Third-Party Processing",
      content: "Payment processing is handled via Stripe. We do not store full credit card numbers on our servers."
    }
  ]
};