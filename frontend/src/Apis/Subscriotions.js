
import axios from "axios";
import { store } from "../store";
import{RetryPolicies,executeWithRetry} from './RetryPolicy/RetryPolicy.js'

const subscription = axios.create({
  baseURL: "http://localhost:7073/api/subscription",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json"
  }
});
subscription.interceptors.request.use(async (config) => {

  let token = store.getState().auth.accessToken;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});


export async function AddNewTenantPlanAsync(data) {
    try {
        const res = await executeWithRetry(() => subscription.post(`/add`, data), RetryPolicies.WriteNormal)      ;
        return { data: res.data.data, message: "Subscription plan added successfully.", success: true };
    } catch (error) {
        return {
            success: false,
            message: error.response?.data?.message || "Failed to add subscription plan.",
            data: null
        };
    }
}