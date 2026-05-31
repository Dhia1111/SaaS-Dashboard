import axios from "axios";
import { store } from "../store";
const TenantPermissionAuth = axios.create({
  baseURL: "http://localhost:7073/api/tenant/permission",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json"
  }
});
TenantPermissionAuth.interceptors.request.use((config) => {
  const token = store.getState().auth.accessToken;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

export const TenantPermissionsListAsync = async () => {
  try { 
    const res = await TenantPermissionAuth.get(`/GetPermissions`);
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
        const res = await TenantPermissionAuth.post(`/AddPermission`, data);
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
        const res = await TenantPermissionAuth.put(`/EditPermission/${id}`, data);
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
        const res = await TenantPermissionAuth.delete(`/DeletePermission/${id}`);
        return { data: res.data.data , message: res.data.message||"Permission deleted successfully." ,success: true } ;
    } catch (err) {
        return {
            success: false,
            message: err.response?.data?.message || "An error occurred.",
            data: null
        };
    }
};

