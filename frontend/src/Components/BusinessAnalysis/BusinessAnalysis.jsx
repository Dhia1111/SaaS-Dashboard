import React from "react";

export default function BusinessAnalysis() {
  return (
    <div className="space-y-6 animate-in fade-in duration-200">
      <div>
        <h2 className="text-2xl font-extrabold text-slate-900 tracking-tight">Business Analysis</h2>
        <p className="text-sm text-slate-500">Track structural data matrices, operational conversions, and tenant growth metrics.</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="bg-white p-6 border border-slate-200/60 rounded-2xl shadow-sm space-y-4">
          <div className="flex justify-between items-center border-b border-slate-100 pb-3">
            <h3 className="font-bold text-slate-900 text-sm">Conversion Matrix</h3>
            <span className="text-xs font-bold text-indigo-600 bg-indigo-50 px-2.5 py-1 rounded-lg">Real-time</span>
          </div>
          <div className="h-48 bg-slate-50 border border-dashed border-slate-200 rounded-xl flex items-center justify-center text-xs text-slate-400 font-bold uppercase tracking-wider">
            [Conversion Flow Chart Engine Context]
          </div>
        </div>

        <div className="bg-white p-6 border border-slate-200/60 rounded-2xl shadow-sm space-y-4">
          <div className="flex justify-between items-center border-b border-slate-100 pb-3">
            <h3 className="font-bold text-slate-900 text-sm">Tenant Growth Trajectory</h3>
            <span className="text-xs font-bold text-indigo-600 bg-indigo-50 px-2.5 py-1 rounded-lg">Monthly</span>
          </div>
          <div className="h-48 bg-slate-50 border border-dashed border-slate-200 rounded-xl flex items-center justify-center text-xs text-slate-400 font-bold uppercase tracking-wider">
            [Growth Over Time Linear Plot Context]
          </div>
        </div>
      </div>
    </div>
  );
}