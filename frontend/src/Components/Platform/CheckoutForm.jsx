import React, { useState } from "react";
import { useCheckoutElements, PaymentElement } from "@stripe/react-stripe-js/checkout";

const paymentElementOptions = {
  layout: "tabs",
};

export default function CheckoutForm() {
  const checkoutState = useCheckoutElements();
  const [isProcessing, setIsProcessing] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  // 1. Handle loading state
  if (checkoutState.type === "loading") {
    return (
      <div className="w-full max-w-md mx-auto bg-white border border-slate-200 rounded-2xl p-6 flex justify-center items-center">
        <span className="text-sm text-slate-500">Loading checkout...</span>
      </div>
    );
  }

  // 2. Handle initialization errors
  if (checkoutState.type === "error") {
    return (
      <div className="w-full max-w-md mx-auto bg-red-50 border border-red-200 rounded-2xl p-4 text-red-700 text-sm">
        {checkoutState.error.message}
      </div>
    );
  }

  // 3. Handle payment submission
  const handleSubmit = async (e) => {
    e.preventDefault();

    setIsProcessing(true);
    setErrorMessage("");

    try {
      
      const {checkout} =  checkoutState;
      const result= await checkout.confirm({email:"dhiaeddine000222@gmail.com"});

      // Stripe handles successful redirects automatically. 
      // We only handle the local state if an error occurs.
      if (result && result.type === "error") {
        setErrorMessage(result.error.message);
      }
    } catch (error) {
      // Catch unexpected code/network failures
      setErrorMessage("An unexpected error occurred. Please try again.");
      console.error(error);
    } finally {
      setIsProcessing(false);
    }
  };

  return (
    <div className="w-full max-w-md mx-auto bg-white border border-slate-200 rounded-2xl shadow-sm overflow-hidden">
      <div className="p-4 border-b border-slate-100 bg-slate-50">
        <h3 className="text-xs font-bold uppercase tracking-wider text-slate-500">
          Secure Checkout
        </h3>
      </div>

      <form onSubmit={handleSubmit} className="p-6 space-y-6">
        <PaymentElement options={paymentElementOptions} />

        {errorMessage && (
          <div className="bg-red-50 border border-red-200 rounded-xl p-3 text-xs text-red-700">
            {errorMessage}
          </div>
        )}

        <button
          type="submit"
          disabled={isProcessing}
          className={`w-full text-white rounded-xl py-3 font-bold transition-colors ${
            isProcessing 
              ? "bg-slate-400 cursor-not-allowed" 
              : "bg-blue-600 hover:bg-blue-700"
          }`}
        >
          {isProcessing ? "Processing..." : "Pay Now"}
        </button>
      </form>
    </div>
  );
}