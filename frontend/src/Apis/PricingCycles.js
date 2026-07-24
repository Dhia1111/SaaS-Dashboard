import axios from "axios";
import { store } from "../store";
import{RetryPolicies,executeWithRetry} from './RetryPolicy/RetryPolicy.js'
import { refreshToken } from "./GenralAuth.js";
import {Redirect}from './RedirectPolicy/RedirectPolicy.js'
var Redirecting=false;
const pricingCycleApi = axios.create({
    baseURL: "http://localhost:7073/api/tenant/pricingcycle",
    withCredentials: true,
    headers: {
        "Content-Type": "application/json"
    }
});

let refreshPromise = null;

pricingCycleApi.interceptors.request.use(async (config) => {

    let token = store.getState().auth.accessToken;

    if (!token) {

        if (!refreshPromise) {
            refreshPromise = refreshToken();
        }

        const result = await refreshPromise;

// Refresh is complete; clear the shared promise.
        if (refreshPromise) {
            refreshPromise = null;
        }

        if (!result.success) {
            Redirect(result.status, Redirecting);
            Redirecting = true;
            return Promise.reject(result);
        }

        token = result.data;
    }

    config.headers.Authorization = `Bearer ${token}`;

    return config;
});


export async function GetPricingCyclesAsync() {
    try {
        const res = await executeWithRetry(() => pricingCycleApi.get("/GetPricingCycles"), RetryPolicies.ReadFast);

        return {
            success: true,
            data: res.data.data,
            message: "Pricing cycles retrieved successfully."
        };
    }
    catch (error) {
        return {
            success: false,
            data: [],
            message:
                error.response?.data?.message ??
                "Failed to retrieve pricing cycles."
        };
    }
}

export async function AddPricingCycleAsync(data) {
    try {
        const res = await executeWithRetry(() => pricingCycleApi.post("/AddPricingCycle", data), RetryPolicies.WriteNormal);

        return {
            success: true,
            data: res.data.data,
            message: "Pricing cycle added successfully."
        };
    }    
    catch (error) {
        return {
            success: false,
            data: null,
            message:
                error.response?.data?.message ??
                "Failed to add pricing cycle."
        };
    }

}

export async function UpdatePricingCycleAsync(data) {
    try {
        const res = await executeWithRetry(() => pricingCycleApi.put("/UpdatePricingCycle", data), RetryPolicies.WriteNormal);

        return {
            success: true,
            data: res.data.data,
            message: "Pricing cycle updated successfully."
        };
    }
    catch (error) {
        return {
            success: false,
            data: null,
            message:
                error.response?.data?.message ??
                "Failed to update pricing cycle."
        };
    }
}

export async function DeletePricingCycleAsync(id) {
    try {
        const res = await executeWithRetry(() => pricingCycleApi.delete(`/DeletePricingCycle/${id}`), RetryPolicies.WriteNormal);

        return {
            success: true,
            data: res.data.data,
            message: "Pricing cycle deleted successfully."
        };
    }
    catch (error) {
        return {
            success: false,
            data: null,
            message:
                error.response?.data?.message ??
                "Failed to delete pricing cycle."
        };
    }
}
