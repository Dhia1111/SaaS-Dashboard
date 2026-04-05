import { useState } from 'react';
import { signupContent } from '../../assets/Data/Platform/SignUp';

export default function SignupFlow() {
  const [step, setStep] = useState(1);
  const [selectedPlan, setSelectedPlan] = useState(null);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [otp, setOtp] = useState("");
  
  // Virtual Payment State
  const [paymentData, setPaymentData] = useState({
    location: '',
    postCode: '',
    cardHolder: ''
  });

  const nextStep = () => setStep(step + 1);
  const prevStep = () => setStep(step - 1);

  // --- VALIDATION LOGIC ---
  function validateEmail(email) {
    return /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(email.toLowerCase());
  }

  const isAuthValid = validateEmail(email) && password.length >= 8;

  const handleVerificationSuccess = () => {
    if (selectedPlan === 'free') {
      // Logic for creating tenant and redirecting
      window.location.href = '/dashboard';
    } else {
      nextStep(); // Move to Step 4: Payment
    }
  };

  return (
    <div className="min-h-screen bg-slate-50 font-body py-12 px-6">
      <div className="max-w-7xl mx-auto">
        
        {/* Progress Bar (4 Steps) */}
        <div className="max-w-md mx-auto mb-12 flex items-center justify-between">
          {[1, 2, 3, 4].map((num) => (
            <div key={num} className="flex items-center ">
              <div className={`w-8 h-8 rounded-full flex items-center justify-center font-bold text-xs transition-base ${
                step >= num ? 'bg-primary text-white' : 'bg-slate-200 text-slate-500'
              }`}>
                {num}
              </div>
              {num < 4 && <div className={`w-14 h-1 mx-2 ${step > num ? 'bg-primary' : 'bg-slate-200'}`} />}
            </div>
          ))}
        </div>

        {/* STEP 1: Plan Selection */}
        {step === 1 && (
          <div className="animate-in fade-in slide-in-from-bottom-4 duration-500 text-center">
            <h1 className="text-4xl font-extrabold font-heading mb-4 text-slate-900">Choose your engine</h1>
            <p className="text-secondary mb-12">Select a plan to start building your billing infrastructure.</p>
            <div className="grid md:grid-cols-3 gap-8 max-w-6xl mx-auto">
              {signupContent.plans.map((plan) => (
                <div key={plan.id} className={`p-card rounded-card border-2 bg-white flex flex-col text-left transition-base ${
                  plan.highlight ? 'border-primary shadow-xl scale-105' : 'border-slate-100'
                }`}>
                  <h3 className="text-xl font-bold font-heading mb-2">{plan.name}</h3>
                  <div className="mb-6">
                    <span className="text-4xl font-black">{plan.price}</span>
                    <span className="text-secondary text-sm ml-2">/ {plan.duration}</span>
                  </div>
                  <ul className="space-y-4 mb-8 flex-1">
                    {plan.features.map(f => (
                      <li key={f} className="text-sm text-slate-600 flex items-center gap-2">
                        <svg className="w-4 h-4 text-success" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="3" d="M5 13l4 4L19 7"/></svg>
                        {f}
                      </li>
                    ))}
                  </ul>
                  <button 
                    onClick={() => { setSelectedPlan(plan.id); nextStep(); }}
                    className={`w-full py-3 rounded-button font-bold transition-base ${
                      plan.highlight ? 'bg-primary text-white shadow-lg shadow-indigo-200' : 'bg-slate-100 text-slate-900 hover:bg-slate-200'
                    }`}
                  >
                    {plan.buttonText}
                  </button>
                </div>
              ))}
            </div>
          </div>
        )}
 {/* STEP 2: Auth (With Google & Validation) */}
{step === 2 && (
  <div className="max-w-md mx-auto bg-white p-card rounded-card shadow-card border border-slate-100 animate-in fade-in zoom-in-95">
    {/* Navigation */}
    <button onClick={prevStep} className="text-xs font-bold text-slate-400 mb-6 hover:text-primary transition-base flex items-center gap-1">
      <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="3" d="M15 19l-7-7 7-7"/></svg>
      Back to plans
    </button>

    <h2 className="text-2xl font-bold font-heading mb-2 text-slate-900">Create your account</h2>
    <p className="text-sm text-secondary mb-8 leading-relaxed">
      Join BillFlow for your <span className="text-primary font-bold uppercase">{selectedPlan}</span> infrastructure.
    </p>

    <div className="space-y-6">
      {/* Google Sign-In Button */}
      <button 
        onClick={() => {
          // Trigger Google Auth Logic here
          console.log("Initiating Google Sign-In");
        }}
        className="w-full flex items-center justify-center gap-3 py-3 px-4 rounded-button border border-slate-200 font-bold text-slate-700 hover:bg-slate-50 hover:border-slate-300 transition-all active:scale-[0.98]"
      >
        <svg className="w-5 h-5" viewBox="0 0 24 24">
          <path fill="#4285F4" d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"/>
          <path fill="#34A853" d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"/>
          <path fill="#FBBC05" d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l3.66-2.84z"/>
          <path fill="#EA4335" d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"/>
        </svg>
        Continue with Google
      </button>

      {/* Modern Divider */}
      <div className="relative">
        <div className="absolute inset-0 flex items-center">
          <div className="w-full border-t border-slate-100"></div>
        </div>
        <div className="relative flex justify-center text-[10px] uppercase font-black tracking-widest text-slate-400">
          <span className="bg-white px-4">Or use work email</span>
        </div>
      </div>

      {/* Manual Email/Password Form */}
      <div className="space-y-4">
        <div className="space-y-1">
          <label className="text-[11px] font-bold text-slate-500 uppercase ml-1">Email Address</label>
          <input 
            type="email" 
            placeholder="name@company.com" 
            className="w-full px-4 py-3 rounded-button border border-slate-200 outline-none focus:border-primary focus:ring-4 focus:ring-primary/10 transition-all"
            onChange={(e) => setEmail(e.target.value)}
          />
        </div>

        <div className="space-y-1">
          <label className="text-[11px] font-bold text-slate-500 uppercase ml-1">Password</label>
          <input 
            type="password" 
            placeholder="Min. 8 characters" 
            className="w-full px-4 py-3 rounded-button border border-slate-200 outline-none focus:border-primary focus:ring-4 focus:ring-primary/10 transition-all"
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>

        <button 
          disabled={!isAuthValid}
          onClick={nextStep} 
          className={`w-full py-4 rounded-button font-bold transition-all shadow-lg ${
            isAuthValid 
              ? 'bg-primary text-white shadow-indigo-100 hover:brightness-110 active:scale-[0.98]' 
              : 'bg-slate-100 text-slate-400 cursor-not-allowed shadow-none'
          }`}
        >
          Verify Account
        </button>
      </div>

      <p className="text-center text-[11px] text-slate-400 px-6">
        By continuing, you agree to BillFlow's <a href="/terms" className="underline hover:text-primary">Terms of Service</a> and <a href="/privacy" className="underline hover:text-primary">Privacy Policy</a>.
      </p>
    </div>
  </div>
)}
{/* STEP 3: Email Verification */}
{step === 3 && (
  <div className="max-w-md mx-auto bg-white p-card rounded-card shadow-card border border-slate-100 text-center animate-in fade-in zoom-in-95">
    {/* Icon Header */}
    <div className="w-16 h-16 bg-primary/10 text-primary rounded-full flex items-center justify-center mx-auto mb-6 shadow-inner">
      <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/>
      </svg>
    </div>

    <h2 className="text-2xl font-bold font-heading mb-2 text-slate-900">Verify your identity</h2>
    <p className="text-sm text-secondary mb-8 px-4">
      We've sent an 8-digit security code to <br/>
      <span className="font-bold text-slate-900 underline decoration-primary/30 decoration-2">{email}</span>
    </p>

    {/* OTP Input Group */}
    <div className="mb-8 px-4">
      <input
        type="text"
        placeholder="Enter Code"
        className="w-full h-16 text-center text-3xl font-black tracking-[0.5em] border-2 border-slate-100 rounded-xl focus:border-primary focus:ring-4 focus:ring-primary/10 outline-none transition-all placeholder:text-slate-200 placeholder:tracking-normal"
        onChange={(e) => setOtp(e.target.value)}
      />
      <div className="flex justify-between mt-3 px-1">
        <p className="text-[10px] font-bold uppercase tracking-widest text-danger animate-pulse">
          Code expires in 0:59
        </p>
        <p className="text-[10px] font-bold uppercase tracking-widest text-slate-400">
          Secure Session
        </p>
      </div>
    </div>

    {/* Verify Button */}
    <button 
      onClick={handleVerificationSuccess}
      className="w-full bg-primary text-white py-4 rounded-button font-bold shadow-lg shadow-indigo-100 hover:brightness-110 active:scale-[0.98] transition-all mb-8"
    >
      {selectedPlan === 'free' ? 'Finalize & Launch' : 'Verify & Continue'}
    </button>

    {/* Modern Resend Section */}
    <div className="pt-6 border-t border-slate-50">
      <p className="text-sm text-secondary mb-3">Didn't receive the code?</p>
      
      <div className="inline-flex items-center gap-2 p-1 px-4 bg-slate-50 border border-slate-100 rounded-full">
        <button 
          className="text-xs font-bold text-primary hover:text-indigo-700 transition-colors py-1 disabled:text-slate-400 disabled:cursor-not-allowed"
          onClick={() => {
            // Logic to trigger API resend
            console.log("Resending to:", email);
          }}
        >
          Resend Code
        </button>
        <span className="h-3 w-px bg-slate-200"></span>
        <span className="text-[10px] font-mono font-bold text-slate-500">Wait 54s</span>
      </div>
      
      <p className="mt-4 text-[11px] text-slate-400 max-w-[220px] mx-auto leading-tight">
        Make sure to check your <span className="font-semibold">spam folder</span> if you don't see it in a minute.
      </p>
    </div>
  </div>
)}

        {/* STEP 4: Payment (Final Step for Paid Plans) */}
        {step === 4 && (
          <div className="max-w-md mx-auto bg-white p-card rounded-card shadow-card border border-slate-100 animate-in fade-in zoom-in-95">
            <h2 className="text-2xl font-bold font-heading mb-2">Finalize Payment</h2>
            <p className="text-sm text-secondary mb-8 italic">Verified: {email}</p>
            <div className="space-y-4">
              <div className="p-4 bg-slate-50 rounded-lg border border-slate-200 flex items-center justify-between">
                <span className="text-sm font-bold text-slate-700">{selectedPlan} Plan</span>
                <span className="text-primary font-black text-lg">{signupContent.plans.find(p => p.id === selectedPlan)?.price}</span>
              </div>
              <input 
                type="text" 
                placeholder="Cardholder Name" 
                className="w-full px-4 py-3 rounded-button border border-slate-200 outline-none focus:border-primary"
                onChange={(e) => setPaymentData({...paymentData, cardHolder: e.target.value})}
              />
              <div className="grid grid-cols-2 gap-4">
                <input 
                  type="text" 
                  placeholder="Country" 
                  className="w-full px-4 py-3 rounded-button border border-slate-200 outline-none focus:border-primary"
                  onChange={(e) => setPaymentData({...paymentData, location: e.target.value})}
                />
                <input 
                  type="text" 
                  placeholder="Zip / Post Code" 
                  className="w-full px-4 py-3 rounded-button border border-slate-200 outline-none focus:border-primary"
                  onChange={(e) => setPaymentData({...paymentData, postCode: e.target.value})}
                />
              </div>
              <button 
                onClick={() => window.location.href = '/dashboard'}
                className="w-full bg-primary text-white py-4 mt-4 rounded-button font-bold shadow-lg hover:shadow-indigo-200 transition-all"
              >
                Start Subscription
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}