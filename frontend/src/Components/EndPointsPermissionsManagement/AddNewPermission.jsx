import React, { useState } from "react";
import { AddTenantPermissionAsync } from "../../Apis/TenantPermissions.js";

export default function AddNewPermission() {
  // Form Submission & Feedback States
  const [isSending, setIsSending] = useState(false);
  const [statusMsg, setStatusMsg] = useState({ type: "", text: "" });
  
  // Interactive helper preview for Bitwise operations
 
  const addNewPermissionAsync = async (e) => {
    e.preventDefault();
    setIsSending(true);
    setStatusMsg({ type: "", text: "" });

    const formData = new FormData(e.target);
    
    // Constructing the payload matching your backend DTO types
    const permissionData = {
        permissionKey: formData.get("permissionKey")?.trim() || null,
       description: formData.get("description")?.trim() || null,
      isActive: formData.get("isActive") === "true", // Safe Boolean extraction
     };

    try {
      const response = await AddTenantPermissionAsync(permissionData);
      
      if (response.success) {
        setStatusMsg({ type: "success", text: response.message || "Permission key established successfully!" });
        e.target.reset();
       } else {
        setStatusMsg({ type: "error", text: response.message || "Failed to register endpoint permission." });
      }
    } catch  {
      setStatusMsg({ type: "error", text: "A critical connection error occurred while syncing." });
    } finally {
      setIsSending(false);
    }
  };

  return (
    <div className="min-h-screen bg-slate-50 p-4 sm:p-8 flex justify-center items-start">
      <div className="w-full max-w-2xl bg-white rounded-2xl border border-slate-200/80 shadow-sm overflow-hidden h-fit animate-in fade-in duration-200">
        
        {/* Header */}
        <header className="p-8 border-b border-slate-100">
          <div className="flex items-center gap-3 mb-2">
            <div className="h-8 w-8 bg-indigo-50 text-indigo-600 rounded-lg flex items-center justify-center">
              <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                <path strokeLinecap="round" strokeLinejoin="round" d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" />
                <path strokeLinecap="round" strokeLinejoin="round" d="M9 8h6M9 12h6M10 16h4" />
                <circle cx="10" cy="8" r="1" fill="currentColor" />
                <circle cx="14" cy="12" r="1" fill="currentColor" />
                <circle cx="11" cy="16" r="1" fill="currentColor" />
              </svg>
            </div>
            <h2 className="text-2xl font-extrabold text-slate-900 tracking-tight">Add New Permission</h2>
          </div>
          <p className="text-sm text-slate-500 ml-11">Establish global bitwise parameters to guard microservice endpoints.</p>
        </header>

        {/* Dynamic Status Notification Alert Panels */}
        {statusMsg.text && (
          <div className={`mx-8 mt-6 p-4 rounded-xl text-sm font-semibold border animate-in fade-in slide-in-from-top-2 ${
            statusMsg.type === "error" 
              ? "bg-red-50 text-red-700 border-red-100" 
              : "bg-green-50 text-green-700 border-green-100"
          }`}>
            <div className="flex items-center gap-2">
              {statusMsg.type === "error" ? (
                <svg className="h-4 w-4 text-red-500 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                </svg>
              ) : (
                <svg className="h-4 w-4 text-green-500 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              )}
              {statusMsg.text}
            </div>
          </div>
        )}

        {/* Input Execution Form Layout */}
        <form className="p-8 space-y-6" onSubmit={addNewPermissionAsync}>
          
          {/* Optional Tenant Matrix Context Variable */}
          <input type="hidden" name="tenantId" value="1" />

          {/* Input: Permission Key identifier string */}
          <div className="space-y-2">
            <label htmlFor="permissionKey" className="text-[11px] font-bold text-slate-500 uppercase tracking-wider ml-1">
              Permission Key Identity
            </label>
            <input
              type="text"
              id="permissionKey"
              name="permissionKey"
              className="w-full px-4 py-3 rounded-xl border border-slate-200 outline-none focus:border-indigo-500 focus:ring-4 focus:ring-indigo-500/10 transition-all placeholder:text-slate-400 text-sm font-semibold text-slate-800"
              placeholder="e.g., AnalysisTools, ReadFromUsers"
              required
            />
          </div>

          

          {/* Input: Narrative/Nullable tracking text block info */}
          <div className="space-y-2">
            <label htmlFor="description" className="text-[11px] font-bold text-slate-500 uppercase tracking-wider ml-1">
              Operational Scope Description
            </label>
            <textarea
              id="description"
              name="description"
              rows={3}
              className="w-full px-4 py-3 rounded-xl border border-slate-200 outline-none focus:border-indigo-500 focus:ring-4 focus:ring-indigo-500/10 transition-all placeholder:text-slate-400 text-sm font-semibold text-slate-700 resize-none"
              placeholder="Detail the target framework microservice pathways protected by this bit mask value..."
            />
          </div>

          {/* Custom Checkbox Input toggler matching your AddUser layout scheme */}
          <div className="pt-2">
            <label className="flex items-center gap-3 p-3 rounded-xl border border-slate-100 bg-slate-50/50 hover:bg-slate-50 transition-colors cursor-pointer group w-fit pr-6">
              <div className="relative flex items-center justify-center">
                <input 
                  type="checkbox" 
                  id="isActiveToggle"
                  className="peer h-5 w-5 appearance-none rounded-md border border-slate-300 bg-white checked:bg-indigo-600 checked:border-indigo-600 transition-all cursor-pointer"
                  defaultChecked={true}
                  onChange={(e) => {
                    document.getElementById("isActiveRealInput").value = e.target.checked ? "true" : "false";
                  }}
                />
                <svg className="absolute h-3 w-3 text-white opacity-0 peer-checked:opacity-100 transition-opacity pointer-events-none" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="4">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" />
                </svg>
              </div>
              <span className="text-sm font-bold text-slate-700 group-hover:text-slate-900 transition-colors">
                Enable Permission Instantly
              </span>
            </label>
            {/* Hidden field capturing explicit boolean value formatting state */}
            <input type="hidden" id="isActiveRealInput" name="isActive" value="true" />
          </div>

          {/* System Interaction Footer Element Panel */}
          <footer className="pt-6 border-t border-slate-100 flex justify-end">
            <button 
              type="submit"
              className="w-full sm:w-auto px-6 py-3 rounded-xl bg-indigo-600 text-white font-bold text-sm shadow-lg shadow-indigo-100 hover:bg-indigo-700 hover:-translate-y-0.5 active:scale-[0.98] transition-all disabled:bg-slate-300 disabled:shadow-none disabled:translate-y-0 min-w-[160px]" 
              disabled={isSending}
            >
              {isSending ? (
                <span className="flex items-center justify-center gap-2">
                  <svg className="animate-spin h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Syncing Mask...
                </span>
              ) : "Register Permission"}
            </button>
          </footer>

        </form>
      </div>
    </div>
  );
}