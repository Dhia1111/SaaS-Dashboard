import React, { useState } from "react";
import { useNavigate, useLoaderData } from "react-router-dom";
import { GetSubscriptionListAsync, DeleteTenantSubscriptionAsync } from "../../Apis/Subscriotions.js";
import { GetPricingCyclesAsync } from "../../Apis/PricingCycles.js";
import { GetAuthorizationOptionsAsync } from "../../Apis/Users.js";
import TenantPermissionsList from "../EndPointsPermissionsManagement/PermissionsList.jsx";
import { TenantPermissionsListAsync } from "../../Apis/TenantPermissions.js";
 
export default function SubscriptionSettings() {
  const navigate = useNavigate();
  const { CyclesList, SubscriptionList, Permissions } = useLoaderData(); 

  // Local state management for real-time reactivity
  const [subscriptions, setSubscriptions] = useState(SubscriptionList || []);
  const [activeDetailsPlan, setActiveDetailsPlan] = useState(null);
  const [isDeletingId, setIsDeletingId] = useState(null);
  const [actionStatus, setActionStatus] = useState({ type: "", text: "" });

  // --- UI Action Handlers ---
  const handleDeletePlan = async (planId) => {
    if (!window.confirm("Are you absolutely sure you want to delete this subscription plan structure?")) return;
    
    setIsDeletingId(planId);
    setActionStatus({ type: "", text: "" });

    try {
      const response = await DeleteTenantSubscriptionAsync(planId);
      if (response.success) {
        // ✅ Updated comparison parameter pattern for flat layout mapping tracking
        setSubscriptions(prev => prev.filter(item => item.id !== planId));
        setActionStatus({ type: "success", text: "Subscription tier deleted successfully." });
      } else {
        setActionStatus({ type: "error", text: response.message || "Failed to drop subscription data tier." });
      }
    } catch {
      setActionStatus({ type: "error", text: "Network communication failure encountered." });
    } finally {
      setIsDeletingId(null);
    }
  };

  return (
    <div className="space-y-6 animate-in fade-in duration-200 p-4 sm:p-6 max-w-7xl mx-auto">
      
      {/* Header Panel */}
      <div>
        <h2 className="text-2xl font-extrabold text-slate-900 tracking-tight">Subscription Architecture Settings</h2>
        <p className="text-sm text-slate-500 mt-1">Modify multi-tenant tier configuration parameters, feature limits, and entitlement matrices.</p>
      </div>
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 border-b border-slate-100 pb-5">
        <button 
          onClick={() => navigate("add")}
          className="inline-flex items-center justify-center px-4 py-2.5 bg-blue-600 text-white font-bold text-sm rounded-xl shadow-md shadow-blue-600/10 hover:bg-blue-700 transition-all outline-none whitespace-nowrap self-start sm:self-center"
        >
          + Add New Subscription
        </button>
      </div>

      {/* Dynamic Messaging Block */}
      {actionStatus.text && (
        <div className={`p-4 rounded-xl text-sm font-semibold border ${
          actionStatus.type === "error" ? "bg-red-50 text-red-700 border-red-100" : "bg-emerald-50 text-emerald-700 border-emerald-100"
        }`}>
          {actionStatus.text}
        </div>
      )}

      {/* Main Subscriptions Loop */}
      {subscriptions.length === 0 ? (
        <div className="text-center p-12 border border-dashed border-slate-200 rounded-2xl bg-white text-slate-400 font-medium text-sm">
          No live active subscriptions mapped to this cluster tenant instance.
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {subscriptions.map((pkg, idx) => {
            // ✅ Updated references extraction straight from core parent block parameters
            const planId = pkg?.id || idx;
            const planName = pkg?.name || "Unnamed Plan";
            const planDescription = pkg?.description || "No core presentation summary notes loaded for this profile configuration.";
            const isPlanActive = pkg?.isActive;
            const primaryPrice = pkg?.planPricingOptions?.[0];
            const displayBenefits = Array.isArray(pkg?.benefits) ? pkg.benefits : [];

            return (
              <div key={planId} className={`bg-white border transition-all rounded-2xl shadow-sm flex flex-col justify-between overflow-hidden group ${
                isPlanActive ? "border-slate-200/80 hover:border-blue-300" : "border-slate-200 opacity-75"
              }`}>
                
                {/* Card Context Content */}
                <div className="p-6 space-y-4">
                  <div className="flex justify-between items-start gap-2">
                    <div>
                      <span className={`inline-flex px-2 py-0.5 rounded-md font-bold text-[10px] tracking-wider uppercase mb-1.5 ${
                        isPlanActive ? "bg-emerald-50 text-emerald-700" : "bg-slate-100 text-slate-500"
                      }`}>
                        {isPlanActive ? "Active" : "Disabled"}
                      </span>
                      <h3 className="font-bold text-slate-900 text-lg leading-snug">{planName}</h3>
                    </div>
                    
                    {/* Core Rates Configuration Variant Processing Layout */}
                    <div className="text-right shrink-0">
                      <p className="text-xl font-black text-blue-600">
                        {/* ✅ Property field modified variant tracking check mapping to .amount */}
                        {primaryPrice ? `$${Number(primaryPrice.amount).toFixed(2)}` : "Free"}
                      </p>
                      <p className="text-[10px] font-bold text-slate-400 tracking-tight">
                        {primaryPrice ? `${primaryPrice.currency} variant` : "No cost tier"}
                      </p>
                    </div>
                  </div>

                  <p className="text-xs text-slate-500 line-clamp-2 font-medium min-h-[32px]">
                    {planDescription}
                  </p>
                  
                  <hr className="border-slate-100" />
                  
                  {/* Sneak Peek Benefits Grid Subsegment */}
                  <div className="space-y-2">
                    <span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">Key Highlights</span>
                    <ul className="space-y-2 text-xs font-semibold text-slate-600">
                      {displayBenefits.slice(0, 2).map((benefit, bIdx) => (
                        <li key={benefit.id || bIdx} className="flex items-start gap-2 line-clamp-1">
                          <span className="h-1.5 w-1.5 bg-emerald-500 rounded-full mt-1.5 shrink-0" />
                          <span>{benefit.title}</span>
                        </li>
                      ))}
                      {displayBenefits.length === 0 && (
                        <span className="text-slate-400 font-normal italic">No baseline markers.</span>
                      )}
                      {displayBenefits.length > 2 && (
                        <li className="text-[11px] text-blue-600 font-bold pl-3.5">
                          + {displayBenefits.length - 2} additional benefits items
                        </li>
                      )}
                    </ul>
                  </div>
                </div>

                {/* Operations Control Actions Panel */}
                <div className="p-4 bg-slate-50 border-t border-slate-100 grid grid-cols-3 gap-2">
                  <button 
                    type="button"
                    onClick={() => setActiveDetailsPlan(pkg)}
                    className="py-2 px-1 bg-white hover:bg-slate-100 text-slate-700 font-bold text-xs rounded-xl border border-slate-200 transition-all outline-none text-center"
                  >
                    Details
                  </button>
                  <button 
                    type="button"
                    onClick={() => navigate(`edit?id=${planId}`, { state: { payload: pkg } })}
                    className="py-2 px-1 bg-white hover:bg-blue-50 hover:text-blue-600 hover:border-blue-200 text-slate-700 font-bold text-xs rounded-xl border border-slate-200 transition-all outline-none text-center"
                  >
                    Update
                  </button>
                  <button 
                    type="button"
                    disabled={isDeletingId === planId}
                    onClick={() => handleDeletePlan(planId)}
                    className="py-2 px-1 bg-red-50 hover:bg-red-100 text-red-600 font-bold text-xs rounded-xl border border-red-100 transition-all outline-none text-center disabled:opacity-50"
                  >
                    {isDeletingId === planId ? "Dropping..." : "Delete"}
                  </button>
                </div>

              </div>
            );
          })}
        </div>
      )}

      {/* --- OVERLAY MODAL: Show Subscription Configuration Details --- */}
      {activeDetailsPlan && (
        <div className="fixed inset-0 bg-slate-900/60 backdrop-blur-xs flex items-center justify-center z-50 p-4 animate-in fade-in duration-150">
          <div className="bg-white rounded-2xl w-full max-w-2xl max-h-[85vh] overflow-hidden shadow-2xl border border-slate-100 flex flex-col scale-in duration-200">
            
            {/* Modal Top Branding Profile Panel */}
            <header className="bg-slate-900 text-white p-6 flex justify-between items-start shrink-0">
              <div>
                <span className="text-[10px] bg-white/10 text-white/80 border border-white/10 px-2 py-0.5 rounded font-bold uppercase tracking-widest">
                  Comprehensive Suite Profile Mapping
                </span>
                <h3 className="text-xl font-black tracking-tight mt-1">{activeDetailsPlan?.name}</h3>
              </div>
              <button 
                type="button" 
                onClick={() => setActiveDetailsPlan(null)}
                className="text-white/60 hover:text-white bg-white/10 hover:bg-white/20 h-8 w-8 rounded-full flex items-center justify-center font-bold text-sm transition-all"
              >
                ✕
              </button>
            </header>

            {/* Modal Body */}
            <div className="p-6 space-y-6 overflow-y-auto text-sm text-slate-700">
              
              {/* Description Mapping Block */}
              <div className="space-y-1">
                <h4 className="text-xs font-bold text-slate-400 uppercase tracking-wider">Plan Sub-Text Description</h4>
                <p className="bg-slate-50 p-3.5 rounded-xl border border-slate-100 font-medium text-slate-700 text-xs leading-relaxed">
                  {activeDetailsPlan?.description || "No copywriting details explicitly structured onto this schema layout model."}
                </p>
              </div>

              {/* Dynamic Pricing Cycles Grid Data Configuration View */}
              <div className="space-y-2">
                <h4 className="text-xs font-bold text-slate-400 uppercase tracking-wider">Pricing Cycles & Charges Configuration</h4>
                <div className="border border-slate-100 rounded-xl overflow-hidden">
                  <table className="w-full text-left text-xs border-collapse">
                    <thead className="bg-slate-50 font-bold text-slate-500 border-b border-slate-100">
                      <tr>
                        <th className="p-3">Pricing Cycle</th>
                        <th className="p-3">Base Operational Rate</th>
                        <th className="p-3 text-center">Status</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100 font-semibold text-slate-700">
                      {activeDetailsPlan.planPricingOptions?.map((opt, oIdx) => (
                        <tr key={opt.id || oIdx} className="hover:bg-slate-50/50">
                          <td className="p-3 font-mono text-slate-500">
                            Cycle: {CyclesList.find(c => Number(c.id) === Number(opt.tenantPricingCycleId))?.cycleName || `ID ${opt.tenantPricingCycleId}`}
                          </td>
                          {/* ✅ Property field references modified variant parsing track change to .amount */}
                          <td className="p-3 font-bold text-blue-600">{Number(opt.amount).toFixed(2)} {opt.currency}</td>
                          <td className="p-3 text-center">
                            <span className={`px-2 py-0.5 rounded font-bold text-[10px] ${opt.isActive ? "bg-emerald-50 text-emerald-700" : "bg-red-50 text-red-700"}`}>
                              {opt.isActive ? "Live" : "Inactive"}
                            </span>
                          </td>
                        </tr>
                      )) || (
                        <tr>
                          <td colSpan={3} className="text-center p-3 text-slate-400 italic">No cycle variations matched.</td>
                        </tr>
                      )}
                    </tbody>
                  </table>
                </div>
              </div>

              {/* Benefits Bullets Layout Mapping */}
              <div className="space-y-2">
                {/* ✅ Adjusted child parsing targets list pointer down to .benefits array collection wrapper */}
                <h4 className="text-xs font-bold text-slate-400 uppercase tracking-wider">
                  Entitled Benefits Bullet Highlight Array ({activeDetailsPlan.benefits?.length || 0})
                </h4>
                <div className="space-y-2 max-h-[160px] overflow-y-auto pr-1">
                  {activeDetailsPlan.benefits?.map((benefit, idx) => (
                    <div key={benefit.id || idx} className="p-3 border border-slate-100 rounded-xl bg-slate-50/40">
                      <h5 className="font-bold text-xs text-slate-900 flex items-center gap-2">
                        <span className="h-1.5 w-1.5 bg-blue-600 rounded-full" /> {benefit.title}
                      </h5>
                      {benefit.description && (
                        <p className="text-[11px] text-slate-500 font-normal mt-0.5 ml-3.5">{benefit.description}</p>
                      )}
                    </div>
                  )) || <p className="text-xs text-slate-400 italic">No feature bullet entries found.</p>}
                </div>
              </div>

              {/* Permission Scopes Flag Access Mapping */}
              <div className="space-y-2">
                <h4 className="text-xs font-bold text-slate-400 uppercase tracking-wider">Authorized Permission Nodes Scope Indices</h4>
                {activeDetailsPlan.permissions?.length > 0 ? (
                  <div className="flex flex-wrap gap-1.5">
                    {activeDetailsPlan.permissions.map((perm, pIdx) => (
                      <span key={perm.id || pIdx} className="font-mono text-[10px] font-bold bg-blue-50 text-blue-700 border border-blue-100 px-2 py-1 rounded-md">
                        Permission: {Permissions?.find(o => Number(o.id) === Number(perm.permissionId))?.permissionKey || `Key ID ${perm.permissionId}`}
                      </span>
                    ))}
                  </div>
                ) : (
                  <p className="text-xs text-slate-400 italic">Zero explicit access permissions bindings map directly attached to this payload cluster node context.</p>
                )}
              </div>

            </div>

            {/* Modal Bottom Close Panel Action Block */}
            <footer className="p-4 bg-slate-50 border-t border-slate-100 flex justify-end shrink-0">
              <button 
                type="button"
                onClick={() => setActiveDetailsPlan(null)}
                className="px-4 py-2 bg-slate-900 text-white font-bold text-xs rounded-xl hover:bg-slate-800 transition-colors"
              >
                Close Profile Blueprint View
              </button>
            </footer>

          </div>
        </div>
      )}

    </div>
  );
}

// Optional export configuration layout integration with React Router loaders patterns
export async function Loader() {
  const [CyclesList, SubscriptionList, Permissions] = await Promise.all([
    GetPricingCyclesAsync(),
    GetSubscriptionListAsync(),
    TenantPermissionsListAsync()
  ]);
  
  return {
    CyclesList: CyclesList?.data || [],
    SubscriptionList: SubscriptionList?.data || [],
    Permissions: Permissions?.data || []
  };
}