import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import {  LoginUserAsync } from "../../Apis/UserAuth"; // Assume your login API exists here

export default function TenantLogin() {
  const [formData, setFormData] = useState({
    email: "",
    password: "",
    tenantName: ""
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    if (error) setError("");
  };

  const handleLogin = async (e) => {
    e.preventDefault();
    setLoading(true);
    
    try {
      const response = await LoginUserAsync(formData);
      
      if (response.state === 1) {
        // Your interceptor/middleware will handle the token storage from here
        navigate("/dashboard");
      } else {
        setError(response.message || "Invalid credentials or workspace name.");
      }
    } catch  {
      setError("Server connection failed. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-slate-50 flex flex-col justify-center py-12 px-4 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <div className="flex justify-center mb-6">
          {/* BillFlow Logo Placeholder */}
          <div className="h-12 w-12 bg-indigo-600 rounded-xl flex items-center justify-center shadow-lg shadow-indigo-200">
            <span className="text-white font-bold text-2xl">B</span>
          </div>
        </div>
        <h2 className="text-center text-3xl font-extrabold text-slate-900 tracking-tight">
          Manager Login
        </h2>
        <p className="mt-2 text-center text-sm text-slate-500">
          Access your organization's dashboard
        </p>
      </div>

      <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-10 px-4 shadow-2xl shadow-slate-200/60 rounded-3xl border border-slate-100 sm:px-12">
          <form className="space-y-5" onSubmit={handleLogin}>
            
            {error && (
              <div className="p-4 rounded-xl bg-red-50 border border-red-100 text-red-700 text-xs font-semibold flex items-center gap-2 animate-shake">
                <svg className="h-4 w-4" fill="currentColor" viewBox="0 0 20 20">
                  <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                </svg>
                {error}
              </div>
            )}

            {/* Tenant/Workspace Name */}
            <div>
              <label className="block text-[11px] font-bold text-slate-400 uppercase tracking-widest ml-1 mb-2">
                Workspace Name (Tenant)
              </label>
              <div className="relative">
                <input
                  name="tenantName"
                  type="text"
                  required
                  className="block w-full px-4 py-3.5 bg-slate-50 border border-slate-200 rounded-2xl focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 focus:bg-white outline-none transition-all placeholder:text-slate-300 text-slate-700"
                  placeholder="e.g. acme-corp"
                  value={formData.tenantName}
                  onChange={handleChange}
                />
              </div>
            </div>

            {/* Email Field */}
            <div>
              <label className="block text-[11px] font-bold text-slate-400 uppercase tracking-widest ml-1 mb-2">
                Email Address
              </label>
              <input
                name="email"
                type="email"
                required
                className="block w-full px-4 py-3.5 bg-slate-50 border border-slate-200 rounded-2xl focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 focus:bg-white outline-none transition-all placeholder:text-slate-300"
                placeholder="manager@company.com"
                value={formData.email}
                onChange={handleChange}
              />
            </div>

            {/* Password Field */}
            <div>
              <div className="flex justify-between ml-1 mb-2">
                <label className="block text-[11px] font-bold text-slate-400 uppercase tracking-widest">
                  Password
                </label>
                <Link to="/forgot-password" size="sm" className="text-[11px] font-bold text-indigo-600 uppercase tracking-widest hover:text-indigo-700">
                  Forgot?
                </Link>
              </div>
              <input
                name="password"
                type="password"
                required
                className="block w-full px-4 py-3.5 bg-slate-50 border border-slate-200 rounded-2xl focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 focus:bg-white outline-none transition-all placeholder:text-slate-300"
                placeholder="••••••••"
                value={formData.password}
                onChange={handleChange}
              />
            </div>

            <button
              type="submit"
              disabled={loading}
              className="w-full flex justify-center items-center py-4 px-4 border border-transparent rounded-2xl shadow-xl shadow-indigo-100 text-sm font-bold text-white bg-indigo-600 hover:bg-indigo-700 hover:-translate-y-0.5 active:scale-[0.98] focus:outline-none transition-all disabled:bg-slate-300 disabled:shadow-none disabled:translate-y-0 mt-6"
            >
              {loading ? (
                <svg className="animate-spin h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
              ) : "Sign In to Dashboard"}
            </button>
          </form>
          
          <div className="mt-8 pt-6 border-t border-slate-100 text-center">
            <p className="text-sm text-slate-500">
              New organization? <Link to="/signup" className="text-indigo-600 font-bold hover:underline">Create a tenant account</Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}