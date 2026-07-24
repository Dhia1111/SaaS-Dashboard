import axios from "axios";
import { store } from "../store";
import { refreshToken } from "./GenralAuth.js";
import{RetryPolicies,executeWithRetry} from './RetryPolicy/RetryPolicy.js'
import { Redirect } from "./RedirectPolicy/RedirectPolicy.js";

var Redirecting=false;
const User = axios.create({
  baseURL: "http://localhost:7073/api/user",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json"
  }
});
let refreshPromise = null;

User.interceptors.request.use(async (config) => {

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


export const ListUsersAsync = async () => {
 try{
 const res = await executeWithRetry(() => User.get(`/`), RetryPolicies.ReadFast)    ;
  return { data: res.data.data, message: "Users listed successfully.", success: true };
} catch {
  return {
    success: false,
    message: "Failed to list users.",
    data: null
  };
}
};

export const FindUserAsync = async (id) => {
  try {
    const res = await executeWithRetry(() => User.get(`/${id}`), RetryPolicies.ReadFast);
    return { data: res.data.data, message: "User found successfully.", success: true };
  } catch {
    return {
      success: false,
      message: "Failed to find user.",
      data: null
    };
  }
};

export const UpdateUserAsync = async (data) => {
  try {
    const res = await executeWithRetry(() => User.put(`/${data.id}`, data), RetryPolicies.WriteNormal)    ;
    return { data: res.data.data, message: "User updated successfully.", success: true };
  } catch {
    return {
      success: false,
      message: "Failed to update user.",
      data: null
    };
  }
};

export const DeleteUserAsync = async (id) => {
  try {
    const res = await executeWithRetry(() => User.delete(`/${id}`), RetryPolicies.WriteNormal);

    return { data: res.data.data, message: "User deleted successfully.", success: true };
  } catch {
    return {
      success: false,
        message: "Failed to delete user.",
        data: null
      };
    }
};


export const GetUserRolesAsync = async () => {
  try {
    const res = await executeWithRetry(() => User.get(`/roles`), RetryPolicies.None)    ;
    return { data: res.data.data, message: "User roles retrieved successfully.", success: true };
  } catch {
    return {
      success: false,
      message: "Failed to get user roles.",
      data: null
    };
  }
};

export const GetAuthorizationOptionsAsync = async () => {
  try {
    const res = await executeWithRetry(() => User.get(`/authorization-options`), RetryPolicies.None)    ;
    return { data: res.data.data, message: "Authorization options retrieved successfully.", success: true };
  } catch {
    return {
      success: false,
      message: "Failed to get authorization options.",
      data: null
    };
  }
};  


export const GetUserInfoAsnc = async (id) => {
  try {
    const res = await executeWithRetry(() => User.get(`/user`,{params:{UserId:id}}), RetryPolicies.None)    ;
    return { data: res.data.data, message: "Authorization options retrieved successfully.", success: true };
  } catch {
    return {
      success: false,
      message: "Failed to get authorization options.",
      data: null
    };
  }
};  

