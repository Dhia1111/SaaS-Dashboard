import axios from "axios";
import { store } from "../store";
 import{RetryPolicies,executeWithRetry} from './RetryPolicy/RetryPolicy.js'
 import { refreshToken } from "./GenralAuth.js";
 import { Redirect } from "./RedirectPolicy/RedirectPolicy.js";
const UserAuth = axios.create({
  baseURL: "http://localhost:7073/api/user/auth",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json"
  }
});
let refreshPromise = null;
var Redirecting=false;
UserAuth.interceptors.request.use(async (config) => {

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

export const LoginUserAsync = async (data) => {

  try {

    const res = await executeWithRetry(() => UserAuth.post(`/login`, data), RetryPolicies.WriteNormal)    ;
    return { data: res.data.data , message: res.data.message||"Log in successful." ,success: true };

  } catch (err) {

    return {
      success: false,
      message: err.response?.data?.message || "An error occurred.",
      data: null
    };
  }
};

export const VerifyUserAsync = async (data) => {

  try {

    const res = await executeWithRetry(() => UserAuth.post(`/complete-registration`, data), RetryPolicies.WriteNormal)    ;
    return { data: res.data.data , message: res.data.message||"User verified successfully." ,success: true } ;

  } catch (err) {

    return {
      success: false,
      message: err.response?.data?.message || "An error occurred.",
      data: null
    };
  }
};



export const LogoutUserAsync = async () => {

 try{ const res = await executeWithRetry(() => UserAuth.post(`/logout`), RetryPolicies.WriteNormal)    ;

  return { data: res.data.data , message: res.data.message||"Log out successful." ,success: true }  ;
} catch {
  return {
    success: false,
    message: "Failed to logout.",
    data: null
  };
}
};

     

export const SendInvitationAsync = async (data) => {
  try{
const res = await executeWithRetry(() => UserAuth.post(`/invitations`, data), RetryPolicies.WriteNormal);
  return { data: res.data.data , message: res.data.message||"Invitation sent successfully." ,success: true } ;
  }catch{
return {
  success: false,
  message: "Failed to send invitation.",
  data: null
};
  }
};
