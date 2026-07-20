import React, { useState, useEffect } from "react";
import {  GetMarkettingPlatformsAsync, SetDescoveryPlatform as ConfirmMarkettingPlatformAsync } from "../../Apis/Platform";
import { jsx } from "react/jsx-runtime";

export default function MarketingDiscoveryStep({ onNextStep, buttonText = "Confirm" }) {
  // Store dictionary data in its native backend layout: Map<int, string> array style
  const [platforms, setPlatforms] = useState([]);
  const [selectedId, setSelectedId] = useState("");
  // Lifecycle & UX states
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  // 1. Fetch data dictionary from backend enum map on mount
  useEffect(() => {
    async function loadPlatforms() {
      try {
        const res = await GetMarkettingPlatformsAsync();
        console.log("res",JSON.stringify(res))
        // Adjust based on your API response wrapper structure (e.g., res.data or direct array)
        if (res && res.success && res.data) {
          // Expecting items structured like: { id: number, name: string } or key-value pairs
          setPlatforms(res.data);
        }  else {
          setErrorMessage("Failed to load attribution platform channels.");
        }
      } catch (err) {
        console.error("Error fetching marketing dictionary metadata:", err);
        setErrorMessage("Could not connect to database configurations.");
      } finally {
        setIsLoading(false);
      }
    }
    loadPlatforms();
  }, []);

  // 2. Dual-action submission handler pipeline
  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!selectedId) {
      setErrorMessage("Please select an option before proceeding.");
      return;
    }

    setErrorMessage("");
    setIsSubmitting(true);

    try {
      // Action 1: Post the explicit database integer key identifier back to the server
      const parsedId = parseInt(selectedId, 10);
      const res = await ConfirmMarkettingPlatformAsync(parsedId);

      if (res && (res.success || res.status === 200)) {
        // Action 2: Safely trigger parent navigation routing flow via code callback
        if (typeof onNextStep === "function") {
          onNextStep();
        }
      } else {
        setErrorMessage(res?.message || "Failed to submit choice validation metrics.");
      }
    } catch (err) {
      console.error("Server synchronization error during step submission:", err);
      setErrorMessage("Network error occurred validating attribution parameters.");
    } finally {
      setIsSubmitting(false);
    }
  };

  if (isLoading) {
    return (
      <div className="w-full max-w-md mx-auto bg-white border border-slate-200/80 rounded-2xl p-6 shadow-xs space-y-3">
        <div className="h-3 bg-slate-200 animate-pulse rounded w-1/3" />
        <div className="h-11 bg-slate-100 animate-pulse rounded-xl border border-slate-200" />
      </div>
    );
  }

  return (
    <div className="w-full max-w-md mx-auto bg-white border border-slate-200/80 rounded-2xl shadow-xs overflow-hidden">
      {/* Container Context Header */}
      <div className="p-4 bg-slate-50/80 border-b border-slate-100">
        <h3 className="text-[11px] font-black uppercase text-slate-500 tracking-wider">Growth Attribution</h3>
      </div>

      <form onSubmit={handleSubmit} className="p-6 space-y-5 text-left">
        <div className="space-y-1">
          <label className="text-[11px] font-bold text-slate-500 uppercase ml-1 block">
            How did you hear about us?
          </label>
          <div className="relative">
            <select
              value={selectedId}
              onChange={(e) => {
                setSelectedId(e.target.value);
                setErrorMessage("");
              }}
              className="w-full px-4 py-3 bg-white text-xs rounded-xl border border-slate-200 outline-none focus:border-blue-600 focus:ring-4 focus:ring-blue-600/10 transition-all appearance-none cursor-pointer text-slate-700 font-medium"
            >
              <option value="" disabled hidden>
                Choose platform routing...
              </option>
              {/* Maps over the <int, string> collection payload cleanly */}
              {platforms&&platforms.length>0 && platforms.map((item) => (
                <option key={item.key} value={item.value} className="text-slate-800">
                  {item.value}
                </option>
              ))}
            </select>

            {/* Custom Chevron vector */}
            <div className="absolute inset-y-0 right-0 flex items-center pr-4 pointer-events-none text-slate-400">
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2.5" d="M19 9l-7 7-7-7" />
              </svg>
            </div>
          </div>
        </div>

        {/* Dynamic Exception Warning Display */}
        {errorMessage && (
          <div className="bg-red-50 border border-red-100 rounded-xl p-3 flex gap-2.5 items-center animate-in fade-in duration-200">
            <span className="h-2 w-2 rounded-full bg-red-500 shrink-0" />
            <p className="text-[11px] font-semibold text-red-700 leading-snug">{errorMessage}</p>
          </div>
        )}

        {/* Execution Trigger button */}
        <button
          type="submit"
          disabled={isSubmitting}
          className={`w-full py-2.5 text-center text-white rounded-xl font-bold text-xs transition-all shadow-xs flex items-center justify-center gap-2 ${
            isSubmitting
              ? "bg-slate-400 border-slate-400 cursor-not-allowed"
              : "bg-blue-600 hover:bg-blue-700 border-blue-600 active:scale-[0.99]"
          }`}
        >
          {isSubmitting ? (
            <>
              <svg className="animate-spin h-3.5 w-3.5 text-white" fill="none" viewBox="0 0 24 24">
                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
              </svg>
              Saving state allocation...
            </>
          ) : (
            buttonText
          )}
        </button>
      </form>
    </div>
  );
}