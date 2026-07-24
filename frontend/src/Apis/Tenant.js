import axios from "axios";
import { store } from "../store";
import { refreshToken } from "./GenralAuth.js";
import{RetryPolicies,executeWithRetry} from './RetryPolicy/RetryPolicy.js'
import { Redirect } from "./RedirectPolicy/RedirectPolicy.js";

var Redirecting=false;
const Tenant = axios.create({
  baseURL: "http://localhost:7073/api/tenat",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json"
  }
});
Tenant.interceptors.request.use(async (config) => {

  let token = store.getState().auth.accessToken;

  if (!token) {
      const res = await refreshToken();
      if(res.success){
        token=res.data;
      }
      else{
        token=null;
        Redirect(res.status,Redirecting);
        Redirecting=true;



        
      }

    
  }

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  

  return config;
});

export const GetTenatInfoAsync = async (id) => {
  try {
    const res = await executeWithRetry(() => Tenant.get(`/tenant`,{params:{TenantId:id}}), RetryPolicies.ReadFast);
    return { data: res.data.data, message: "User found successfully.", success: true };
  } catch {
    return {
      success: false,
      message: "Failed to find user.",
      data: null
    };
  }
};

export const ClientSubscriptionAsync = async () => {
  try {
    const res = await executeWithRetry(() => Tenant.get(`/clients-subscriptions`), RetryPolicies.ReadFast);
    return { data: res.data.data, message: "User found successfully.", success: true };
  } catch {
    return {
      success: false,
      message: "Failed to find user.",
      data: null
    };
  }
};