import axios from "axios";
import { store } from "../store";
import{RetryPolicies,executeWithRetry} from './RetryPolicy/RetryPolicy.js'
import { refreshToken } from "./GenralAuth.js";
import {Redirect} from './RedirectPolicy/RedirectPolicy.js'
var Redirecting=false;
const TenantPermissionAuth = axios.create({
  baseURL: "http://localhost:7073/api/tenant/permission",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json"
  }
});


let refreshPromise = null;

TenantPermissionAuth.interceptors.request.use(async (config) => {

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


export const TenantPermissionsListAsync = async () => {
  try { 
    const res = await executeWithRetry(() => TenantPermissionAuth.get(`/GetPermissions`), RetryPolicies.ReadFast)   ;
    return { data: res.data.data , message: res.data.message||"Permissions fetched successfully." ,success: true } ;
  } catch (err) {
    return {
      success: false,
        message: err.response?.data?.message || "An error occurred.",
        data: null
    };
  } 
};


export const AddTenantPermissionAsync = async (data) => {   
    try {
        const res = await executeWithRetry(() => TenantPermissionAuth.post(`/AddPermission`, data), RetryPolicies.WriteNormal);
        return { data: res.data.data , message: res.data.message||"Permission added successfully." ,success: true } ;
    } catch (err) {
        return {
            success: false,
            message: err.response?.data?.message || "An error occurred.",
            data: null
        };
    }
};

export const EditTenantPermissionAsync = async (id, data) => {
    try {
        const res = await executeWithRetry(() => TenantPermissionAuth.put(`/EditPermission/${id}`, data), RetryPolicies.WriteNormal)    ;
        return { data: res.data.data , message: res.data.message||"Permission updated successfully." ,success: true } ;
    } catch (err) {
        return {
            success: false,
            message: err.response?.data?.message || "An error occurred.",

            data: null
        };
    }
};

export const DeleteTenantPermissionAsync = async (id) => {
    try {
        const res = await executeWithRetry(() => TenantPermissionAuth.delete(`/DeletePermission/${id}`), RetryPolicies.WriteNormal)    ;
        return { data: res.data.data , message: res.data.message||"Permission deleted successfully." ,success: true } ;
    } catch (err) {
        return {
            success: false,
            message: err.response?.data?.message || "An error occurred.",
            data: null
        };
    }
};

