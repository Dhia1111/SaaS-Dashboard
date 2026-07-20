import React, { useState, useEffect } from "react";
import { PlatformSubscriptionsAsync as PlatformSubscriptionAsync, GetPricingCyclesAsync } from "../../Apis/Platform.js";
// Assuming TenantHaveUsedHisFreePlanAsync is imported from your auth/tenant APIs
import { TenantHaveUsedHisFreePlanAsync } from "../../Apis/Platform.js"; 

export default function SubscriptionOptionsDisplay({ 
  onPlanSelect, 
  hideFreePlan = false, 
  hidePaidPlan = false 
}) {
  const [plans, setPlans] = useState([]);
  const [pricingCycles, setPricingCycles] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMsg, setErrorMsg] = useState("");
  const [hasUsedFreePlan, setHasUsedFreePlan] = useState(false);
  
  // Tracks selection states
  const [activeCycleId, setActiveCycleId] = useState(null);
  const [selectedPlanId, setSelectedPlanId] = useState(null);
  const [isFreeTrialChecked, setIsFreeTrialChecked] = useState({});

  // -----------------------------------------------------------------------
  // UNIFIED DATA INGESTION LIFECYCLE WITH TENANT CHECK
  // -----------------------------------------------------------------------
  useEffect(() => {
    async function loadSubscriptionWorkspace() {
      try {
        // 1. Fetch data requirements in parallel
        const [subscriptionRes, cyclesRes] = await Promise.all([
          PlatformSubscriptionAsync(),
          GetPricingCyclesAsync()
        ]);

        // 2. Safely parse localStorage and fetch free tier history usage if tenant user exists
        try {
          const storedUser = localStorage.getItem("userInfo");
          if (storedUser) {
            const parsedUser = JSON.parse(storedUser);
            if (parsedUser?.tenantId) {
              const usedFreeRes = await TenantHaveUsedHisFreePlanAsync(parsedUser.tenantId);
         
              // Handle both raw boolean responses or object payloads like { success: true, data: true }
         if(usedFreeRes.success){
                         setHasUsedFreePlan(usedFreeRes.data);
      
         }
         else{
                                   setHasUsedFreePlan(true);

         }
            }
          }
        } catch (storageErr) {
          console.error("Failed processing tenant history context boundary tokens:", storageErr);
        }

        const standardSubscriptionValid = subscriptionRes && subscriptionRes.success && Array.isArray(subscriptionRes.data);
        const standardCyclesValid = cyclesRes && cyclesRes.success && Array.isArray(cyclesRes.data);

        if (standardSubscriptionValid && standardCyclesValid) {
          setPlans(subscriptionRes.data);
          setPricingCycles(cyclesRes.data);

          if (cyclesRes.data.length > 0) {
            setActiveCycleId(Number(cyclesRes.data[0].id));
          }
        } else {
          setErrorMsg("Data streaming payload structural initialization mismatch error.");
        }
      } catch (err) {
        setErrorMsg("Failed to synchronize component application runtime nodes.");
        console.error(err);
      } finally {
        setIsLoading(false);
      }
    }

    loadSubscriptionWorkspace();
  }, []);

  // -----------------------------------------------------------------------
  // SELECTION HANDLER MUTATOR
  // -----------------------------------------------------------------------
  const handleSelectionChange = (plan,ActiveRate ,forcedTrialState = null) => {
    const targetPlanId = plan.id;
    setSelectedPlanId(targetPlanId);
    
    // Fallback logic to protect payload if tenant has used free options
    const canUseTrial = plan.hasFreeTryOption && !hasUsedFreePlan;
    const choseTrial = canUseTrial && (forcedTrialState !== null 
      ? forcedTrialState 
      : !!isFreeTrialChecked[targetPlanId]);

    if (typeof onPlanSelect === "function") {
  
      onPlanSelect({
        id: plan.id,
        name: plan.name || "Unnamed Plan",
        description: plan.description || "",
        hasfreeplan: canUseTrial,
        userChoseFreetry: choseTrial,
        priceOptionId:ActiveRate?.id,
      });
    }
  };

  const handleTrialCheckboxToggle = (e,ActiveRate, plan) => {
    e.stopPropagation(); // Avoid triggering full card selection changes natively 
    const checkedStatus = e.target.checked;
    setIsFreeTrialChecked(prev => ({
      ...prev,
      [plan.id]: checkedStatus
    }));

    handleSelectionChange(plan,ActiveRate, checkedStatus);
  };

  // -----------------------------------------------------------------------
  // DTO RUNTIME DICTIONARY PARSERS
  // -----------------------------------------------------------------------
  const activeCycleTabs = pricingCycles.filter(cycle => 
    plans.some(plan => 
      Array.isArray(plan?.planPricingOptions) && 
      plan.planPricingOptions.some(opt => Number(opt.tenantPricingCycleId) === Number(cycle.id))
    )
  );

  if (isLoading) {
    return (
      <div className="min-h-screen bg-slate-50 flex flex-col items-center justify-center p-8">
        <div className="flex items-center gap-3 bg-white px-6 py-4 rounded-2xl border border-slate-200/80 shadow-xs font-bold text-xs text-slate-600">
          <svg className="animate-spin h-4 w-4 text-blue-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          Synchronizing Pricing Matrix...
        </div>
      </div>
    );
  }

  if (errorMsg || plans.length === 0) {
    return (
      <div className="min-h-screen bg-slate-50 flex items-center justify-center p-8">
        <div className="w-full max-w-md bg-white p-6 rounded-2xl border border-red-100 shadow-sm text-center space-y-3">
          <span className="h-2 w-2 rounded-full bg-red-500 inline-block" />
          <h4 className="text-xs font-black text-red-700 uppercase tracking-wider">Pipeline Loading Alert</h4>
          <p className="text-xs font-semibold text-slate-500 leading-relaxed">{errorMsg || "No active metadata found."}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-50 p-4 sm:p-8 flex flex-col items-center justify-start space-y-8">
      
      <header className="text-center max-w-xl space-y-2">
        <h2 className="text-2xl font-black text-slate-900 tracking-tight">Flexible Application Tiers</h2>
        <p className="text-xs text-slate-400 font-semibold leading-relaxed">
          Select the subscription architecture layout that fits your workflow operational requirements.
        </p>
      </header>

      {activeCycleTabs.length > 0 && !hidePaidPlan && (
        <div className="bg-white border border-slate-200/80 p-1.5 rounded-xl shadow-xs flex gap-1">
          {activeCycleTabs.map((cycle) => (
            <button
              key={cycle.id}
              type="button"
              onClick={() => setActiveCycleId(Number(cycle.id))}
              className={`px-4 py-2 text-[11px] font-bold uppercase tracking-wider rounded-lg transition-all ${
                activeCycleId === Number(cycle.id)
                  ? "bg-blue-600 text-white shadow-sm font-extrabold"
                  : "text-slate-500 hover:text-slate-800 hover:bg-slate-50"
              }`}
            >
              {cycle.cycleName} ({cycle.period} {cycle.periodUnit})
            </button>
          ))}
        </div>
      )}

      <div className="w-full max-w-4xl grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-12 items-start justify-center">
        {plans.map((suite) => {
          const planId = suite?.id;
          const planName = suite?.name || "Unnamed Plan";
          const planDescription = suite?.description || "No supplemental details provided.";
          const isActive = suite?.isActive;
          
          // Determine active features depending on whether user has checked out trial nodes before
          const hasFreeTryOption = suite?.hasFreeTryOption && !hasUsedFreePlan;
          
          const tenantPlanBenefits = Array.isArray(suite?.benefits) ? suite.benefits : [];
          const tenantPricingOptions = Array.isArray(suite?.planPricingOptions) ? suite.planPricingOptions : [];

          if (!isActive) return null;

          const activeRate = tenantPricingOptions.find(opt => Number(opt.tenantPricingCycleId) === activeCycleId) 
                             || tenantPricingOptions[0];

                
          // Conditional evaluation logic using the newly passed flag props
          const isPlanFree =  Number(activeRate?.amount) === 0;

          if (hideFreePlan && isPlanFree) return null;
          if (hidePaidPlan && !isPlanFree) return null;

          const matchedCycleMeta = pricingCycles.find(c => Number(c.id) === Number(activeRate?.tenantPricingCycleId));
          const unitLabel = matchedCycleMeta ? `${matchedCycleMeta.period} ${matchedCycleMeta.periodUnit}` : "cycle";
          
          const isCurrentlySelected = selectedPlanId === planId;
          const trialSelected = !!isFreeTrialChecked[planId];
          console.log("activeRate",JSON.stringify(activeRate)) 
          return (
            <div 
              key={planId} 
              onClick={() => handleSelectionChange(suite,activeRate,null)}
              className={`bg-white rounded-2xl border shadow-xs hover:shadow-md transition-all flex flex-col overflow-hidden  h-full group cursor-pointer min-w-[300px] ${
                isCurrentlySelected 
                  ? "border-blue-600 ring-2 ring-blue-600/20 bg-blue-50/5" 
                  : "border-slate-200/80 hover:border-slate-300 "
              }`}
            >
              <div className="p-6 h-fit  border-b border-slate-100 space-y-2 ">
                <div className="flex justify-between items-start gap-2">
                  <h3 className="text-sm font-black text-slate-800 tracking-tight group-hover:text-blue-600 transition-colors">
                    {planName}
                  </h3>
                  {isCurrentlySelected && (
                    <span className="bg-blue-600 text-white text-[9px] font-black uppercase tracking-wider px-2 py-0.5 rounded-md shrink-0">
                      Active Target
                    </span>
                  )}
                </div>
                <p className="text-xs text-slate-400 font-semibold leading-relaxed">
                  {planDescription}
                </p>
              </div>

              {/* Displays if hasFreeTryOption is set to true and has not been used by tenant yet */}
              {hasFreeTryOption&& !hideFreePlan && (
                <div className="px-6 py-3 bg-blue-50/40 border-b border-slate-100 flex items-center justify-between">
                  <label 
                    className="flex items-center gap-2.5 cursor-pointer w-full"
                    onClick={(e) => e.stopPropagation()} 
                  >
                    <input 
                      type="checkbox"
                      checked={trialSelected}
                      onChange={(e) => handleTrialCheckboxToggle(e,activeRate ,suite)}
                      className="h-4 w-4 text-blue-600 border-slate-300 rounded focus:ring-blue-500 cursor-pointer"
                    />
                    <div>
                      <span className="text-[11px] font-extrabold text-blue-700 uppercase tracking-wider block">
                        Activate Free Trial Period
                      </span>
                      <span className="text-[10px] text-slate-400 font-medium block">
                        Test system infrastructure rules before paying.
                      </span>
                    </div>
                  </label>
                </div>
              )}

              <div className="px-6 py-4 bg-slate-50/50 border-b border-slate-100 flex items-baseline gap-1">
                <span className={`text-2xl font-black font-mono tracking-tight transition-colors ${trialSelected ? 'text-slate-400 line-through text-xl' : 'text-slate-900'}`}>
                  {activeRate ? activeRate.amount : "0"}
                </span>
                {trialSelected && (
                  <span className="text-xl font-black text-emerald-600 font-mono tracking-tight mr-1">
                    0.00
                  </span>
                )}
                <span className="text-xs font-bold text-slate-400 uppercase font-mono">
                  {activeRate ? activeRate.currency : "USD"}
                </span>
                <span className="text-[10px] text-slate-400 font-semibold ml-1">
                  / {unitLabel}
                </span>
              </div>

              <div className="p-6 space-y-4">
                <h4 className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">Allocated Capabilities Matrix</h4>
                <ul className="space-y-3">
                  {tenantPlanBenefits.map((benefit) => (
                    <li key={benefit.id} className="flex gap-3 items-start text-xs">
                      <div className="h-4 w-4 rounded-md bg-emerald-50 border border-emerald-100 text-emerald-600 flex items-center justify-center shrink-0 mt-0.5">
                        <svg className="h-2.5 w-2.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="4">
                          <path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" />
                        </svg>
                      </div>
                      <div className="space-y-0.5">
                        <span className="block font-bold text-slate-700 leading-snug">{benefit.title}</span>
                        {benefit.description && (
                          <span className="block text-[11px] text-slate-400 font-medium leading-normal">{benefit.description}</span>
                        )}
                      </div>
                    </li>
                  ))}
                </ul>
              </div>

              <div className="p-6 mt-auto border-t border-slate-50">
                <button
                  type="button"
                  className={`w-full py-2.5 text-center rounded-xl font-bold text-xs transition-all border ${
                    isCurrentlySelected 
                      ? "bg-blue-600 text-white border-blue-600 shadow-sm" 
                      : "border-slate-200 hover:border-blue-600 hover:bg-blue-600 hover:text-white"
                  }`}
                >
                  {isCurrentlySelected ? "Tier Selected" : "Configure Allocation Tier"}
                </button>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}