import React from "react";

export default function TenantPayments() {
  return (
    <div className="space-y-6 animate-in fade-in duration-200">
      <div>
        <h2 className="text-2xl font-extrabold text-slate-900 tracking-tight">Tenant Payment Management</h2>
        <p className="text-sm text-slate-500">Monitor processing bills, structured transactions, and direct invoice executions.</p>
      </div>

      <div className="bg-white border border-slate-200/60 rounded-2xl shadow-sm overflow-hidden">
        <div className="p-4 bg-slate-50/70 border-b border-slate-100 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
          <span className="text-xs font-bold text-slate-400 uppercase tracking-widest">Transaction Log</span>
          <input type="text" placeholder="Search invoices..." className="px-3 py-1.5 text-xs border border-slate-200 bg-white rounded-lg outline-none focus:border-indigo-500 transition-all w-full sm:w-48" />
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-slate-50/30 border-b border-slate-100">
                <th className="p-4 text-[11px] font-bold text-slate-400 uppercase tracking-widest">Invoice Code</th>
                <th className="p-4 text-[11px] font-bold text-slate-400 uppercase tracking-widest">Organization / Workspace</th>
                <th className="p-4 text-[11px] font-bold text-slate-400 uppercase tracking-widest">Volume</th>
                <th className="p-4 text-[11px] font-bold text-slate-400 uppercase tracking-widest">Settlement</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 text-sm font-medium text-slate-700">
              {[
                { id: "INV-2026-001", tenant: "Acme Corporate", amount: "$2,450.00", status: "Settled", style: "text-emerald-700 bg-emerald-50 border-emerald-100" },
                { id: "INV-2026-002", tenant: "Globex Frameworks", amount: "$1,800.00", status: "Processing", style: "text-amber-700 bg-amber-50 border-amber-100" }
              ].map((invoice, i) => (
                <tr key={i} className="hover:bg-slate-50/50 transition-colors">
                  <td className="p-4 font-bold text-slate-900">{invoice.id}</td>
                  <td className="p-4 text-slate-500">{invoice.tenant}</td>
                  <td className="p-4 font-bold text-slate-900">{invoice.amount}</td>
                  <td className="p-4">
                    <span className={`px-2 py-0.5 text-xs font-bold rounded-md border ${invoice.style}`}>{invoice.status}</span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}