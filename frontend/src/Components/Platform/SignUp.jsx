import { useState } from 'react';
import { signupContent } from '../../assets/Data/Platform/SignUp';

export default function SignupFlow() {
  const [step, setStep] = useState(1); // 1: Pricing, 2: Auth, 3: Verification
  const [selectedPlan, setSelectedPlan] = useState(null);
  const [email, setEmail] = useState('');

  const nextStep = () => setStep(step + 1);

  return (
    <div className="min-h-screen bg-slate-50 font-body py-12 px-6">
      <div className="max-w-7xl mx-auto">
        
        {/* Progress Bar */}
        <div className="max-w-md mx-auto mb-12 flex items-center justify-between">
          {[1, 2, 3].map((num) => (
            <div key={num} className="flex items-center">
              <div className={`w-8 h-8 rounded-full flex items-center justify-center font-bold text-xs transition-base ${
                step >= num ? 'bg-primary text-white' : 'bg-slate-200 text-slate-500'
              }`}>
                {num}
              </div>
              {num < 3 && <div className={`w-20 h-1 mx-2 ${step > num ? 'bg-primary' : 'bg-slate-200'}`} />}
            </div>
          ))}
        </div>

        {/* STEP 1: Plan Selection */}
        {step === 1 && (
          <div className="animate-in fade-in slide-in-from-bottom-4 duration-500">
            <h1 className="text-4xl font-extrabold font-heading text-center mb-4">Choose your engine</h1>
            <p className="text-secondary text-center mb-12">Select a plan to start building your billing infrastructure.</p>
            
            <div className="grid md:grid-cols-3 gap-8 max-w-6xl mx-auto">
              {signupContent.plans.map((plan) => (
                <div key={plan.id} className={`p-card rounded-card border-2 bg-white flex flex-col transition-base ${
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

        {/* STEP 2: Account Creation / Google Auth */}
        {step === 2 && (
          <div className="max-w-md mx-auto bg-white p-card rounded-card shadow-card border border-slate-100 animate-in fade-in zoom-in-95">
            <h2 className="text-2xl font-bold font-heading mb-2">Create your account</h2>
            <p className="text-sm text-secondary mb-8">Selected Plan: <span className="text-primary font-bold uppercase">{selectedPlan}</span></p>
            
            <button className="w-full border border-slate-200 py-3 rounded-button font-bold flex items-center justify-center gap-2 hover:bg-slate-50 mb-6 transition-base">
              <img src="https://www.google.com/favicon.ico" className="w-4 h-4" alt="google" />
              Continue with Google
            </button>

            <div className="relative mb-6">
              <div className="absolute inset-0 flex items-center"><div className="w-full border-t border-slate-100"></div></div>
              <span className="relative bg-white px-2 text-[10px] font-bold text-slate-400 uppercase left-1 -translate-x-1">Or use email</span>
            </div>

            <div className="space-y-4">
              <input 
                type="email" 
                placeholder="Work email" 
                className="w-full px-4 py-3 rounded-button border border-slate-200 outline-none focus:border-primary"
                onChange={(e) => setEmail(e.target.value)}
              />
              <button onClick={nextStep} className="w-full bg-primary text-white py-3 rounded-button font-bold">
                Continue
              </button>
            </div>
          </div>
        )}

        {/* STEP 3: 6-Digit Verification */}
        {step === 3 && (
          <div className="max-w-md mx-auto bg-white p-card rounded-card shadow-card border border-slate-100 text-center animate-in fade-in zoom-in-95">
            <div className="w-16 h-16 bg-primary/10 text-primary rounded-full flex items-center justify-center mx-auto mb-6">
              <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/></svg>
            </div>
            <h2 className="text-2xl font-bold font-heading mb-2">Verify your email</h2>
            <p className="text-sm text-secondary mb-8">We sent a unique code to <span className="font-bold text-slate-900">{email}</span></p>

            <div className="flex justify-center gap-2 mb-8">
              {[1, 2, 3, 4, 5, 6].map((i) => (
                <input key={i} id={`input-${i}`} type="text" maxLength="1" 
              onKeyDown={(key)=>{

                 if((key.target.value.length === 1) && key.key!=='Backspace' ){
                      const nextInput = document.getElementById(`input-${i+1}`);
                      if(nextInput) nextInput.focus();
                    }
                   
                    else if((key.target.value.length === 0&& i > 1) && key.key === 'Backspace' ){
                      const prevInput = document.getElementById(`input-${i-1}`);
                        key.target.value = '';
                      if(prevInput) prevInput.focus();
                    }
              }}
                
                className="w-12 h-14 text-center text-xl font-bold border-2 border-slate-200 rounded-lg focus:border-primary outline-none" />
              ))}
            </div>

            <p className="text-xs text-danger font-bold mb-6">Code expires in 0:59</p>
            
            <button 
              onClick={() => window.location.href = '/dashboard'} // Assuming success
              className="w-full bg-primary text-white py-4 rounded-button font-bold shadow-lg"
            >
              Verify & Create Tenant
            </button>
          </div>
        )}
      </div>
    </div>
  );
}