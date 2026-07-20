import React, { useState } from "react";
import { AddPricingCycleAsync } from "../../Apis/PricingCycles.js";

export default function AddNewPricingCycles() {
  const [formData, setFormData] = useState({
    cycleName: "",
    period: 1,            // Defaulting to 1 for better UX
    periodUnit: "Months", // Matches C# DTO pluralized string examples
  });
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Unified input handler handling type casting for integers
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === "period" ? parseInt(value, 10) || 0 : value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    // Basic client-side validation
    if (!formData.cycleName.trim()) {
      alert("Please enter a cycle name.");
      return;
    }
    if (formData.period <= 0) {
      alert("Duration must be a positive number.");
      return;
    }

    setIsSubmitting(false);
    setIsSubmitting(true);

    try {
      const response = await AddPricingCycleAsync(formData);
      if (!response.success) {
        alert(response.message || "Something went wrong.");
      } else {
        alert("Pricing Cycle Added Successfully");
        // Reset form to defaults
        setFormData({ cycleName: "", period: 1, periodUnit: "Months" });
      }
    } catch  {
      alert("An unexpected error occurred while saving.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className=" max-w rounded-xl h-full border border-slate-200 bg-white p-6 shadow-sm animate-in fade-in duration-200 ">
      <div>
        <h2 className="text-lg font-semibold text-slate-800">Add New Pricing Cycle</h2>
        <p className="text-sm text-slate-500">Define a recurring billing interval for tenant subscriptions.</p>
      </div>

      <form onSubmit={handleSubmit} className="mt-6 space-y-4">
        {/* Cycle Name Field */}
        <div className="flex flex-col space-y-1.5">
          <label htmlFor="cycleName" className="text-xs font-medium text-slate-600">
            Cycle Name
          </label>
          <input
            id="cycleName"
            name="cycleName"
            type="text"
            placeholder="e.g., Quarterly, Standard Monthly"
            value={formData.cycleName}
            onChange={handleInputChange}
            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-800 placeholder-slate-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-100 transition"
            required
          />
        </div>

        {/* Duration and Period Unit Inline Grid */}
        <div className="grid grid-cols-2 gap-4">
          <div className="flex flex-col space-y-1.5">
            <label htmlFor="period" className="text-xs font-medium text-slate-600">
              Duration
            </label>
            <input
              id="period"
              name="period"
              type="number"
              min="1"
              placeholder="e.g., 3"
              value={formData.period || ""}
              onChange={handleInputChange}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-800 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-100 transition"
              required
            />
          </div>

          <div className="flex flex-col space-y-1.5">
            <label htmlFor="periodUnit" className="text-xs font-medium text-slate-600">
              Unit
            </label>
            <select
              id="periodUnit"
              name="periodUnit"
              value={formData.periodUnit}
              onChange={handleInputChange}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-800 bg-white focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-100 transition"
            >
               <option value="Hours">Hours</option>
              <option value="Days">Days</option>
              <option value="Weeks">Weeks</option>
              <option value="Months">Months</option>
              <option value="Years">Years</option>
            </select>
          </div>
        </div>

        {/* Submit Action Button */}
        <button
          type="submit"
          disabled={isSubmitting}
          className="mt-2 w-full h-full py-4 rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-medium text-white shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-60 disabled:cursor-not-allowed transition"
        >
          {isSubmitting ? "Saving Pricing Cycle..." : "Save Pricing Cycle"}
        </button>
      </form>
    </div>
  );
}