import { authContent } from '../../assets/Data/Platform/AuthLogin.js';

export default function Login() {
  const { login } = authContent;

  return (
    <div className="min-h-screen flex bg-white font-body">
      {/* Left Side: Form */}
      <div className="flex-1 flex flex-col justify-center px-8 md:px-16 lg:px-24">
        <div className="max-w-md w-full mx-auto">
          {/* Logo */}
          <div className="mb-10 text-2xl font-bold tracking-tighter text-primary font-heading">
            BillFlow<span className="text-slate-400">.io</span>
          </div>

          <h1 className="text-3xl font-extrabold font-heading text-slate-900 mb-2">
            {login.title}
          </h1>
          <p className="text-secondary mb-8">{login.subtitle}</p>

          <form className="space-y-5" onSubmit={(e) => e.preventDefault()}>
            <div className="space-y-2">
              <label className="text-sm font-bold text-slate-700">Work Email</label>
              <input 
                type="email" 
                placeholder="name@company.com"
                className="w-full px-4 py-3 rounded-button border border-slate-200 focus:border-primary focus:ring-4 focus:ring-primary/10 outline-none transition-base"
              />
            </div>

            <div className="space-y-2">
              <div className="flex justify-between items-center">
                <label className="text-sm font-bold text-slate-700">Password</label>
                <a href="#" className="text-xs font-bold text-primary hover:underline">
                  {login.forgotPassword}
                </a>
              </div>
              <input 
                type="password" 
                placeholder="••••••••"
                className="w-full px-4 py-3 rounded-button border border-slate-200 focus:border-primary focus:ring-4 focus:ring-primary/10 outline-none transition-base"
              />
            </div>

            <button className="w-full bg-primary text-white py-4 rounded-button font-bold shadow-lg shadow-indigo-100 hover:scale-[1.01] active:scale-[0.99] transition-base">
              Sign In
            </button>
          </form>

          {/* Social Login Divider */}
          <div className="relative my-8">
            <div className="absolute inset-0 flex items-center"><div className="w-full border-t border-slate-100"></div></div>
            <div className="relative flex justify-center text-xs uppercase font-bold text-slate-400">
              <span className="bg-white px-2">Or continue with</span>
            </div>
          </div>

          <button className="w-full border border-slate-200 text-slate-700 py-3 rounded-button font-bold flex items-center justify-center gap-2 hover:bg-slate-50 transition-base">
            <svg className="w-5 h-5" viewBox="0 0 24 24">
              <path fill="currentColor" d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" />
              <path fill="currentColor" d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" />
              <path fill="currentColor" d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l3.66-2.84z" />
              <path fill="currentColor" d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" />
            </svg>
            Google
          </button>

          <p className="mt-8 text-center text-sm text-secondary">
            {login.switchText}{' '}
            <a href={login.switchLink} className="text-primary font-bold hover:underline">
              Sign up
            </a>
          </p>
        </div>
      </div>

      {/* Right Side: Branding/Visual */}
      <div className="hidden lg:flex flex-1 bg-primary relative overflow-hidden flex-col justify-center px-16 text-white">
        {/* Abstract Background Decoration */}
        <div className="absolute top-0 right-0 w-96 h-96 bg-white/10 rounded-full -mr-20 -mt-20 blur-3xl"></div>
        <div className="absolute bottom-0 left-0 w-64 h-64 bg-indigo-900/20 rounded-full -ml-10 -mb-10 blur-3xl"></div>

        <div className="relative z-10">
          <blockquote className="text-3xl font-heading font-medium leading-relaxed mb-8">
            "{login.testimonial.quote}"
          </blockquote>
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 rounded-full bg-white/20 border border-white/40"></div>
            <div>
              <p className="font-bold">{login.testimonial.author}</p>
              <p className="text-white/60 text-sm">{login.testimonial.company}</p>
            </div>
          </div>
        </div>
        
        {/* Layer 2 Visual Teaser */}
        <div className="absolute bottom-12 left-16 right-16 p-6 bg-white/5 backdrop-blur-md rounded-card border border-white/10">
          <div className="flex justify-between items-center mb-4">
            <div className="text-xs font-bold uppercase tracking-widest text-indigo-200">System Entitlements</div>
            <div className="h-2 w-8 bg-success rounded-full"></div>
          </div>
          <div className="space-y-2 opacity-50">
            <div className="h-1.5 w-full bg-white/20 rounded-full"></div>
            <div className="h-1.5 w-3/4 bg-white/20 rounded-full"></div>
          </div>
        </div>
      </div>
    </div>
  );
}