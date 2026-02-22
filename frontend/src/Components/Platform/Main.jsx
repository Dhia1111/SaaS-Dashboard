// MainPage.jsx
import { 
  ArrowRight, 
  Zap, 
  Shield, 
  BarChart3, 
  Users, 
  CheckCircle,
  Star,
  TrendingUp
} from 'lucide-react';

const MainPage = () => {
  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-white">
      {/* Navigation */}
    
      {/* Hero Section */}
      <section className="pt-32 pb-20 px-6">
        <div className="max-w-7xl mx-auto">
          <div className="grid md:grid-cols-2 gap-12 items-center">
            <div>
              <div className="inline-flex items-center gap-2 bg-primary/10 text-primary px-4 py-2 rounded-full mb-6">
                <Zap size={16} />
                <span className="text-sm font-medium">The Billing Engine for SaaS</span>
              </div>
              <h1 className="text-5xl md:text-6xl font-bold leading-tight mb-6">
                We help tenants bill{' '}
                <span className="bg-gradient-to-r from-primary to-secondary bg-clip-text text-transparent">
                  THEIR users
                </span>
              </h1>
              <p className="text-xl text-gray-600 mb-8">
                Stop being just a dashboard. Become the subscription & billing engine 
                that SaaS founders build their business on.
              </p>
              <div className="flex flex-col sm:flex-row gap-4">
                <button className="px-8 py-4 bg-gradient-to-r from-primary to-secondary text-white rounded-lg font-medium flex items-center justify-center gap-2 hover:shadow-xl hover:shadow-primary/30 transition-all">
                  Start Building
                  <ArrowRight size={18} />
                </button>
                <button className="px-8 py-4 border border-gray-200 rounded-lg font-medium hover:border-primary hover:text-primary transition">
                  Talk to Sales
                </button>
              </div>
              <div className="flex items-center gap-6 mt-8 text-sm text-gray-500">
                <div className="flex items-center gap-2">
                  <CheckCircle size={16} className="text-primary" />
                  <span>No credit card required</span>
                </div>
                <div className="flex items-center gap-2">
                  <CheckCircle size={16} className="text-primary" />
                  <span>14-day free trial</span>
                </div>
              </div>
            </div>
            <div className="relative">
              <div className="absolute -top-20 -right-20 w-64 h-64 bg-primary/20 rounded-full blur-3xl"></div>
              <div className="absolute -bottom-20 -left-20 w-64 h-64 bg-secondary/20 rounded-full blur-3xl"></div>
              <div className="relative bg-white rounded-2xl shadow-2xl p-8 border border-gray-100">
                <div className="flex items-center gap-3 mb-6">
                  <div className="w-10 h-10 bg-gradient-to-br from-primary to-secondary rounded-lg"></div>
                  <div>
                    <div className="font-semibold">Acme SaaS</div>
                    <div className="text-sm text-gray-500">Enterprise Plan • Active</div>
                  </div>
                </div>
                <div className="space-y-4">
                  <div className="p-4 bg-gray-50 rounded-lg">
                    <div className="text-sm text-gray-500 mb-1">Monthly Revenue</div>
                    <div className="text-2xl font-bold">$124,592</div>
                    <div className="flex items-center gap-1 text-sm text-success mt-1">
                      <TrendingUp size={14} />
                      <span>+23.5% from last month</span>
                    </div>
                  </div>
                  <div className="grid grid-cols-2 gap-3">
                    <div className="p-3 bg-gray-50 rounded-lg">
                      <div className="text-xs text-gray-500">Active Users</div>
                      <div className="text-lg font-semibold">2,451</div>
                    </div>
                    <div className="p-3 bg-gray-50 rounded-lg">
                      <div className="text-xs text-gray-500">MRR/User</div>
                      <div className="text-lg font-semibold">$50.82</div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Value Proposition */}
      <section className="py-20 px-6 bg-white">
        <div className="max-w-7xl mx-auto">
          <div className="text-center mb-16">
            <h2 className="text-3xl md:text-4xl font-bold mb-4">
              Two billing layers. One powerful platform.
            </h2>
            <p className="text-xl text-gray-600 max-w-3xl mx-auto">
              Not just a dashboard. A complete subscription & billing engine 
              that handles the complexity your tenants don't want.
            </p>
          </div>

          <div className="grid md:grid-cols-2 gap-8">
            {/* Layer 1 */}
            <div className="p-8 rounded-2xl border border-gray-100 hover:border-primary/30 hover:shadow-xl transition-all group">
              <div className="w-12 h-12 bg-primary/10 rounded-xl flex items-center justify-center mb-6 group-hover:bg-primary/20 transition">
                <Shield className="text-primary" size={24} />
              </div>
              <h3 className="text-2xl font-bold mb-3">Layer 1: You → Tenant</h3>
              <p className="text-gray-600 mb-4 text-lg">Simple. Optional. Your subscription.</p>
              <ul className="space-y-3">
                {['Flat or usage-based pricing', 'Simple SaaS subscription', 'Can be faked in MVP'].map((item, i) => (
                  <li key={i} className="flex items-center gap-2 text-gray-600">
                    <CheckCircle size={16} className="text-primary" />
                    {item}
                  </li>
                ))}
              </ul>
            </div>

            {/* Layer 2 */}
            <div className="p-8 rounded-2xl bg-gradient-to-br from-primary/5 to-secondary/5 border-2 border-primary/20 hover:border-primary/40 hover:shadow-xl transition-all group">
              <div className="w-12 h-12 bg-gradient-to-br from-primary to-secondary rounded-xl flex items-center justify-center mb-6">
                <Users className="text-white" size={24} />
              </div>
              <h3 className="text-2xl font-bold mb-3">Layer 2: Tenant → End Users</h3>
              <p className="text-gray-600 mb-4 text-lg">The real value. The core pain.</p>
              <ul className="space-y-3">
                {[
                  'Define plans: Free → Common → Enterprise → Enterprise+',
                  'Attach features & limits to plans',
                  'Create checkout sessions',
                  'Handle webhooks & sync subscription state',
                  'Enforce entitlements automatically'
                ].map((item, i) => (
                  <li key={i} className="flex items-center gap-2 text-gray-600">
                    <CheckCircle size={16} className="text-secondary" />
                    {item}
                  </li>
                ))}
              </ul>
            </div>
          </div>
        </div>
      </section>

      {/* Features */}
      <section className="py-20 px-6">
        <div className="max-w-7xl mx-auto">
          <div className="text-center mb-16">
            <h2 className="text-3xl md:text-4xl font-bold mb-4">
              Everything your tenants need
            </h2>
            <p className="text-xl text-gray-600 max-w-3xl mx-auto">
              APIs, hosted checkout, and optional admin UI. 
              Let your tenants focus on their product.
            </p>
          </div>

          <div className="grid md:grid-cols-3 gap-8">
            {[
              {
                icon: <Zap className="text-primary" size={24} />,
                title: "APIs First",
                desc: "Powerful APIs that let your tenants build exactly what they need"
              },
              {
                icon: <BarChart3 className="text-primary" size={24} />,
                title: "Hosted Checkout",
                desc: "Beautiful, customizable checkout pages that convert"
              },
              {
                icon: <Shield className="text-primary" size={24} />,
                title: "Entitlement Enforcement",
                desc: "Automatic feature gating based on subscription status"
              }
            ].map((feature, i) => (
              <div key={i} className="p-6 rounded-xl border border-gray-100 hover:border-primary/30 hover:shadow-lg transition-all group">
                <div className="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center mb-4 group-hover:bg-primary/20 transition">
                  {feature.icon}
                </div>
                <h3 className="text-xl font-semibold mb-2">{feature.title}</h3>
                <p className="text-gray-600">{feature.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA */}
      <section className="py-20 px-6">
        <div className="max-w-4xl mx-auto text-center">
          <div className="bg-gradient-to-br from-primary to-secondary p-1 rounded-3xl">
            <div className="bg-white rounded-3xl p-12">
              <h2 className="text-3xl md:text-4xl font-bold mb-4">
                Ready to become a real platform?
              </h2>
              <p className="text-xl text-gray-600 mb-8">
                Stop being a nice internal tool. Start being worth integrating.
              </p>
              <button className="px-8 py-4 bg-gradient-to-r from-primary to-secondary text-white rounded-lg font-medium flex items-center gap-2 mx-auto hover:shadow-xl hover:shadow-primary/30 transition-all">
                Start building your billing engine
                <ArrowRight size={18} />
              </button>
            </div>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="border-t border-gray-100 py-8 px-6">
        <div className="max-w-7xl mx-auto flex flex-col md:flex-row justify-between items-center gap-4 text-sm text-gray-500">
          <div>© 2024 BillFlow. The subscription engine for SaaS founders.</div>
          <div className="flex gap-6">
            <a href="#" className="hover:text-primary transition">Terms</a>
            <a href="#" className="hover:text-primary transition">Privacy</a>
            <a href="#" className="hover:text-primary transition">Contact</a>
          </div>
        </div>
      </footer>
    </div>
  );
};

export default MainPage;