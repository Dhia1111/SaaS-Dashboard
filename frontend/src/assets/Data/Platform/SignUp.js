export const signupContent = {
  plans: [
    {
      id: 'free',
      name: 'Free Trial',
      price: '$0',
      duration: '1 Week',
      features: ['Up to 5 Tenants', 'Basic Webhooks', 'Community Support'],
      buttonText: 'Start Trial',
      highlight: false
    },
    {
      id: 'common',
      name: 'Common',
      price: '$49',
      duration: 'per month',
      features: ['Unlimited Tenants', 'Layer 2 Billing Engine', 'Custom Checkouts', 'Priority Support'],
      buttonText: 'Get Started',
      highlight: true
    },
    {
      id: 'enterprise',
      name: 'Enterprise',
      price: 'Custom',
      duration: 'Contact Sales',
      features: ['Dedicated Infrastructure', 'SLA Guarantees', 'White-label SDK', '24/7 Phone Support'],
      buttonText: 'Contact Us',
      highlight: false
    }
  ]
};