
import axios from "axios";
import { executeWithRetry,RetryPolicies } from "./RetryPolicy/RetryPolicy";
import { store } from "../store";
import { refreshToken } from "./GenralAuth.js";
 import { Redirect } from "./RedirectPolicy/RedirectPolicy.js";
var Redirecting=false;


const BusinessAnalyses = axios.create({
  baseURL: "http://localhost:7073/api/dashboard/business-analyses",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json"
  }
});
let refreshPromise = null;

BusinessAnalyses.interceptors.request.use(async (config) => {

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

export async function MarkettingPlaataformsLeadsContributions () {

    try{
         const res= await executeWithRetry(()=>BusinessAnalyses.get("/platform-leads-counts"),RetryPolicies.ReadFast);
                 
                return{success: true,
                    data: res.data.data,
                    message: "Marketting platforms retrieved successfully."}

    }
    catch{
        return {success:false,data:null,message:"Faild to fetch marketting platforms  "}
    }

}



export async function MarkettingPlaataformsConverssionContributions () {

    try{
         const res= await executeWithRetry(()=>BusinessAnalyses.get("/platform-coverssion-counts"),RetryPolicies.ReadFast);
                 
                return{success: true,
                    data: res.data.data,
                    message: "Marketting platforms retrieved successfully."}

    }
    catch{
        return {success:false,data:null,message:"Faild to fetch marketting platforms  "}
    }

}
