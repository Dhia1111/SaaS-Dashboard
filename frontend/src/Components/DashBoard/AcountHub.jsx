import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { TenantHaveUsedHisFreePlanAsync, GetTenantActiveSubscriptionAsync } from "../../Apis/Platform";
import { GetUserInfoAsnc } from "../../Apis/Users";
import { GetTenatInfoAsync } from "../../Apis/Tenant";
import { refreshToken } from '../../Apis/GenralAuth';

export default function AccountProfileHub() {
  const navigate = useNavigate();
  
  // 1. Ingest clean, decentralized claims from the bifurcation token storage engines
  const [userInfoClaims] = useState(() => {
    try {
      const stored = localStorage.getItem("userInfo");
            console.log("user",JSON.parse(stored))
      return stored ? JSON.parse(stored) : null;
    } catch { return null; }
  });

  const [tenantInfoClaims] = useState(() => {
    try {
      const stored = localStorage.getItem("tenantInfo");
            console.log("tenantInfo",JSON.parse(stored))

      return stored ? JSON.parse(stored) : null;
    } catch { return null; }
  });

  // 2. Component Domain Domain States
  const [profileData, setProfileData] = useState({
    user: null,
    tenant: null,
    subscription: null,
    hasUsedFreePlan: false
  });
  const [isLoading, setIsLoading] = useState(true);
  const [errorMsg, setErrorMsg] = useState("");

  // 3. Parallel Ingestion Pipeline Matrix Lifecycle
  useEffect(() => {
    async function hydrateWorkspaceContext() {
      await refreshToken();
      // Direct security boundary protection: fall back if no token claims are resident
      const currentTenantId = tenantInfoClaims?.tenantId || userInfoClaims?.tenantId;
      const currentUserId = userInfoClaims?.userId;

      if (!currentTenantId) {
        setIsLoading(false);
        return;
      }

      try {
        // Run lookups synchronously in parallel threads across domain data tables
        const [freePlanRes, activeSubRes, tenantRes, userRes] = await Promise.all([
          TenantHaveUsedHisFreePlanAsync(currentTenantId),
          GetTenantActiveSubscriptionAsync(currentTenantId).catch(() => null),
          GetTenatInfoAsync(currentTenantId).catch(() => null),
          currentUserId ? GetUserInfoAsnc(currentUserId).catch(() => null) : null
        ]);

        // Evaluate boolean status safely flag variants from API standard rules 
        const parsedHasUsedFree = typeof freePlanRes === "boolean" 
          ? freePlanRes 
          : !!(freePlanRes?.success && (freePlanRes.data === true || freePlanRes.data?.haveUsedTheFreeTry));

        setProfileData({
          user: userRes?.success ? userRes.data : null,
          tenant: tenantRes?.success ? tenantRes.data : null,
          subscription: activeSubRes?.success && activeSubRes.data?.isActive ? activeSubRes.data : null,
          hasUsedFreePlan: parsedHasUsedFree
        });

      } catch (err) {
        console.error("Critical exceptions matching runtime database constraints:", err);
        setErrorMsg("Failed to synchronize component application runtime layout matrices.");
      } finally {
        setIsLoading(false);
      }
    }

    hydrateWorkspaceContext();
  }, [userInfoClaims, tenantInfoClaims]);

  // UI Interactive Context Routes Mutation
  const handleAvatarClick = () => {
    if (!userInfoClaims && !tenantInfoClaims) {
      navigate('/login');
    } else {
      console.log("Account Profile claims are active for identity ID:", userInfoClaims?.userId);
    }
  };

  if (isLoading) {
    return (
      <div className="max-w-4xl mx-auto p-6 space-y-6">
        <div className="h-32 bg-white border border-slate-200/60 rounded-2xl animate-pulse flex items-center p-6 gap-4">
          <div className="w-16 h-16 rounded-full bg-slate-100" />
          <div className="space-y-2 flex-1">
            <div className="h-4 bg-slate-100 rounded w-1/4" />
            <div className="h-3 bg-slate-100 rounded w-1/2" />
          </div>
        </div>
      </div>
    );
  }

  // Handle fully anonymous unauthenticated view boundaries safely
  if (!tenantInfoClaims && !userInfoClaims) {
    return (
      <div className="max-w-md mx-auto p-8 text-center bg-white border rounded-2xl my-12 space-y-4">
        <div className="w-12 h-12 bg-amber-50 text-amber-500 rounded-full flex items-center justify-center mx-auto">⚠️</div>
        <h3 className="text-sm font-black text-slate-800 uppercase tracking-wider">Authentication Required</h3>
        <p className="text-xs text-slate-400 font-semibold leading-relaxed">No active relational workspace keys found in memory parameters.</p>
        <button onClick={() => navigate('/login')} className="w-full py-2.5 bg-blue-600 hover:bg-blue-700 text-white font-bold text-xs rounded-xl shadow-xs transition-colors">
          Proceed to Login
        </button>
      </div>
    );
  }

  // Destructure matching parameters safely directly out of parsed API responses
  const { user, tenant, subscription, hasUsedFreePlan } = profileData;
  
  // Resolve Display Values matching prioritized data hierarchies (User DTO -> Tenant Person DTO -> Claims Fallbacks)
  const resolvedFirstName = user?.person?.firstName || tenant?.person?.firstName || "Operator";
  const resolvedLastName = user?.person?.lastName || tenant?.person?.lastName || "";
  const resolvedFullName = `${resolvedFirstName} ${resolvedLastName}`.trim();
  const resolvedEmail = user?.person?.email || tenant?.person?.email || "No email bound";
  const resolvedPhone = user?.person?.phone || tenant?.person?.phone || "No metadata";
  const resolvedTenantName = tenant?.name || "System Tenant Instance";
  

  return (
    <div className="max-w-4xl mx-auto p-6 space-y-6">
      
      {/* HEADER SECTION WITH USER AVATAR */}
      <div className="bg-white border border-slate-200/80 rounded-2xl p-6 shadow-xs flex flex-col sm:flex-row items-center gap-6">
        <button 
          onClick={handleAvatarClick}
          className="relative group focus:outline-none focus:ring-4 focus:ring-blue-600/10 rounded-full transition-transform active:scale-95 shrink-0"
        >
          <div className="w-20 h-20 bg-blue-600 text-white rounded-full flex items-center justify-center font-black text-2xl shadow-inner border-2 border-white ring-4 ring-slate-100">
            {resolvedFirstName.charAt(0).toUpperCase()}
          </div>
          <div className="absolute inset-0 bg-black/40 rounded-full opacity-0 group-hover:opacity-100 flex items-center justify-center text-[10px] text-white font-bold uppercase tracking-wider transition-opacity">
            Account Node
          </div>
        </button>

        {/* RELATIONAL SYSTEM PROFILE TEXT LAYOUT */}
        <div className="text-center sm:text-left space-y-2 flex-1 w-full min-w-0">
          <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-2">
            <h2 className="text-xl font-black text-slate-900 tracking-tight truncate">{resolvedFullName}</h2>
            {tenantInfoClaims?.isTheOwner  && (
              <span className="self-center sm:self-auto bg-amber-50 border border-amber-200 text-amber-700 text-[9px] font-black uppercase tracking-wider px-2 py-0.5 rounded-md">
                Workspace Owner
              </span>
            )}
          </div>
          
          <div className="flex flex-wrap gap-2 justify-center sm:justify-start items-center text-xs text-slate-500 font-semibold">
            <span className="bg-slate-100 text-slate-700 px-2.5 py-1 rounded-md border border-slate-200/60 font-mono text-[11px]">
              Tenant: {resolvedTenantName}
            </span>
            <span>•</span>
            <span className="truncate">{resolvedEmail}</span>
            {resolvedPhone !== "No metadata" && (
              <>
                <span>•</span>
                <span>{resolvedPhone}</span>
              </>
            )}
          </div>
        </div>
      </div>

      {/* SUBSCRIPTION PIPELINE DISPLAY MATRICES */}
      {
        !tenantInfoClaims?.isTheOwner&&!userInfoClaims?.isEmployee&& (<div className="bg-white border border-slate-200/80 rounded-2xl overflow-hidden shadow-xs">
        <div className="p-4 bg-slate-50/80 border-b border-slate-100 flex justify-between items-center">
          <h3 className="text-[11px] font-black uppercase text-slate-500 tracking-wider">Workspace Subscription Matrix</h3>
          <span className={`h-2 w-2 rounded-full ${subscription ? 'bg-emerald-500' : 'bg-amber-400'}`} />
        </div>

        <div className="p-6">
          {subscription?(
            /* CASE 1: Active database allocation parameters identified */
            <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-6 animate-in fade-in duration-200">
              <div className="space-y-2">
                <span className="text-[10px] font-black text-slate-400 uppercase tracking-wider block">Provisioned Service Layer</span>
                <p className="text-lg font-black text-blue-600 tracking-tight">
                  Pricing Option #{subscription.pricingOptionId}
                </p>
                <div className="text-xs text-slate-400 font-semibold space-y-0.5">
                  <p>Deployment Active: <span className="text-slate-700 font-bold">{new Date(subscription.startedAt).toLocaleDateString()}</span></p>
                  <p>Next Scheduled Cycle Process: <span className="text-slate-700 font-bold">{new Date(subscription.endsAt).toLocaleDateString()}</span></p>
                </div>
              </div>

              <div className="text-left sm:text-right bg-blue-50/40 p-4 rounded-xl border border-blue-100/60 w-full sm:w-auto shrink-0">
                <span className="text-[10px] font-bold text-blue-600 uppercase block tracking-wider">Rate Metric</span>
                <span className="text-xl font-black text-slate-800 font-mono">
                  {subscription.pricingOption?.amount} {subscription.pricingOption?.currency}
                </span>
                <span className="text-[10px] text-slate-400 font-bold block">/ billing cycle</span>
              </div>
            </div>
          ) : (
            /* CASE 2: Subscription database row returns null or inactive */
            <div className="space-y-4 animate-in fade-in duration-200">
              <div className="max-w-md">
                <h4 className="text-sm font-bold text-slate-800">No Premium Deployment Plan Detected</h4>
                <p className="text-xs text-slate-400 font-semibold leading-relaxed mt-1">
                  Your tenant operational context is executing inside an unmapped infrastructure layer.
                </p>
              </div>

              {!hasUsedFreePlan && (
                /* Sub-Case A: Unused Free Try verified by backend constraints */
                <div className="bg-blue-50/50 border border-blue-100 rounded-xl p-4 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
                  <div className="space-y-0.5">
                    <span className="text-xs font-black text-blue-700 uppercase tracking-wide block">Free Benefit Option Available</span>
                    <p className="text-xs text-slate-500 font-medium">Your tenant identity is fully authorized to switch into an isolated free evaluation sandbox runtime environment.</p>
                  </div>
                  <button 
                    onClick={() => navigate('/payment-process?tier=free-trial')}
                    className="w-full sm:w-auto bg-blue-600 hover:bg-blue-700 text-white font-bold text-xs px-4 py-2.5 rounded-xl shadow-xs transition-colors shrink-0"
                  >
                    Activate Free Try
                  </button>
                </div>
              )  
                }
                
        
            </div>
          )}
                  {
              (  <div className="bg-slate-50 border border-slate-200 p-4 py-10 mt-10 rounded-xl flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
                  <p className="text-xs text-slate-500 font-medium leading-relaxed">
                    do not wait until your  trial expires, upgrade to a premium plan to unlock advanced features and ensure uninterrupted service,
                    you wont lose the time for the trial you have already used, 
                    it will be added to your new subscription cycle.
          
                  </p>
                  <button 
                    onClick={() => navigate('/upgrade-subscription')}
                    className="w-full sm:w-auto bg-slate-900 hover:bg-slate-800 text-white font-bold text-xs px-4 py-2.5 rounded-xl shadow-xs transition-colors shrink-0"
                  >
                    Upgrade 
                  </button>
                </div>
              )
                }
        </div>
      </div>)
      }

    </div>
  );
}