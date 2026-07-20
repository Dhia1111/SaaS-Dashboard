import React, { useState, useEffect, useCallback } from "react";
import { GetSubscriptionStatusAsync } from "../../Apis/Platform";

export default function SubscriptionStatusChecker() {
  // 1. Extract paymentId from the browser's window location queries
  const queryParams = new URLSearchParams(window.location.search);
  const rawPaymentId = queryParams.get("paymentId");
  const paymentId = rawPaymentId ? parseInt(rawPaymentId, 10) : null;

  // State Management Matrix
  const [isLoading, setIsLoading] = useState(true);
  const [isPolling, setIsPolling] = useState(false);
  const [isActive, setIsActive] = useState(false);
  const [statusMessage, setStatusMessage] = useState("");
  const [systemError, setSystemError] = useState("");

  // 2. Centralized Verification Execution Pipeline
  const verifySubscriptionStatus = useCallback(async (isManualClick = false) => {
    if (!paymentId || isNaN(paymentId)) {
      setSystemError("Missing or invalid tracking identifier parameter in URL routing context.");
      setIsLoading(false);
      return;
    }

    if (isManualClick) {
      setIsPolling(true);
    } else {
      setIsLoading(true);
    }
    
    setSystemError("");

    try {
      const response = await GetSubscriptionStatusAsync(paymentId);

      if (response && response.success) {
        // Evaluate the raw database boolean verification flag
        if (response.data === true) {
          setIsActive(true);
          setStatusMessage( "Your infrastructure subscription has been successfully activated.");
        } else {
          setIsActive(false);
          setStatusMessage( "Subscription provisioning is currently processing or pending.");
        }
      } else {
        setIsActive(false);
        setStatusMessage( "Verification gateway rejected the confirmation signature.");
      }
    } catch (err) {
      console.error("Error evaluating gateway payment status:", err);
      setSystemError("A network exception occurred communicating with the verification API nodes.");
    } finally {
      setIsLoading(false);
      setIsPolling(false);
    }
  }, [paymentId]);

  // 3. Trigger validation automatically on initial mount component render
  useEffect(() => {
    verifySubscriptionStatus(false);
  }, [verifySubscriptionStatus]);

  // Handle explicitly broken route setups gracefully
  if (!paymentId || isNaN(paymentId)) {
    return (
      <div className="w-full max-w-md mx-auto bg-white border border-red-100 rounded-2xl p-6 text-center space-y-4 shadow-sm">
        <div className="h-12 w-12 rounded-full bg-red-50 text-red-600 flex items-center justify-center mx-auto">
          <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2.5" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </div>
        <h3 className="text-sm font-black text-red-700 uppercase tracking-wider">Routing Verification Error</h3>
        <p className="text-xs font-semibold text-slate-500 leading-relaxed">
          {systemError || "The active payment gateway parameter signature could not be extracted from your URI query tree."}
        </p>
      </div>
    );
  }

  // Handle main asynchronous loader setup
  if (isLoading) {
    return (
      <div className="w-full max-w-md mx-auto bg-white border border-slate-200/80 rounded-2xl p-8 text-center space-y-4 shadow-xs animate-pulse">
        <div className="mx-auto h-10 w-10 rounded-full bg-slate-50 flex items-center justify-center">
          <svg className="animate-spin h-5 w-5 text-blue-600" fill="none" viewBox="0 0 24 24">
            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
          </svg>
        </div>
        <div className="h-4 bg-slate-200 rounded w-1/2 mx-auto" />
        <div className="h-3 bg-slate-100 rounded w-2/3 mx-auto" />
      </div>
    );
  }

  return (
    <div className="w-full max-w-lg mx-auto bg-white border border-slate-200/80 rounded-2xl shadow-sm overflow-hidden">
      {/* Context Guard Header */}
      <div className="p-4 bg-slate-50/80 border-b border-slate-100 flex justify-between items-center">
        <h3 className="text-[11px] font-black uppercase text-slate-500 tracking-wider">Gateway Provisioning State</h3>
        <span className="text-[10px] font-mono font-bold text-slate-400 bg-white border border-slate-200/60 px-2 py-0.5 rounded-md">
          ID: #{paymentId}
        </span>
      </div>

      <div className="p-8 text-center space-y-8">
        
        {/* =================================================================
            DYNAMIC STATE IDENTIFICATION SYMBOLS & GRAPHICS
           ================================================================= */}
        {isActive ? (
          /* ACTIVE SUCCESS ICON SHIELD */
          <div className="h-16 w-16 rounded-full bg-emerald-50 border border-emerald-200 text-emerald-600 flex items-center justify-center mx-auto shadow-sm scale-100 transition-all duration-300">
            <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="3" d="M5 13l4 4L19 7" />
            </svg>
          </div>
        ) : (
          /* PENDING / PROCESSING ALERT SYMBOL SHIELD */
          <div className="h-16 w-16 rounded-full bg-amber-50 border border-amber-200 text-amber-600 flex items-center justify-center mx-auto shadow-sm animate-pulse">
            <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2.5" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
        )}

        {/* =================================================================
            PROMINENT LARGE-FONT STATE MESSAGING DESK
           ================================================================= */}
        <div className="space-y-3">
          {isActive ? (
            <>
              <h2 className="text-2xl font-black text-emerald-600 tracking-tight uppercase">
                Activation Successful
              </h2>
              <div className="bg-emerald-50/40 border border-emerald-100 rounded-2xl p-4 max-w-md mx-auto">
                <p className="text-sm font-bold text-slate-800 leading-relaxed">
                  {statusMessage}
                </p>
              </div>
            </>
          ) : (
            <>
              <h2 className="text-2xl font-black text-amber-600 tracking-tight uppercase">
                Verification Pending
              </h2>
              <div className="bg-amber-50/40 border border-amber-100 rounded-2xl p-4 max-w-md mx-auto">
                <p className="text-sm font-bold text-slate-800 leading-relaxed">
                  {statusMessage}
                </p>
              </div>
            </>
          )}
        </div>

        {/* Global Pipeline Error Alert Panels */}
        {systemError && (
          <div className="bg-red-50 border border-red-100 rounded-xl p-3 flex gap-2.5 items-center justify-center max-w-md mx-auto">
            <span className="h-1.5 w-1.5 rounded-full bg-red-500 shrink-0" />
            <p className="text-[11px] font-semibold text-red-700">{systemError}</p>
          </div>
        )}

        {/* =================================================================
            CONDITIONAL ACTION CONTROL DECK
           ================================================================= */}
        {isActive ? (
          <div className="space-y-3 pt-2 max-w-md mx-auto">
            <div className="bg-slate-50 border border-slate-100 rounded-xl p-3.5 text-[11px] font-mono text-slate-500 text-left space-y-1">
              <p>• Infrastructure Handshake: <span className="text-emerald-600 font-bold uppercase">Success</span></p>
              <p>• Domain Access Tokens: <span className="text-slate-800 font-bold">Granted & Verified</span></p>
            </div>
            <button
              type="button"
              onClick={() => window.location.replace("/dashboard")}
              className="w-full py-3 bg-slate-900 hover:bg-slate-800 text-white font-bold text-xs rounded-xl shadow-xs transition-colors tracking-wide uppercase"
            >
              Enter Workspace Console
            </button>
          </div>
        ) : (
          <div className="max-w-md mx-auto">
            <button
              type="button"
              disabled={isPolling}
              onClick={() => verifySubscriptionStatus(true)}
              className={`w-full py-3 text-center font-bold text-xs rounded-xl border transition-all flex items-center justify-center gap-2 shadow-xs uppercase tracking-wider ${
                isPolling
                  ? "bg-slate-50 text-slate-400 border-slate-200 cursor-not-allowed"
                  : "bg-blue-600 border-blue-600 text-white hover:bg-blue-700 active:scale-[0.99]"
              }`}
            >
              {isPolling ? (
                <>
                  <svg className="animate-spin h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
                  </svg>
                  Re-Evaluating System Nodes...
                </>
              ) : (
                "Check Deployment Status Again"
              )}
            </button>
          </div>
        )}
      </div>
    </div>
  );
}