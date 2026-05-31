import React, { useState } from "react";
import { useLoaderData } from "react-router-dom";
import { GetAuthorizationOptionsAsync, GetUserRolesAsync } from '../../APIs/Users.js';
import { SendInvitationAsync } from '../../APIs/UserAuth.js';

export default function AddUser() {
    const { AllAthorization, AllRoles } = useLoaderData();
    
    // Form State
    const [email, setEmail] = useState("");
    const [selectedRole, setSelectedRole] = useState(null);
    const [selectedAuths, setSelectedAuths] = useState(0); 
    const [isSending, setIsSending] = useState(false);
    const [statusMsg, setStatusMsg] = useState({ type: "", text: "" });

    const handleAuthChange = (key, checked) => {
        const authKey = Number(key);
        setSelectedAuths(prev => checked ? prev + authKey : prev - authKey);
    };

    const handleSendInvoice = async () => {
        if (!email || !selectedRole) {
            setStatusMsg({ type: "error", text: "Please provide email and role." });
            return;
        }

        setIsSending(true);
        setStatusMsg({ type: "", text: "" });

        const dto = {
            email: email,
            role: Number(selectedRole),
            userAuthorization: selectedAuths,
        };

        const response = await SendInvitationAsync(dto);
        setIsSending(false);

        if (response.success) {
            setStatusMsg({ type: "success", text: "Invitation sent successfully!" });
            setEmail(""); 
        } else {
            setStatusMsg({ type: "error", text: response.message || "Failed to send." });
        }
        setIsSending(false);
    };

    return (
        <div className="min-h-screen bg-slate-50 p-8 flex justify-center">
            <div className="w-full max-w-2xl bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden h-fit">
                {/* Header */}
                <header className="p-8 border-b border-slate-100">
                    <h2 className="text-2xl font-bold text-slate-900">Send Invite Invoice</h2>
                    <p className="text-sm text-slate-500 mt-1">Invite a new user to your organization via email.</p>
                </header>

                {/* Status Alerts */}
                {statusMsg.text && (
                    <div className={`mx-8 mt-6 p-4 rounded-lg text-sm font-medium animate-in fade-in slide-in-from-top-2 ${
                        statusMsg.type === 'error' 
                        ? 'bg-red-50 text-red-700 border border-red-100' 
                        : 'bg-green-50 text-green-700 border border-green-100'
                    }`}>
                        {statusMsg.text}
                    </div>
                )}

                <div className="p-8 space-y-8">
                    {/* Email Input */}
                    <div className="space-y-2">
                        <label className="text-[11px] font-bold text-slate-500 uppercase tracking-wider ml-1">
                            Recipient Email
                        </label>
                        <input 
                            type="email" 
                            className="w-full px-4 py-3 rounded-xl border border-slate-200 outline-none focus:border-indigo-500 focus:ring-4 focus:ring-indigo-500/10 transition-all placeholder:text-slate-400"
                            placeholder="colleague@company.com"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                        />
                    </div>

                    {/* Roles Radio Selection */}
                    <div className="space-y-3">
                        <label className="text-[11px] font-bold text-slate-500 uppercase tracking-wider ml-1">
                            Assigned Role
                        </label>
                        <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
                            {AllRoles.map((role) => (
                                <label key={role.key} className="relative cursor-pointer group">
                                    <input 
                                        type="radio" 
                                        name="userRole" 
                                        className="peer sr-only"
                                        value={role.key}
                                        onChange={(e) => setSelectedRole(e.target.value)}
                                    />
                                    <div className="p-3 text-center rounded-xl border border-slate-200 bg-slate-50 text-slate-600 font-semibold text-sm transition-all peer-checked:bg-indigo-600 peer-checked:text-white peer-checked:border-indigo-600 peer-checked:shadow-lg peer-checked:shadow-indigo-200 group-hover:border-slate-300">
                                        {role.value}
                                    </div>
                                </label>
                            ))}
                        </div>
                    </div>

                    {/* Authorizations Checkboxes */}
                    <div className="space-y-3">
                        <label className="text-[11px] font-bold text-slate-500 uppercase tracking-wider ml-1">
                            Access Permissions
                        </label>
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                            {AllAthorization.map((auth) => (
                                <label key={auth.key} className="flex items-center gap-3 p-3 rounded-xl border border-slate-100 hover:bg-slate-50 transition-colors cursor-pointer group">
                                    <div className="relative flex items-center justify-center">
                                        <input 
                                            type="checkbox" 
                                            className="peer h-5 w-5 appearance-none rounded-md border border-slate-300 bg-white checked:bg-indigo-600 checked:border-indigo-600 transition-all cursor-pointer"
                                            onChange={(e) => handleAuthChange(auth.key, e.target.checked)}
                                        />
                                        <svg className="absolute h-3 w-3 text-white opacity-0 peer-checked:opacity-100 transition-opacity pointer-events-none" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="4">
                                            <path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" />
                                        </svg>
                                    </div>
                                    <span className="text-sm font-medium text-slate-700 group-hover:text-slate-900 transition-colors">
                                        {auth.value}
                                    </span>
                                </label>
                            ))}
                        </div>
                    </div>
                </div>

                {/* Action Footer */}
                <footer className="p-8 bg-slate-50 border-top border-slate-100">
                    <button 
                        className="w-full py-4 rounded-xl bg-indigo-600 text-white font-bold text-lg shadow-lg shadow-indigo-100 hover:bg-indigo-700 hover:-translate-y-0.5 active:scale-[0.98] transition-all disabled:bg-slate-300 disabled:shadow-none disabled:translate-y-0" 
                        disabled={isSending}
                        onClick={handleSendInvoice}
                    >
                        {isSending ? (
                            <span className="flex items-center justify-center gap-2">
                                <svg className="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                </svg>
                                Sending...
                            </span>
                        ) : "Send Invoice & Invite"}
                    </button>
                </footer>
            </div>
        </div>
    );
}

export async function Loader() {
    const [authRes, rolesRes] = await Promise.all([
        GetAuthorizationOptionsAsync(),
        GetUserRolesAsync()
    ]);

    return {
        AllAthorization: authRes.data || [],
        AllRoles: rolesRes.data || []
    };
}