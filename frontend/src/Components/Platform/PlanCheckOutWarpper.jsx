import React from 'react';
import CheckoutForm from './CheckoutForm'; // Assuming CheckoutForm is in the same directory
import { CheckoutElementsProvider } from '@stripe/react-stripe-js/checkout';
import { loadStripe } from '@stripe/stripe-js';

export default function PlanCheckoutWrapper({ plan, clientSecret }) {
  const stripe = loadStripe("pk_test_51Sc6KUPuRhnpDysBCK0h0Q1qN3pWrCnIBGiIUizOenfIst8QSyUjERqR9j6oCexqmd4c2ws5n1MRnYTIq5YdMVro002iSUNdBw");
 console.log(clientSecret);
  if (!plan) {
    return (
      <div className="w-full max-w-4xl mx-auto p-6 text-center text-xs font-bold text-slate-400">
        No implementation plan data provided.
      </div>
    );
  }

  // Extract the main price metric to show on the summary page
  const primaryPricing = plan.planPricingOptions?.[0] || plan.planPricingOptions?.find(p => p.isActive);

  return (
    <div className="w-full max-w-5xl mx-auto p-6">
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-start">
        
        {/* LEFT COLUMN: PLAN METADATA & SUMMARY (DTO INFRASTRUCTURE) */}
        <div className="lg:col-span-5 space-y-6">
          <div className="bg-white border border-slate-200/80 rounded-2xl p-6 shadow-xs space-y-4">
            <div>
              <span className="text-[10px] font-black text-blue-600 uppercase tracking-wider block">
                Selected Configuration
              </span>
              <h2 className="text-2xl font-black text-slate-900 tracking-tight mt-0.5">
                {plan.name}
              </h2>
              {plan.description && (
                <p className="text-xs font-medium text-slate-400 mt-1 leading-relaxed">
                  {plan.description}
                </p>
              )}
            </div>

            {/* Price Visualization */}
            {primaryPricing && (
              <div className="bg-slate-50 border border-slate-100 rounded-xl p-4 flex justify-between items-center">
                <span className="text-xs font-bold text-slate-500 uppercase">Subtotal Cycle Cost</span>
                <div className="text-right">
                  <span className="text-xl font-mono font-black text-slate-800">
                    {primaryPricing.amount} {primaryPricing.currency || 'USD'}
                  </span>
                </div>
              </div>
            )}

            {/* Dynamic Benefits Loop */}
            {plan.benefits && plan.benefits.length > 0 && (
              <div className="space-y-2 pt-2 border-t border-slate-100">
                <h4 className="text-[10px] font-black text-slate-400 uppercase tracking-wider">
                  Included Allocation Privileges
                </h4>
                <ul className="space-y-2">
                  {plan.benefits.map((benefit, index) => (
                    <li key={benefit.id || index} className="flex items-start gap-2 text-xs font-semibold text-slate-600">
                      <svg className="w-4 h-4 text-emerald-500 mt-0.5 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="3" d="M5 13l4 4L19 7" />
                      </svg>
                      <span>{benefit.name || benefit.description}</span>
                    </li>
                  ))}
                </ul>
              </div>
            )}
            
            {/* Free Try Option Badge */}
            {plan.hasFreeTryOption && (
              <div className="bg-emerald-50/60 border border-emerald-100 rounded-xl p-3 flex items-center gap-2">
                <span className="h-1.5 w-1.5 rounded-full bg-emerald-500" />
                <p className="text-[11px] font-bold text-emerald-800">
                  Includes a standard risk-free introductory period trial profile.
                </p>
              </div>
            )}
          </div>
        </div>

        {/* RIGHT COLUMN: SECURE STRIPE CHECKOUT SURFACE */}
        <div className="lg:col-span-7">
          {clientSecret&&stripe ? (
            <CheckoutElementsProvider stripe={ stripe } options={{clientSecret:clientSecret}}   >
                   <CheckoutForm />
</CheckoutElementsProvider>
          ) : (
            <div className="w-full bg-white border border-slate-200/80 rounded-2xl p-8 text-center space-y-3 shadow-xs flex flex-col items-center justify-center min-h-[300px]">
              <div className="w-10 h-10 rounded-full bg-slate-50 flex items-center justify-center text-slate-400">🔒</div>
              <p className="text-xs font-bold text-slate-700">Awaiting Stripe Cryptographic Payload</p>
              <p className="text-[11px] text-slate-400 font-medium max-w-xs">
                The gateway authorization state parameter has not finalized parsing from your upstream deployment.
              </p>
            </div>
          )}
        </div>

      </div>
    </div>
  );
}