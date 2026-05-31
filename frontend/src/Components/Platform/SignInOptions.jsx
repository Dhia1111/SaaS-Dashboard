import React from "react";
import { useNavigate } from "react-router-dom";

export default function AuthGateway() {
  const navigate = useNavigate();

  return (
    <div className="min-h-screen bg-slate-50 flex flex-col justify-center py-12 px-4 sm:px-6 lg:px-8">
      {/* Header / Branding */}
      <div className="sm:mx-auto sm:w-full sm:max-w-md text-center mb-10">
        <div className="inline-flex h-14 w-14 bg-indigo-600 rounded-2xl items-center justify-center shadow-xl shadow-indigo-200 mb-4">
          <span className="text-white font-black text-3xl">B</span>
        </div>
        <h2 className="text-3xl font-extrabold text-slate-900 tracking-tight">
          Welcome to BillFlow
        </h2>
        <p className="mt-2 text-sm text-slate-500">
          Please select your access type to continue to your workspace.
        </p>
      </div>

      {/* Choice Matrix Container */}
      <div className="sm:mx-auto sm:w-full sm:max-w-3xl">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          
          {/* Option 1: Tenant / Organization Workspace Access */}
          <div 
            onClick={() => navigate("/login")} 
            className="group relative bg-white p-8 rounded-3xl border border-slate-200/80 shadow-sm hover:shadow-xl hover:shadow-slate-200/60 hover:border-indigo-500/50 transition-all duration-300 cursor-pointer flex flex-col justify-between"
          >
            <div>
              {/* Icon */}
              <div className="h-12 w-12 bg-indigo-50 text-indigo-600 rounded-xl flex items-center justify-center mb-6 group-hover:bg-indigo-600 group-hover:text-white transition-all duration-300">
                <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                </svg>
              </div>
              
              <h3 className="text-xl font-bold text-slate-900 mb-2 group-hover:text-indigo-600 transition-colors">
                Organization Workspace
              </h3>
              <p className="text-sm text-slate-500 leading-relaxed">
                Access your company's dedicated multi-tenant infrastructure or build out a new business corporate space.
              </p>
            </div>

            <div className="mt-8 pt-4 border-t border-slate-50 flex items-center text-sm font-bold text-indigo-600 group-hover:text-indigo-700">
              Go to Corporate Space
              <svg className="h-4 w-4 ml-1 transform group-hover:translate-x-1 transition-transform" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2.5">
                <path strokeLinecap="round" strokeLinejoin="round" d="M9 5l7 7-7 7" />
              </svg>
            </div>
          </div>

          {/* Option 2: Tenant Account Manager Login */}
          <div 
            onClick={() => navigate("/user-auth/login")} 
            className="group relative bg-white p-8 rounded-3xl border border-slate-200/80 shadow-sm hover:shadow-xl hover:shadow-slate-200/60 hover:border-indigo-500/50 transition-all duration-300 cursor-pointer flex flex-col justify-between"
          >
            <div>
              {/* Icon */}
              <div className="h-12 w-12 bg-emerald-50 text-emerald-600 rounded-xl flex items-center justify-center mb-6 group-hover:bg-emerald-600 group-hover:text-white transition-all duration-300">
                <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
                </svg>
              </div>

              <h3 className="text-xl font-bold text-slate-900 mb-2 group-hover:text-emerald-600 transition-colors">
                Account Manager
              </h3>
              <p className="text-sm text-slate-500 leading-relaxed">
                Log in as an authorized admin or tenant manager using your secure email credentials and verified token access.
              </p>
            </div>

            <div className="mt-8 pt-4 border-t border-slate-50 flex items-center text-sm font-bold text-emerald-600 group-hover:text-emerald-700">
              Sign In as Manager
              <svg className="h-4 w-4 ml-1 transform group-hover:translate-x-1 transition-transform" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2.5">
                <path strokeLinecap="round" strokeLinejoin="round" d="M9 5l7 7-7 7" />
              </svg>
            </div>
          </div>

        </div>
      </div>

      {/* Footer Branding */}
      <p className="mt-12 text-center text-xs text-slate-400 uppercase tracking-widest font-bold">
        Secure Multi-Tenant Gateway &bull; BillFlow
      </p>
    </div>
  );
}