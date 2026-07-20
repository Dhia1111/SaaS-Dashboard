import React, { useState } from "react";
import { useLoaderData, useNavigate } from "react-router-dom";
import { GetAuthorizationOptionsAsync, GetUserRolesAsync, CreatePlatformEmployeeAsync as CreatePlatformUserAsync, UpdateUserAsync  } from '../../APIs/EmployeesManagment.js';

export default function AddEmployee() {
    const { AllAthorization, AllRoles } = useLoaderData();
    const navigate = useNavigate();
    console.log("Auth",JSON.stringify(AllAthorization));
    // Status & Loading Management States
    const [isSending, setIsSending] = useState(false);
    const [statusMsg, setStatusMsg] = useState({ type: "", text: "" });

    // Nested DtoPlatformUser State Definition
    const [formData, setFormData] = useState({
        platformRole: 1,
        adminstationAuth: 0,
        isActive: true,
        user: {
            role: 0,
            authorization: 0,
            isActive: true,
            passwordHash:"",
            person: {
                firstName: "",
                lastName: "",
                email: "",
                phone: "",
                address: "",
                provider: 0,
                isEmailVeryfied: true
            }
        }
    });

    // Helper to safely path update deep nested properties
    const updatePersonField = (field, value) => {
        setFormData(prev => ({
            ...prev,
            user: {
                ...prev.user,
                person: {
                    ...prev.user.person,
                    [field]: value
                }
            }
        }));
    };

    const updateUserField = (field, value) => {
        setFormData(prev => ({
            ...prev,
            user: {
                ...prev.user,
                person: {
                    ...prev.user.person,
                },
            
                [field]: value

            }
        }));
    };
    // Bitmask Combinator: Handles the custom mathematical flag calculations
    const handleAuthBitmaskChange = (bitValue, isChecked) => {
        const numericBit = Number(bitValue);
        setFormData(prev => {
            const currentAuth = prev.user.authorization;
            const updatedAuth = isChecked ? currentAuth + numericBit : currentAuth - numericBit;
            return {
                ...prev,
                adminstationAuth: updatedAuth, // Sync both authorization nodes based on system requirements
                user: {
                    ...prev.user,
                    authorization: updatedAuth
                }
            };
        });
    };

    const handleRoleSelection = (roleKey) => {
        const numericRole = Number(roleKey);
        setFormData(prev => ({
            ...prev,
            platformRole: numericRole,
            user: {
                ...prev.user,
                role: numericRole
            }
        }));
    };

    const handleCreateEmployee = async () => {
        const { firstName, lastName, email, phone } = formData.user.person;
        
        if (!firstName.trim() || !lastName.trim() || !email.trim() || !phone.trim()) {
            setStatusMsg({ type: "error", text: "Please complete all base personal record information keys." });
            return;
        }

        setIsSending(true);
        setStatusMsg({ type: "", text: "" });

        try {
            const response = await CreatePlatformUserAsync(formData);
            if (response.success) {
                setStatusMsg({ type: "success", text: "Employee platform profile provisioned successfully!" });
                // Reset state parameters or navigate back to index grid rows
                setTimeout(() => navigate("/employees"), 2000);
            } else {
                setStatusMsg({ type: "error", text: response.message || "Failed to create employee profile mapping." });
            }
        } catch {
            setStatusMsg({ type: "error", text: "Server communication interface fault caught during post execution." });
        } finally {
            setIsSending(false);
        }
    };

    return (
        <div className="min-h-screen bg-slate-50 p-4 sm:p-8 flex justify-center items-start">
            <div className="w-full max-w-3xl bg-white rounded-2xl border border-slate-200/80 shadow-sm overflow-hidden">
                
                {/* Header Context Deck */}
                <header className="p-6 sm:p-8 bg-blue-600 text-white">
                    <h2 className="text-2xl font-black tracking-tight">Provision Employee Profile</h2>
                    <p className="text-white/80 text-xs mt-1">Register identity credentials and apply functional permission bitmasks inside this tenant scope.</p>
                </header>

                {/* Toast Alerts Notice Block */}
                {statusMsg.text && (
                    <div className={`mx-6 sm:mx-8 mt-6 p-4 rounded-xl text-xs font-bold border transition-all ${
                        statusMsg.type === 'error' 
                        ? 'bg-red-50 text-red-700 border-red-100' 
                        : 'bg-green-50 text-green-700 border-green-100'
                    }`}>
                        <div className="flex items-center gap-2">
                            <span className={`w-2 h-2 rounded-full ${statusMsg.type === 'error' ? 'bg-red-500' : 'bg-green-500'}`} />
                            {statusMsg.text}
                        </div>
                    </div>
                )}

                <div className="p-6 sm:p-8 space-y-10">
                    
                    {/* SECTION 1: PERSONAL DISCRIPTORS */}
                    <section className="space-y-4">
                        <div className="border-b border-slate-100 pb-2">
                            <h3 className="text-sm font-bold text-slate-800">1. Personal Identity Metrics</h3>
                            <p className="text-[11px] text-slate-400 font-medium">Core personal identification descriptors used for system auditing records.</p>
                        </div>
                        
                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                            <div className="space-y-1">
                                <label className="text-[10px] font-bold text-slate-400 uppercase tracking-wide ml-1">First Name</label>
                                <input 
                                    type="text" 
                                    className="w-full px-4 py-2.5 rounded-xl border border-slate-200 text-xs font-semibold text-slate-800 outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-500/10 transition-all"
                                    placeholder="Jane"
                                    value={formData.user.person.firstName}
                                    onChange={(e) => updatePersonField("firstName", e.target.value)}
                                />
                            </div>
                            <div className="space-y-1">
                                <label className="text-[10px] font-bold text-slate-400 uppercase tracking-wide ml-1">Last Name</label>
                                <input 
                                    type="text" 
                                    className="w-full px-4 py-2.5 rounded-xl border border-slate-200 text-xs font-semibold text-slate-800 outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-500/10 transition-all"
                                    placeholder="Doe"
                                    value={formData.user.person.lastName}
                                    onChange={(e) => updatePersonField("lastName", e.target.value)}
                                />
                            </div>
                        </div>

                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                            <div className="space-y-1">
                                <label className="text-[10px] font-bold text-slate-400 uppercase tracking-wide ml-1">Communication Email</label>
                                <input 
                                    type="email" 
                                    className="w-full px-4 py-2.5 rounded-xl border border-slate-200 text-xs font-semibold text-slate-800 outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-500/10 transition-all"
                                    placeholder="j.doe@enterprise.com"
                                    value={formData.user.person.email}
                                    onChange={(e) => updatePersonField("email", e.target.value)}
                                />
                            </div>
                            <div className="space-y-1">
                                <label className="text-[10px] font-bold text-slate-400 uppercase tracking-wide ml-1">Phone Number Connection</label>
                                <input 
                                    type="tel" 
                                    className="w-full px-4 py-2.5 rounded-xl border border-slate-200 text-xs font-semibold text-slate-800 outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-500/10 transition-all"
                                    placeholder="+1 (555) 000-0000"
                                    value={formData.user.person.phone}
                                    onChange={(e) => updatePersonField("phone", e.target.value)}
                                />
                            </div>
                        </div>

                        <div className="space-y-1">
                            <label className="text-[10px] font-bold text-slate-400 uppercase tracking-wide ml-1">Physical Base Address</label>
                            <input 
                                type="text" 
                                className="w-full px-4 py-2.5 rounded-xl border border-slate-200 text-xs font-semibold text-slate-800 outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-500/10 transition-all"
                                placeholder="HQ - Floor 4, Suite 402, Tech District"
                                value={formData.user.person.address}
                                onChange={(e) => updatePersonField("address", e.target.value)}
                            />
                        </div>
                    </section>

                    {/* SECTION 2: ROLES RADIO MATRIX */}
                    <section className="space-y-4">
                        <div className="border-b border-slate-100 pb-2">
                            <h3 className="text-sm font-bold text-slate-800">2. Account Functional Role</h3>
                            <p className="text-[11px] text-slate-400 font-medium">Select the foundational classification matrix mapping determining the base interface viewports.</p>
                        </div>
                        
                        <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
                            {AllRoles.map((role) => {
                                const isSelected = formData.platformRole === Number(role.key);
                                return (
                                    <label key={role.key} className="relative cursor-pointer group select-none">
                                        <input 
                                            type="radio" 
                                            name="platformRole" 
                                            className="peer sr-only"
                                            value={role.key}
                                            checked={isSelected}
                                            onChange={() => handleRoleSelection(role.key)}
                                        />
                                        <div className="p-3 text-center rounded-xl border border-slate-200 bg-slate-50 text-slate-600 font-bold text-xs transition-all peer-checked:bg-blue-600 peer-checked:text-white peer-checked:border-blue-600 peer-checked:shadow-md group-hover:border-slate-300">
                                            {role.value}
                                        </div>
                                    </label>
                                );
                            })}
                        </div>
                    </section>

                    {/* SECTION 3: AUTHORIZATIONS ACCESS CODES BITMASK */}
                    <section className="space-y-4">
                        <div className="border-b border-slate-100 pb-2 flex justify-between items-end">
                            <div>
                                <h3 className="text-sm font-bold text-slate-800">3. Functional Security Bitmask</h3>
                                <p className="text-[11px] text-slate-400 font-medium">Toggle live authority permissions scopes mapping across endpoints pipelines.</p>
                            </div>
                            <div className="bg-slate-100 border border-slate-200 px-3 py-1 rounded-lg text-right font-mono text-[11px] font-extrabold text-slate-600">
                                Compiled Mask Value: {formData.user.authorization}
                            </div>
                        </div>

                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                            {AllAthorization.map((auth) => {
                                const isChecked = (formData.user.authorization & Number(auth.key)) === Number(auth.key) && Number(auth.key) !== 0;
                                return (
                                    <label key={auth.key} className={`flex items-center gap-3 p-3 rounded-xl border transition-all cursor-pointer select-none group ${
                                        isChecked 
                                            ? "bg-blue-50/50 border-blue-500/30 shadow-xs" 
                                            : "bg-white border-slate-200/80 hover:border-slate-300"
                                    }`}>
                                        <div className="relative flex items-center justify-center shrink-0">
                                            <input 
                                                type="checkbox" 
                                                checked={isChecked}
                                                className="peer h-4 w-4 appearance-none rounded-md border border-slate-300 bg-white checked:bg-blue-600 checked:border-blue-600 transition-all cursor-pointer"
                                                onChange={(e) => handleAuthBitmaskChange(auth.key, e.target.checked)}
                                            />
                                            <svg className="absolute h-2.5 w-2.5 text-white opacity-0 peer-checked:opacity-100 transition-opacity pointer-events-none" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="4">
                                                <path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" />
                                            </svg>
                                        </div>
                                        <span className="text-xs font-bold text-slate-700 group-hover:text-slate-900 transition-colors">
                                            {auth.value}
                                        </span>
                                    </label>
                                );
                            })}
                        </div>
                    </section>
                                 {/* SECTION 4 */}

                    <section className="space-y-4 ">
                        <div className="border-b border-slate-100 pb-2">
                            <h3 className="text-sm font-bold text-slate-800">4. Account Activation</h3>
                       <div>
              <label className="flex items-center gap-3 p-3 rounded-xl border border-slate-100 bg-slate-50/50 hover:bg-slate-50 transition-colors cursor-pointer group w-fit pr-6">
                <input 
                  type="checkbox" 
                  checked={formData.isActive}
                  onChange={(e) => setFormData(prev=>({ ...prev, isActive: e.target.checked }))}
                  className="h-5 w-5 rounded-md border-slate-300 text-blue-600 focus:ring-blue-500 cursor-pointer"
                />
                <span className="text-sm font-bold text-slate-700 group-hover:text-slate-900 transition-colors">Activate employee immediately</span>
              </label>
            </div>
            
          </div>
                     <div className="flex-column grid-cols-1 sm:grid-cols-2 gap-4">
                           
                                <label className="text-[10px] font-bold text-slate-400 uppercase tracking-wide ml-1">Pass Word</label>
                                <input 
                                    type="text" 
                                    className="w-full px-4 py-2.5 rounded-xl border border-slate-200 text-xs font-semibold text-slate-800 outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-500/10 transition-all"
                                    placeholder="Pass Word"
                                    value={formData.user.passwordHash}
                                    onChange={(e) =>{ updateUserField("passwordHash",e.target.value) }}
                                />
                            </div>
                    
                    </section>
                </div>

                {/* Action Form Footer Submit Panel */}
                <footer className="p-6 sm:p-8 bg-slate-50 border-t border-slate-100 flex justify-end">
                    <button 
                        type="button"
                        disabled={isSending}
                        onClick={handleCreateEmployee}
                        className="w-full sm:w-auto px-8 py-3.5 rounded-xl bg-primary text-white font-bold text-xs tracking-wide uppercase shadow-md shadow-emerald-900/10 hover:bg-blue-400 active:scale-[0.98] transition-all disabled:bg-blue-700 disabled:shadow-none" 
                    >
                        {isSending ? (
                            <span className="flex items-center justify-center gap-2">
                                <svg className="animate-spin h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                </svg>
                                Provisioning Space...
                            </span>
                        ) : "Commit & Provision Employee"}
                    </button>
                </footer>
            </div>
        </div>
    );
}

// React Router Master Parallel Data Fetcher Loader Block
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