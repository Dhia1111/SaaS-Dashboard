// src/content.js
export const contactContent = {
  header: {
    title: "Let’s talk infrastructure.",
    highlight: "infrastructure.",
    description: "Whether you're migrating 10,000 tenants or just starting your first SaaS, our engineering team is here to help you get Layer 2 billing right."
  },
  supportCards: [
    {
      title: "Technical Support",
      description: "Deep dive into our APIs, webhooks, or integration logic.",
      linkText: "support@billflow.io",
      linkHref: "mailto:support@billflow.io",
      type: "standard"
    },
    {
      title: "Sales & Partnerships",
      description: "Looking for enterprise-grade volume or custom SLAs?",
      linkText: "sales@billflow.io",
      linkHref: "mailto:sales@billflow.io",
      type: "standard"
    },
    {
      title: "Developer Discord",
      description: "Join 2,000+ SaaS founders building on BillFlow.",
      linkText: "Join Community",
      linkHref: "#",
      type: "featured" // This one uses the primary color background
    }
  ],
  formOptions: [
    "New SaaS MVP",
    "Migrating from custom engine",
    "Enterprise Integration",
    "General Inquiry"
  ]
};

