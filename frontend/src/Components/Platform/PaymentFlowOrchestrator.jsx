import React, { useState, useEffect } from "react";
import MarketingDiscoveryStep from "./MarketingDiscoveryStep";
import SubscriptionOptionsDisplay from "./Subscription";
import PlanCheckoutWrapper from "./PlanCheckOutWarpper";

// Import step orchestration endpoint vectors
import { CreateNewSubscriptionAsync as SubscribeAsync, GetPaymentProviderList as  GetPaymentProvidersAsync } from "../../Apis/Platform";

export default function RegistrationPaymentFlowMaster() {
  const [currentStep, setCurrentStep] = useState(1);
  
  // Tracking infrastructure selection data across workflows
  const [selectedPlanInfo, setSelectedPlanInfo] = useState(null); 
  const [rawPlanObject, setRawPlanObject] = useState(null); 
  const [stripeProviderId, setStripeProviderId] = useState(null);

  // Operation pipeline responses
  const [subscriptionResult, setSubscriptionResult] = useState(null);
  const [isProcessing, setIsProcessing] = useState(false);
  const [flowError, setFlowError] = useState("");

  // 1. Pre-fetch payment infrastructure provider enums on mount
  useEffect(() => {
    async function resolveProviders() {
      try {
        const res = await GetPaymentProvidersAsync();
        // Expecting a dictionary lookup map structure matching <int, string>
        let providersList = [];
        if (res && res.success && res.data) {
          providersList = res.data;
        } else if (Array.isArray(res)) {
          providersList = res;
        }

        // Locate Stripe enum index automatically case-insensitively
        const stripeMatch = providersList.find(p => p.name?.toLowerCase() === "stripe" || p.value?.toLowerCase() === "stripe");
        if (stripeMatch) {
          setStripeProviderId(stripeMatch.id || stripeMatch.key);
        } else {
          // Fallback to standard industry enum default if table lookups match 0 or 1 indexes
          setStripeProviderId(1); 
        }
      } catch (err) {
        console.error("Error resolving payment provider indices:", err);
        setStripeProviderId(1); // Resilient runtime protection fallback
      }
    }
    resolveProviders();
  }, []);

  // -----------------------------------------------------------------------
  // STEP ROUTING CALLBACK INTERCEPTORS
  // -----------------------------------------------------------------------
  
  // Step 1 Callback -> Transition to Plan Selection
  const handleMarketingComplete = () => {
    setCurrentStep(2);
  };

  // Step 2 Interceptor -> Ingest plan select event arguments
  const handlePlanSelection = (planPayload) => {
    setSelectedPlanInfo(planPayload);
    // Retain full structural dictionary reference for Step 3 billing summary columns
    setRawPlanObject(planPayload._rawPlanSource || planPayload);
    console.log("obj",planPayload)
  };

  // Step 2 Proceed Trigger -> Call SubscribeAsync API endpoint mapping parameters
  const executeSubscriptionPipeline = async () => {
    if (!selectedPlanInfo) {
      setFlowError("Please select an architecture deployment plan to continue.");
      return;
    }

    setFlowError("");
    setIsProcessing(true);

    // Dynamic resolution matching plan options
    const isFreeTrial = !!selectedPlanInfo.userChoseFreetry;
    
    // Safely extract price metrics out of underlying database rows if present
      const priceOptionId = rawPlanObject?.priceOptionId

    // Build standard structure matching your C# Backend endpoint DTO contract
    const subscribeDto = {
      requiresPayment: !isFreeTrial,
      planId: Number(selectedPlanInfo.id),
      priceOptionId: Number(priceOptionId),
      provider: stripeProviderId || 1, // Binds verified Stripe enum key
      providerPaymentId: null // Populated by server gateway orchestrator details
    };

    try {
      const res = await SubscribeAsync(subscribeDto);

      if (res && (res.success || res.status === 200)) {
        const payloadData = res.data || res;
        setSubscriptionResult(payloadData);
        // Step 3 Core Branch: Jump into structural payment checkout or success node layouts
        setCurrentStep(3);
      } else {
        setFlowError(res?.message || "Failed finalizing standard instance token handshakes.");
      }
    } catch (err) {
      console.error("Exception thrown inside active subscription pipeline:", err);
      setFlowError("A critical API database execution timeout exception occurred.");
    } finally {
      setIsProcessing(false);
    }
  };

  return (
    <div className="min-h-screen bg-slate-50/50 py-12 px-4 sm:px-6 lg:px-8 space-y-8">
      
      {/* SEAMLESS STEP STATUS NAVIGATION TRACKER HEADER */}
      <div className="max-w-md mx-auto">
        <div className="flex items-center justify-between relative">
          {[
            { step: 1, label: "Attribution" },
            { step: 2, label: "Plan Setup" },
            { step: 3, label: "Provision" }
          ].map((item, idx, arr) => (
            <React.Fragment key={item.step}>
              <div className="flex flex-col items-center z-10">
                <div className={`h-8 w-8 rounded-full flex items-center justify-center text-xs font-black transition-all ${
                  currentStep >= item.step 
                    ? "bg-blue-600 text-white ring-4 ring-blue-600/10" 
                    : "bg-white text-slate-400 border border-slate-200"
                }`}>
                  {item.step}
                </div>
                <span className={`text-[10px] font-bold uppercase tracking-wider mt-2 ${
                  currentStep === item.step ? "text-blue-600 font-black" : "text-slate-400"
                }`}>
                  {item.label}
                </span>
              </div>
              {idx < arr.length - 1 && (
                <div className={`flex-1 h-0.5 -mt-5 transition-all ${
                  currentStep > item.step ? "bg-blue-600" : "bg-slate-200"
                }`} />
              )}
            </React.Fragment>
          ))}
        </div>
      </div>

      {/* GLOBAL DISMISSABLE ERROR NOTIFICATION MATRIX */}
      {flowError && (
        <div className="max-w-md mx-auto bg-red-50 border border-red-100 rounded-xl p-4 flex gap-3 items-center animate-in fade-in duration-200">
          <span className="h-2 w-2 rounded-full bg-red-500 shrink-0" />
          <p className="text-xs font-semibold text-red-700 leading-relaxed flex-1">{flowError}</p>
        </div>
      )}

      {/* -------------------------------------------------------------------
          DYNAMIC WORKSPACE CORE RENDERING CONTROLLER MAPS
         ------------------------------------------------------------------- */}
      <main className="animate-in fade-in zoom-in-95 duration-300">
        
        {/* STEP 1: MARKETING SOURCE ATTRIBUTION DISCOVERY NODE */}
        {currentStep === 1 && (
          <MarketingDiscoveryStep 
            onNextStep={handleMarketingComplete} 
            buttonText="Next: View Subscription Plans" 
          />
        )}

        {/* STEP 2: FLEXIBLE CLUSTER INFRASTRUCTURE OPTIONS GRID SELECTION */}
        {currentStep === 2 && (
          <div className="space-y-6 flex flex-col items-center">
            {/* Inject native platform rendering element engine cards */}
            <SubscriptionOptionsDisplay 
              onPlanSelect={(payload) => {
                // Decorate object with references to pass deep components safely
                const targetRaw = payload.id ? payload : null;
                handlePlanSelection({ ...payload, _rawPlanSource: targetRaw });
              }} 
            />

            {/* Sticky Execution Navigation Control Deck */}
            <div className="w-full max-w-4xl bg-white border border-slate-200/60 rounded-2xl p-4 flex flex-col sm:flex-row items-center justify-between gap-4 shadow-xs">
              <div className="text-center sm:text-left">
                <p className="text-xs font-bold text-slate-800">
                  Selected Target: <span className="text-blue-600">{selectedPlanInfo?.name || "None Chosen"}</span>
                </p>
                <p className="text-[11px] text-slate-400 font-medium">
                  {selectedPlanInfo?.userChoseFreetry ? "Configured under free trial sandbox rules." : "Requires operational payment authentication."}
                </p>
              </div>

              <button
                type="button"
                disabled={!selectedPlanInfo || isProcessing}
                onClick={executeSubscriptionPipeline}
                className={`w-full sm:w-auto px-6 py-2.5 rounded-xl font-bold text-xs transition-all flex items-center justify-center gap-2 ${
                  !selectedPlanInfo || isProcessing
                    ? "bg-slate-100 text-slate-400 border border-slate-200 cursor-not-allowed"
                    : "bg-blue-600 text-white border border-blue-600 hover:bg-blue-700 active:scale-[0.98] shadow-xs"
                }`}
              >
                {isProcessing ? (
                  <>
                    <svg className="animate-spin h-3.5 w-3.5 text-white" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
                    </svg>
                    Provisioning Node Keys...
                  </>
                ) : (
                  "Proceed to Finalize"
                )}
              </button>
            </div>
          </div>
        )}

        {/* STEP 3: SERVER SIDE EXECUTION CONDITIONAL BOUNDARY CROSSER */}
        {currentStep === 3 && subscriptionResult && (
          <div className="w-full">
            {subscriptionResult.requiresPayment || subscriptionResult.RequiresPayment ? (
              
              /* SUB-ROUTE A: PAID ALLOCATION TIER DETECTED -> INJECT ELEMENT STRIPE SURFACE */
              <PlanCheckoutWrapper 
                plan={rawPlanObject} 
                clientSecret={subscriptionResult.clientSecret || subscriptionResult.ClientSecret} 
              />
            ) : (
              
              /* SUB-ROUTE B: FREE ALLOCATION TIERS -> RENDER IMMEDIATE COMPLETION STATE */
              <div className="w-full max-w-md mx-auto bg-white border border-slate-200/80 rounded-2xl shadow-xs overflow-hidden text-center p-8 space-y-6">
                <div className="w-14 h-14 bg-emerald-50 border border-emerald-100 text-emerald-600 rounded-full flex items-center justify-center mx-auto text-xl font-bold animate-bounce">
                  ✓
                </div>
                
                <div className="space-y-1.5">
                  <h3 className="text-lg font-black text-slate-900 tracking-tight">Sandbox Provisioned Successfully</h3>
                  <p className="text-xs font-semibold text-slate-400 max-w-sm mx-auto leading-relaxed">
                    Your tenant subscription deployment profile has finalized compilation execution parameters natively.
                  </p>
                </div>

                <div className="bg-slate-50 rounded-xl p-4 border border-slate-100 text-left space-y-1 font-mono text-[11px] text-slate-600">
                  <p>Subscription Token ID: <span className="text-slate-900 font-bold">#{subscriptionResult.subscriptionId || subscriptionResult.SubscriptionId}</span></p>
                  <p>Deployment Isolation: <span className="text-emerald-600 font-bold">Active Free Try</span></p>
                </div>

                <button
                  type="button"
                  onClick={() => window.location.replace("/dashboard")}
                  className="w-full py-2.5 bg-slate-900 hover:bg-slate-800 text-white font-bold text-xs rounded-xl shadow-xs transition-colors"
                >
                  Enter Workspace Console
                </button>
              </div>
            )}
          </div>
        )}
      </main>
    </div>
  );
}