// src/content.js
export const contactContent = {
  header: {
    title: "Let’s talk infrastructure.",
    highlight: "infrastructure.",
    description: "Whether you're migrating 10,000 subscribers or just starting your first SaaS, our engineering team is here to help you get subscription management right."
  },
  supportCards: [
    {
      title: "Technical Support",
      description: "Deep dive into our APIs, webhooks, or integration logic.",
      linkText: "support@subflow.io",
      linkHref: "mailto:support@subflow.io",
      type: "standard"
    },
    {
      title: "Sales & Partnerships",
      description: "Looking for enterprise-grade volume or custom SLAs?",
      linkText: "sales@subflow.io",
      linkHref: "mailto:sales@subflow.io",
      type: "standard"
    },
    {
      title: "Developer Discord",
      description: "Join 2,000+ SaaS founders building on SubFlow.",
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

