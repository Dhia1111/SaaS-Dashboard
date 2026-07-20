import React, { useState } from "react";
import { useLoaderData } from "react-router-dom";
import { AddNewTenantSubscriptionAsync } from "../../Apis/Subscriotions.js";
import { TenantPermissionsListAsync } from '../../Apis/TenantPermissions.js';
import { GetPricingCyclesAsync } from '../../Apis/PricingCycles.js';

export default function AddNewPlanWizard() {
  // Navigation Flow State Tracking Variable
  const [currentStep, setCurrentStep] = useState(1);
  const [isSending, setIsSending] = useState(false);
  const [statusMsg, setStatusMsg] = useState({ type: "", text: "" });
  const { permissions, pricingCycles } = useLoaderData();
  
  // Mock array simulating permissions fetched from your database for choice parsing
  const [availablePermissions] = useState(permissions || []);
  const [currentPricingCycle, setCurrentPricingCycle] = useState(pricingCycles?.[0] || null);

  // --- Step Segment Target Form States ---
  
  // Step 1: Core Tenant Plan Metadata Fields
  const [planDetails, setPlanDetails] = useState({
    name: "",
    description: "",
    isActive: true,
    hasFreeTryOption: false // 🆕 Expanded payload variable configuration flag tracker
  });

  // Step 2: Dynamic Copywriting Benefits Array Group
  const [benefitsList, setBenefitsList] = useState([]);
  const [newBenefit, setNewBenefit] = useState({ title: "", description: "" });

  // Step 3: Dynamic Multi-Cycle Price Variant Collection Matrix
  const [pricingOptions, setPricingOptions] = useState([]);
  const [newPrice, setNewPrice] = useState(pricingCycles.length > 0 ? {
    amount: "",
    currency: "USD",
    period: pricingCycles[0].period,
    periodUnit: pricingCycles[0].periodUnit,
    cycleName: pricingCycles[0].cycleName,
    tenantPricingCycleId: pricingCycles[0].id
  } : null);

  // 🆕 DTO Free Trial Property Settings Tracker
  const [freePlanConfig, setFreePlanConfig] = useState({
    cycleId: pricingCycles?.[0]?.id || "",
    startAt: new Date().toISOString().split('T')[0], // Falls back elegantly onto strings
    endAt:  new  Date().toISOString().split('T')[0] // Defaulting window offset cleanly
  });

  // Step 4: Chosen System Permission Nodes Tracker
  const [selectedPermissionIds, setSelectedPermissionIds] = useState([]);

  // --- Helper Action Mutators ---
  const addBenefitRow = () => {
    if (!newBenefit.title.trim()) return;
    setBenefitsList([...benefitsList, { title: newBenefit.title.trim(), description: newBenefit.description.trim() }]);
    setNewBenefit({ title: "", description: "" });
  };

  const removeBenefitRow = (index) => {
    setBenefitsList(benefitsList.filter((_, i) => i !== index));
  };

  const addPricingRow = () => {
    if (pricingCycles.length === 0) {
      setStatusMsg({ type: "error", text: "You must configure at least one billing cycle option matrix before registering." });
      setCurrentStep(3);
      return;
    }
    if (!newPrice.amount || Number(newPrice.amount) <= 0) return;

    const filtered = pricingOptions.filter(x => x.tenantPricingCycleId !== newPrice.tenantPricingCycleId);

    setPricingOptions([
      ...filtered,
      {
        ...newPrice,
        amount: Number(newPrice.amount)
      }
    ]);
  };

  const removePricingRow = (index) => {
    setPricingOptions(pricingOptions.filter((_, i) => i !== index));
  };

  const togglePermissionSelection = (id) => {
    setSelectedPermissionIds(prev => 
      prev.includes(id) ? prev.filter(pId => pId !== id) : [...prev, id]
    );
  };

  const totalCalculatedBitmask = availablePermissions
    .filter(p => selectedPermissionIds.includes(p.id))
    .reduce((sum, p) => sum + p.bitValue, 0);

  // --- Final Aggregate Structural Compilation Execution ---
  const handleFinalFormSubmission = async () => {
    if (benefitsList.length === 0) {
      setStatusMsg({ type: "error", text: "You must add at least one plan benefit highlight before registering." });
      setCurrentStep(2);
      return;
    }
    if (pricingOptions.length === 0) {
      setStatusMsg({ type: "error", text: "You must configure at least one billing cycle option matrix before registering." });
      setCurrentStep(3);
      return;
    }
    if (selectedPermissionIds.length === 0) {
      setStatusMsg({ type: "error", text: "You must select at least one permission scope to define the access bitmask for this plan." });
      setCurrentStep(4);
      return;
    }

    setIsSending(true);
    setStatusMsg({ type: "", text: "" });

    // Compiling parameters into precise multi-tenant DTO signature requirements structure
    const payload = {
      tenantPlan: {
        name: planDetails.name.trim(),
        description: planDetails.description.trim() || null,
        isActive: planDetails.isActive,
        hasFreeTryOption: planDetails.hasFreeTryOption, // ✅ Added structural field integration
        tenant: null 
      },
      // ✅ Injected conditionally matched DtoTenantFreePlan metadata parameters payload context
      tenantFreePlan: planDetails.hasFreeTryOption ? {
        id: 0,
        tenantId: 0,
        tenantPlanId: 0,
        cycleId: Number(freePlanConfig.cycleId),
        startAt: freePlanConfig.startAt,
        endAt: freePlanConfig.endAt
      } : null,
      tenantPlanBenefits: benefitsList.map(benefit => ({
        title: benefit.title,
        description: benefit.description || null,
        tenantPlan: null,
        tenant: null
      })),
      tenantPricingOptions: pricingOptions.map(option => ({
        tenantPricingCycleId: option.tenantPricingCycleId,
        amount: Number(option.amount),
        currency: option.currency,
        isActive: true
      })),
      tenantPlanPermissions: selectedPermissionIds.map(permissionId => ({
        permissionId: permissionId
      }))
    };

      const response = await AddNewTenantSubscriptionAsync(payload);
      if (response.success) {
        setStatusMsg({ type: "success", text: response.message || "Plan suite compiled and active!" });
        setPlanDetails({ name: "", description: "", isActive: true, hasFreeTryOption: false });
        setBenefitsList([]);
        setPricingOptions([]);
        setSelectedPermissionIds([]);
        setCurrentStep(1);
      } else {
        setStatusMsg({ type: "error", text: response.message || "Failed to persist multi-tier plan profile mapping." });
      }
      setStatusMsg({ type: "error", text: "An unhandled server communication failure occurred." });
      setIsSending(false);
    
  };

  return (
    <div className="min-h-screen bg-slate-50 p-4 sm:p-8 flex flex-col items-center">
      <div className="w-full max-w-3xl bg-white rounded-2xl border border-slate-200/80 shadow-sm overflow-hidden">
        
        {/* Top Segment Visual Indicator Track */}
        <header className="bg-blue-600 text-white p-8">
          <div className="flex flex-col sm:flex-row justify-between sm:items-center gap-4">
            <div>
              <h2 className="text-2xl font-extrabold tracking-tight">Create Subscription Architecture</h2>
              <p className="text-white/80 text-xs mt-1">Orchestrate structural features, dynamic limits, and multi-cycle fees.</p>
            </div>
            <div className="text-xs bg-white/10 px-3 py-1.5 rounded-lg border border-white/10 font-bold self-start sm:self-center">
              Step {currentStep} of 4
            </div>
          </div>

          {/* Graphical Step Progress Pipeline bar */}
          <div className="mt-8 flex items-center justify-between relative">
            <div className="absolute left-0 right-0 h-0.5 bg-white/20 top-1/2 -translate-y-1/2 z-0" />
            {[
              { step: 1, label: "Identity" },
              { step: 2, label: "Benefits Copy" },
              { step: 3, label: "Pricing Matrix" },
              { step: 4, label: "Access Bitmask" }
            ].map((item) => (
              <button
                key={item.step}
                type="button"
                onClick={() => currentStep > item.step && setCurrentStep(item.step)}
                className={`relative z-10 flex flex-col items-center gap-2 group outline-none ${currentStep >= item.step ? "cursor-pointer" : "cursor-not-allowed"}`}
              >
                <div className={`h-8 w-8 rounded-full flex items-center justify-center font-bold text-xs transition-all border ${
                  currentStep === item.step 
                    ? "bg-white text-blue-600 border-white ring-4 ring-white/20 shadow-md scale-110" 
                    : currentStep > item.step 
                      ? "bg-emerald-600 text-white border-emerald-500" 
                      : "bg-blue-700 text-white/50 border-white/10"
                }`}>
                  {currentStep > item.step ? "✓" : item.step}
                </div>
                <span className={`text-[11px] font-bold tracking-wider uppercase transition-colors ${currentStep >= item.step ? "text-white" : "text-white/40"}`}>
                  {item.label}
                </span>
              </button>
            ))}
          </div>
        </header>

        {/* Dynamic Warning Notification Elements */}
        {statusMsg.text && (
          <div className={`mx-8 mt-6 p-4 rounded-xl text-sm font-semibold border animate-in fade-in ${
            statusMsg.type === "error" ? "bg-red-50 text-red-700 border-red-100" : "bg-green-50 text-green-700 border-green-100"
          }`}>
            <div className="flex items-center gap-2">
              <span className={`w-2 h-2 rounded-full ${statusMsg.type === 'error' ? 'bg-red-500' : 'bg-green-500'}`} />
              {statusMsg.text}
            </div>
          </div>
        )}

        {/* --- STEP 1 CONTENT: Core Plan Parameters --- */}
        {currentStep === 1 && (
          <div className="p-8 space-y-6 animate-in fade-in duration-200">
            <div className="border-b border-slate-100 pb-2">
              <h3 className="text-lg font-bold text-slate-800">Plan Identity Specifications</h3>
              <p className="text-xs text-slate-400 font-medium">Define structural identification descriptors for tier packaging classification.</p>
            </div>
            
            <div className="space-y-2">
              <label className="text-[11px] font-bold text-slate-500 uppercase tracking-wider ml-1">Plan Display Name</label>
              <input
                type="text"
                value={planDetails.name}
                onChange={(e) => setPlanDetails({ ...planDetails, name: e.target.value })}
                className="w-full px-4 py-3 rounded-xl border border-slate-200 outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-500/10 text-sm font-semibold text-slate-800"
                placeholder="e.g., Enterprise Premium Scale Package"
                required
              />
            </div>

            <div className="space-y-2">
              <label className="text-[11px] font-bold text-slate-500 uppercase tracking-wider ml-1">Marketing Description</label>
              <textarea
                value={planDetails.description}
                onChange={(e) => setPlanDetails({ ...planDetails, description: e.target.value })}
                rows={4}
                className="w-full px-4 py-3 rounded-xl border border-slate-200 outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-500/10 text-sm font-semibold text-slate-700 resize-none"
                placeholder="Summarize structural limitations or user tier quotas governed under this profile tier context..."
              />
            </div>

            <div>
              <label className="flex items-center gap-3 p-3 rounded-xl border border-slate-100 bg-slate-50/50 hover:bg-slate-50 transition-colors cursor-pointer group w-fit pr-6">
                <input 
                  type="checkbox" 
                  checked={planDetails.isActive}
                  onChange={(e) => setPlanDetails({ ...planDetails, isActive: e.target.checked })}
                  className="h-5 w-5 rounded-md border-slate-300 text-blue-600 focus:ring-blue-500 cursor-pointer"
                />
                <span className="text-sm font-bold text-slate-700 group-hover:text-slate-900 transition-colors">Activate Plan Tier Immediately</span>
              </label>
            </div>
          </div>
        )}

        {/* --- STEP 2 CONTENT: Plan Marketing Highlight Benefits --- */}
        {currentStep === 2 && (
          <div className="p-8 space-y-6 animate-in fade-in duration-200">
            <div className="border-b border-slate-100 pb-2">
              <h3 className="text-lg font-bold text-slate-800">Plan Marketing Highlight Benefits</h3>
              <p className="text-xs text-slate-400 font-medium">Add the feature highlights displayed to potential subscribers on pricing sheets.</p>
            </div>

            <div className="bg-slate-50 p-4 rounded-xl border border-slate-100 space-y-3">
              <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
                <div className="md:col-span-1">
                  <input
                    type="text"
                    placeholder="Benefit Title (e.g., Advanced AI Analytics)"
                    value={newBenefit.title}
                    onChange={(e) => setNewBenefit({ ...newBenefit, title: e.target.value })}
                    className="w-full px-3 py-2.5 rounded-lg border border-slate-200 text-xs font-bold outline-none bg-white focus:border-blue-500"
                  />
                </div>
                <div className="md:col-span-2">
                  <input
                    type="text"
                    placeholder="Description text (e.g., Processing up to 15 million metrics)"
                    value={newBenefit.description}
                    onChange={(e) => setNewBenefit({ ...newBenefit, description: e.target.value })}
                    className="w-full px-3 py-2.5 rounded-lg border border-slate-200 text-xs font-semibold outline-none bg-white focus:border-blue-500"
                  />
                </div>
              </div>
              <button
                type="button"
                onClick={addBenefitRow}
                className="px-3 py-1.5 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-xs font-bold transition-all flex items-center gap-1.5 shadow-sm"
              >
                <span>+</span> Append Benefit Node
              </button>
            </div>

            <div className="space-y-2">
              <label className="text-[11px] font-bold text-slate-400 uppercase tracking-wider block ml-1">Added Plan Benefits Queue ({benefitsList.length})</label>
              {benefitsList.length === 0 ? (
                <div className="text-center p-8 border border-dashed border-slate-200 rounded-xl text-xs font-medium text-slate-400">
                  No copy bullets appended yet. Provide elements using the inputs above.
                </div>
              ) : (
                <div className="space-y-2 max-h-[220px] overflow-y-auto pr-2">
                  {benefitsList.map((benefit, index) => (
                    <div key={index} className="flex justify-between items-center bg-white border border-slate-200 p-3.5 rounded-xl text-xs font-semibold text-slate-800 group hover:border-slate-300 transition-all">
                      <div>
                        <h4 className="font-bold text-slate-900 flex items-center gap-1.5">
                          <span className="h-1.5 w-1.5 bg-blue-600 rounded-full" /> {benefit.title}
                        </h4>
                        {benefit.description && <p className="text-slate-500 mt-0.5 ml-3 font-normal">{benefit.description}</p>}
                      </div>
                      <button 
                        type="button" 
                        onClick={() => removeBenefitRow(index)}
                        className="text-red-500 font-bold hover:bg-red-50 px-2 py-1 rounded-md transition-colors"
                      >
                        Remove
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        )}

        {/* --- STEP 3 CONTENT: Pricing, Cycles & Try Configurations --- */}
        {currentStep === 3 && (
          <div className="p-8 space-y-6 animate-in fade-in duration-200">
            <div className="border-b border-slate-100 pb-2">
              <h3 className="text-lg font-bold text-slate-800">Pricing and Cycles Configuration</h3>
              <p className="text-xs text-slate-400 font-medium">Map structural rate pricing parameters across flexible transaction intervals.</p>
            </div>

            {/* 🆕 HasFreeTryOption Checkbox Panel Component */}
            <div className="bg-blue-50/40 p-4 rounded-xl border border-blue-100 space-y-4">
              <label className="flex items-center gap-3 cursor-pointer group w-fit">
                <input 
                  type="checkbox" 
                  checked={planDetails.hasFreeTryOption}
                  onChange={(e) => setPlanDetails({ ...planDetails, hasFreeTryOption: e.target.checked })}
                  className="h-5 w-5 rounded-md border-slate-300 text-blue-600 focus:ring-blue-500 cursor-pointer"
                />
                <span className="text-sm font-bold text-slate-700 group-hover:text-slate-900 transition-colors">
                  Include Free Trial Access Window
                </span>
              </label>

              {/* 🆕 Conditional Select UI Segment Block for Trial Parameters Mapping */}
              {planDetails.hasFreeTryOption && (
                <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 pt-2 animate-in slide-in-from-top-2 duration-200">
                  <div className="space-y-1">
                    <label className="text-[10px] font-bold text-slate-500 uppercase">Trial Pricing Cycle</label>
                    <select
                      value={freePlanConfig.cycleId}
                      onChange={(e) => setFreePlanConfig({ ...freePlanConfig, cycleId: e.target.value })}
                      className="w-full p-2.5 rounded-lg border border-slate-200 bg-white text-xs font-bold outline-none focus:border-blue-500"
                    >
                      {pricingCycles.map(cycle => (
                        <option key={cycle.id} value={cycle.id}>{cycle.cycleName}</option>
                      ))}
                    </select>
                  </div>
                 
                 
                </div>
              )}
            </div>

            <div className="bg-slate-50 p-4 rounded-xl border border-slate-100 grid grid-cols-1 sm:grid-cols-3 gap-3 items-end">
              <div className="space-y-1">
                <label className="text-[10px] font-bold text-slate-500 uppercase">Billing Interval Scope</label>
                <select
                  value={currentPricingCycle?.id || ''}
                  onChange={(e) => {
                    const selected = pricingCycles.find(cycle => cycle.id === Number(e.target.value)) || null;
                    setCurrentPricingCycle(selected);
                    if (selected) {
                      setNewPrice({ 
                        ...newPrice, 
                        cycleName: selected.cycleName, 
                        period: selected.period, 
                        periodUnit: selected.periodUnit,
                        tenantPricingCycleId: selected.id
                      });
                    }
                  }}
                  className="w-full p-2.5 rounded-lg border border-slate-200 bg-white text-xs font-bold outline-none focus:border-blue-500"
                >
                  {pricingCycles.length > 0 ? (
                    pricingCycles.map(cycle => (
                      <option key={cycle.id} value={cycle.id}>{cycle.cycleName}</option>
                    ))
                  ) : (
                    <option value="">No Billing Cycles Available</option>
                  )}
                </select>
              </div>

              <div className="space-y-1">
                <label className="text-[10px] font-bold text-slate-500 uppercase">Rate Base Charge Price</label>
                <div className="relative">
                  <span className="absolute left-3 top-1/2 -translate-y-1/2 text-xs font-bold text-slate-400">$</span>
                  <input
                    type="number"
                    min="0"
                    placeholder="0.00"
                    value={newPrice?.amount || ''}
                    onChange={(e) => setNewPrice({ ...newPrice, amount: e.target.value })}
                    className="w-full pl-6 pr-3 py-2.5 rounded-lg border border-slate-200 text-xs font-bold outline-none bg-white focus:border-blue-500"
                  />
                </div>
              </div>

              <button
                type="button"
                onClick={addPricingRow}
                className="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold text-xs py-2.5 rounded-lg transition-all h-[38px] flex items-center justify-center gap-1 shadow-sm"
              >
                <span>+</span> Append Rate Scope
              </button>
            </div>

            <div className="space-y-2">
              <label className="text-[11px] font-bold text-slate-400 uppercase tracking-wider block ml-1">Current Active Tiers Pricing Array</label>
              {pricingOptions.length === 0 ? (
                <div className="text-center p-8 border border-dashed border-slate-200 rounded-xl text-xs font-medium text-slate-400">
                  No pricing matrix variables configured yet. Append rates above.
                </div>
              ) : (
                <div className="border border-slate-200 rounded-xl overflow-hidden text-xs">
                  <table className="w-full border-collapse bg-white">
                    <thead className="bg-slate-50 text-slate-500 font-bold border-b border-slate-200 text-left">
                      <tr>
                        <th className="p-3">Interval Cycle Type</th>
                        <th className="p-3">Duration</th>
                        <th className="p-3 text-right">Target Rate Amount</th>
                        <th className="p-3 text-center">Actions</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100 font-semibold text-slate-700">
                      {pricingOptions.map((opt, idx) => (
                        <tr key={idx} className="hover:bg-slate-50/60 transition-colors">
                          <td className="p-3 font-bold text-slate-900">{opt.cycleName} Option</td>
                          <td className="p-3 font-mono text-slate-500">{opt.period} {opt.periodUnit}</td>
                          <td className="p-3 text-right font-bold text-blue-600">{Number(opt.amount).toFixed(2)} {opt.currency}</td>
                          <td className="p-3 text-center">
                            <button 
                              type="button" 
                              onClick={() => removePricingRow(idx)}
                              className="text-red-500 font-bold hover:underline"
                            >
                              Drop
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          </div>
        )}

        {/* --- STEP 4 CONTENT: Permission Access Scoping --- */}
        {currentStep === 4 && (
          <div className="p-8 space-y-6 animate-in fade-in duration-200">
            <div className="border-b border-slate-100 pb-2 flex flex-col sm:flex-row justify-between sm:items-end gap-2">
              <div>
                <h3 className="text-lg font-bold text-slate-800">Bitwise Permission Access Scoping</h3>
                <p className="text-xs text-slate-400 font-medium">Select endpoint security variables enabled out-of-the-box for subscribers on this plan level.</p>
              </div>
              <div className="bg-blue-50 border border-blue-100 px-3 py-1.5 rounded-lg text-right">
                <span className="text-[10px] font-bold text-blue-600 block uppercase tracking-wide">Compiled Mask Flag Integer</span>
                <span className="font-mono text-xs font-extrabold text-blue-600">Sum Score: {totalCalculatedBitmask}</span>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-3 max-h-[300px] overflow-y-auto pr-1">
              {availablePermissions.map((permission) => {
                const isChecked = selectedPermissionIds.includes(permission.id);
                return (
                  <div
                    key={permission.id}
                    onClick={() => togglePermissionSelection(permission.id)}
                    className={`p-4 rounded-xl border transition-all cursor-pointer select-none flex flex-col justify-between h-full group ${
                      isChecked 
                        ? "bg-blue-50/50 border-blue-500/30 shadow-sm ring-1 ring-blue-500/20" 
                        : "bg-white border-slate-200 hover:border-slate-300"
                    }`}
                  >
                    <div>
                      <div className="flex items-start justify-between gap-2">
                        <span className="font-bold text-sm text-slate-900 group-hover:text-blue-600 transition-colors">
                          {permission.permissionKey}
                        </span>
                        <span className="font-mono text-[10px] font-bold bg-slate-100 text-slate-600 px-1.5 py-0.5 rounded shrink-0">
                          Bit: {permission.bitValue}
                        </span>
                      </div>
                      <p className="text-slate-500 text-xs mt-1.5 font-medium leading-relaxed">
                        {permission.description}
                      </p>
                    </div>

                    <div className="mt-4 pt-3 border-t border-slate-100/80 flex items-center gap-2">
                      <div className={`h-4 w-4 rounded border flex items-center justify-center transition-all ${
                        isChecked ? "bg-blue-600 border-blue-600 text-white" : "border-slate-300 bg-white"
                      }`}>
                        {isChecked && <span className="text-[10px]">✓</span>}
                      </div>
                      <span className="text-[11px] font-bold text-slate-400 group-hover:text-slate-600 transition-colors">
                        {isChecked ? "Scope Selected" : "Click to authorize"}
                      </span>
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        )}

        {/* --- SYSTEM WORKSPACE INTERACTION COMMAND FOOTER PANEL --- */}
        <footer className="p-6 bg-slate-50 border-t border-slate-200/60 flex justify-between items-center">
          <button
            type="button"
            disabled={currentStep === 1 || isSending}
            onClick={() => setCurrentStep(prev => prev - 1)}
            className="px-4 py-2.5 rounded-xl border border-slate-200 text-slate-700 bg-white text-xs font-bold hover:bg-slate-100 transition-all disabled:opacity-0 pointer-events-auto"
          >
            ← Move Back
          </button>

          {currentStep < 4 ? (
            <button
              type="button"
              disabled={currentStep === 1 && !planDetails.name.trim()}
              onClick={() => {
                setStatusMsg({ type: "", text: "" });
                setCurrentStep(prev => prev + 1);
              }}
              className="px-5 py-2.5 rounded-xl bg-blue-600 text-white text-xs font-bold hover:bg-blue-700 transition-all shadow-sm disabled:bg-slate-200 disabled:text-slate-400"
            >
              Continue Forward →
            </button>
          ) : (
            <button
              type="button"
              disabled={isSending}
              onClick={handleFinalFormSubmission}
              className="px-6 py-2.5 rounded-xl bg-blue-600 text-white text-xs font-bold hover:bg-blue-700 transition-all shadow-md disabled:bg-slate-300 min-w-[140px]"
            >
              {isSending ? "Publishing Layer..." : "Save Plan Suite"}
            </button>
          )}
        </footer>

      </div>
    </div>
  );
}

export async function Loader() {
  const [permissions, pricingCycles] = await Promise.all([
    TenantPermissionsListAsync(),
    GetPricingCyclesAsync()
  ]);
  return { permissions: permissions.data || [], pricingCycles: pricingCycles.data || [] };
}