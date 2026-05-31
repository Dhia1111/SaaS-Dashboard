import React, { useState, useEffect } from "react";
import { useDispatch } from "react-redux";
import { useSearchParams, useNavigate } from "react-router-dom";
import { setAccessToken } from '../../globalStates/AccessToken';    
import { VerifyUserAsync} from "../../APIs/UserAuth.js";

export default function UserAuthVerify() {
    const [searchParams] = useSearchParams();
    const dispatch = useDispatch();
    const navigate = useNavigate();

    // Form State
    const [formData, setFormData] = useState({
        password: "",
        confirmPassword: "",
        email: ""
    });
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);

    // 1. Capture Token from URL on mount
    useEffect(() => {
        const token = searchParams.get("accessToken");
        if (token) {
            dispatch(setAccessToken(token));
        }  
    }, [searchParams, dispatch]);

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
        if (error) setError(""); // Clear error when typing
    };

    const handleVerify = async (e) => {
        e.preventDefault();
if (formData.email==null || formData.email==""){
            setError("Email is required.");
            return;
        }
        if (formData.password !== formData.confirmPassword) {
            setError("Passwords do not match.");
            return;
        }
        // 2. Client-side Validation
        if (formData.password.length < 8) {
            setError("Password must be at least 8 characters long.");
            return;
        }
       
        setLoading(true);
           // Your interceptor will automatically add the token from Redux
            const response = await VerifyUserAsync({ 
                email: formData.email,
                password: formData.password 
            });
            if (response.success) {
                setSuccess(true);
                // Optional: Redirect to login or dashboard after 2 seconds
                dispatch(setAccessToken(response.data)); // Clear token after successful verification
                setTimeout(() => navigate("/user"), 2000);
            } else {
                console.log("Error response:", JSON.stringify(response));  
                setError(response.message || "Verification failed.");
            }
           
           
        
    };

    return (
        <div className="min-h-screen bg-slate-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
            <div className="sm:mx-auto sm:w-full sm:max-w-md">
                <h2 className="text-center text-3xl font-extrabold text-slate-900">
                    Verify Your Account
                </h2>
                <p className="mt-2 text-center text-sm text-slate-600">
                    Set your password to activate your BillFlow access.
                </p>
            </div>

            <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
                <div className="bg-white py-8 px-4 shadow-xl shadow-slate-200/50 sm:rounded-2xl sm:px-10 border border-slate-100">
                    {success ? (
                        <div className="text-center space-y-4 py-4 animate-in zoom-in duration-300">
                            <div className="mx-auto flex items-center justify-center h-12 w-12 rounded-full bg-green-100">
                                <svg className="h-6 w-6 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M5 13l4 4L19 7" />
                                </svg>
                            </div>
                            <h3 className="text-lg font-medium text-slate-900">Account Verified!</h3>
                            <p className="text-sm text-slate-500">Redirecting you to login...</p>
                        </div>
                    ) : (
                        <form className="space-y-6" onSubmit={handleVerify}>
                            {error && (
                                <div className="p-3 rounded-lg bg-red-50 border border-red-100 text-red-700 text-xs font-medium">
                                    {error}
                                </div>
                            )}
                            {!searchParams.get("accessToken") && (
                                <div className="p-3 rounded-lg bg-yellow-50 border border-yellow-100 text-yellow-700 text-xs font-medium">
                                    "Missing verification token. Please check your email link."
                                </div>
                            )}
                             <div>
                                <label className="block text-xs font-bold text-slate-500 uppercase tracking-wide">
                                    Email Address
                                </label>
                                <input
                                    name="email"
                                    type="email"
                                    required
                                    className="mt-1 block w-full px-4 py-3 border border-slate-200 rounded-xl shadow-sm focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 outline-none transition-all"
                                    placeholder="you@example.com"
                                    value={formData.email}
                                    onChange={handleChange}
                                />
                            </div>
                            <div>
                                <label className="block text-xs font-bold text-slate-500 uppercase tracking-wide">
                                    New Password
                                </label>
                                <input
                                    name="password"
                                    type="password"
                                    onChange={handleChange}
                                />
                            </div>
                            <div>
                                <label className="block text-xs font-bold text-slate-500 uppercase tracking-wide">
                                    New Password
                                </label>
                                <input
                                    name="password"
                                    type="password"
                                    required
                                    className="mt-1 block w-full px-4 py-3 border border-slate-200 rounded-xl shadow-sm focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 outline-none transition-all"
                                    placeholder="••••••••"
                                    value={formData.password}
                                    onChange={handleChange}
                                />
                            </div>

                            <div>
                                <label className="block text-xs font-bold text-slate-500 uppercase tracking-wide">
                                    Confirm Password
                                </label>
                                <input
                                    name="confirmPassword"
                                    type="password"
                                    required
                                    className="mt-1 block w-full px-4 py-3 border border-slate-200 rounded-xl shadow-sm focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 outline-none transition-all"
                                    placeholder="••••••••"
                                    value={formData.confirmPassword}
                                    onChange={handleChange}
                                />
                            </div>

                            <button
                                type="submit"
                                disabled={loading}
                                className="w-full flex justify-center py-3.5 px-4 border border-transparent rounded-xl shadow-lg shadow-indigo-200 text-sm font-bold text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:bg-slate-300 disabled:shadow-none transition-all active:scale-[0.98]"
                            >
                                {loading ? "Processing..." : "Verify & Activate"}
                            </button>
                        </form>
                    )}
                </div>
            </div>
        </div>
    );
}