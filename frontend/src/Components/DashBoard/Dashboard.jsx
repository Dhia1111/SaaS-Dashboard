import React, { useState } from "react";
import { Link, Outlet, useLocation } from "react-router-dom";

export default function DashboardLayout() {
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);
  const location = useLocation();

  const navigationItems = [
    {
      path: "/dashboard/user",
      label: "Users Management",
      icon: (
        <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
          <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
        </svg>
      ),
    },
    {
      path: "/dashboard/analytics",
      label: "Business Analysis",
      icon: (
        <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
          <path strokeLinecap="round" strokeLinejoin="round" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 002 2h2a2 2 0 002-2z" />
        </svg>
      ),
    },
    {
      path: "/dashboard/payments",
      label: "Tenant Payments",
      icon: (
        <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
          <path strokeLinecap="round" strokeLinejoin="round" d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
        </svg>
      ),
    },
    {
      path: "/dashboard/subscriptions",
      label: "Subscription Settings",
      icon: (
        <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
          <path strokeLinecap="round" strokeLinejoin="round" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
        </svg>
      ),
    },
     {
      path: "/dashboard/permissions",
      label: "Permissions",
     icon: (
  <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
    {/* Shield Boundary */}
    <path strokeLinecap="round" strokeLinejoin="round" d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" />
    {/* Explicit Access Keyhole */}
    <path strokeLinecap="round" strokeLinejoin="round" d="M12 8a2.5 2.5 0 00-2 4l-1 3.5h6l-1-3.5a2.5 2.5 0 00-2-4z" />
  </svg>
),
    },
  ];

  return (
    <div className="min-h-screen bg-slate-50 flex flex-col md:flex-row font-sans">
      
      {/* MOBILE HEADER BAR */}
      <div className="md:hidden bg-white border-b border-slate-200 px-4 py-4 flex items-center justify-between shadow-sm z-50">
        <div className="flex items-center gap-3">
          <div className="h-9 w-9 bg-indigo-600 rounded-lg flex items-center justify-center shadow-md shadow-indigo-100">
            <span className="text-white font-black text-xl">B</span>
          </div>
          <span className="font-bold text-slate-900 tracking-tight">BillFlow Admin</span>
        </div>
        <button 
          onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
          className="p-2 rounded-xl border border-slate-200 text-slate-600 hover:bg-slate-50 outline-none"
        >
          <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
            <path strokeLinecap="round" strokeLinejoin="round" d={mobileMenuOpen ? "M6 18L18 6M6 6l12 12" : "M4 6h16M4 12h16M4 18h16"} />
          </svg>
        </button>
      </div>

      {/* SIDEBAR NAVIGATION */}
      <aside className={`
        fixed inset-y-0 left-0 bg-white border-r border-slate-200/80 w-64 p-6 z-40 transform transition-transform duration-300 ease-in-out flex flex-col justify-between
        md:relative md:transform-none md:flex h-screen sticky top-0
        ${mobileMenuOpen ? "translate-x-0" : "-translate-x-full md:translate-x-0"}
      `}>
        <div className="space-y-8">
          <div className="hidden md:flex items-center gap-3 px-2">
            <div className="h-10 w-10 bg-indigo-600 rounded-xl flex items-center justify-center shadow-lg shadow-indigo-100">
              <span className="text-white font-black text-2xl">B</span>
            </div>
            <div>
              <h1 className="font-extrabold text-slate-900 tracking-tight leading-none">BillFlow</h1>
              <span className="text-[10px] font-bold text-slate-400 uppercase tracking-widest mt-1 block">SaaS Gateway</span>
            </div>
          </div>

          <nav className="space-y-1.5">
            <span className="block text-[10px] font-bold text-slate-400 uppercase tracking-widest px-2 mb-3">
              Management Workspace
            </span>
            {navigationItems.map((item) => {
              // Validates if the current route matches the button path
              const isSelected = location.pathname.startsWith(item.path);
              return (
                <Link
                  key={item.path}
                  to={item.path}
                  onClick={() => setMobileMenuOpen(false)}
                  className={`w-full flex items-center gap-3.5 px-4 py-3.5 rounded-xl font-bold text-sm transition-all duration-200 outline-none group ${
                    isSelected
                      ? "bg-indigo-600 text-white shadow-lg shadow-indigo-100"
                      : "text-slate-500 hover:text-slate-900 hover:bg-slate-50"
                  }`}
                >
                  <span className={`${isSelected ? "text-white" : "text-slate-400 group-hover:text-slate-600"}`}>
                    {item.icon}
                  </span>
                  {item.label}
                </Link>
              );
            })}
          </nav>
        </div>

        <div className="pt-4 border-t border-slate-100 flex items-center gap-3 px-2">
          <div className="h-10 w-10 bg-slate-100 rounded-xl flex items-center justify-center border border-slate-200 font-bold text-slate-700">
            AM
          </div>
          <div className="flex-1 min-w-0">
            <p className="text-sm font-bold text-slate-900 truncate">Account Manager</p>
            <p className="text-xs font-semibold text-slate-400 truncate">manager@tenant.com</p>
          </div>
        </div>
      </aside>

      {mobileMenuOpen && (
        <div onClick={() => setMobileMenuOpen(false)} className="fixed inset-0 bg-slate-900/20 backdrop-blur-sm z-30 md:hidden" />
      )}

      {/* REACT ROUTER ROUTE OUTLET VIEWPORT */}
      <main className="flex-1 p-6 md:p-10 overflow-x-hidden">
        <div className="max-w-6xl mx-auto space-y-8 py-20">
          <Outlet />
        </div>
      </main>

    </div>
  );
}