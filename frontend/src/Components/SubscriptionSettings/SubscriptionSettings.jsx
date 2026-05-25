import React from "react";

export default function SubscriptionSettings() {
  return (
    <div className="space-y-6 animate-in fade-in duration-200">
      <div>
        <h2 className="text-2xl font-extrabold text-slate-900 tracking-tight">Subscription Settings</h2>
        <p className="text-sm text-slate-500">Modify tier configuration parameters, limits boundaries, and corporate package scopes.</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        {[
          { tier: "Startup Tier", cost: "$49/mo", limits: "Max 50 operations/day", feature: "Standard Webhooks access" },
          { tier: "Enterprise Scale", cost: "$199/mo", limits: "Unlimited calculations", feature: "Bifurcated Token security architecture" },
          { tier: "Custom Enterprise", cost: "Custom", limits: "Dedicated infrastructure layout", feature: "Full API configurations control" }
        ].map((pkg, i) => (
          <div key={i} className="bg-white p-6 border border-slate-200/60 rounded-2xl shadow-sm flex flex-col justify-between space-y-6">
            <div className="space-y-2">
              <h3 className="font-bold text-slate-900 text-lg">{pkg.tier}</h3>
              <p className="text-2xl font-black text-indigo-600">{pkg.cost}</p>
              <hr className="border-slate-100 my-3" />
              <ul className="space-y-2 text-xs font-semibold text-slate-500">
                <li className="flex items-center gap-2 text-slate-600">
                  <svg className="h-4 w-4 text-indigo-500" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="3"><path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" /></svg>
                  {pkg.limits}
                </li>
                <li className="flex items-center gap-2 text-slate-600">
                  <svg className="h-4 w-4 text-indigo-500" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="3"><path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" /></svg>
                  {pkg.feature}
                </li>
              </ul>
            </div>
            <button className="w-full py-2 bg-slate-50 hover:bg-slate-100 border border-slate-200 rounded-xl text-xs font-bold text-slate-700 transition-all outline-none">
              Modify Parameters
            </button>
          </div>
        ))}
      </div>
    </div>
  );
}