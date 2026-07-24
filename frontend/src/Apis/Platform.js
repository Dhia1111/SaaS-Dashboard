import axios from "axios";
import { executeWithRetry,RetryPolicies } from "./RetryPolicy/RetryPolicy";
import { store } from "../store";
import { refreshToken } from "./GenralAuth";
import { Redirect } from "./RedirectPolicy/RedirectPolicy";


var Redirecting=false
const PlatformSubscription = axios.create({
  baseURL: "http://localhost:7073/api/platform/subscription",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json"
  }
});
let refreshPromise = null;

PlatformSubscription.interceptors.request.use(async (config) => {

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


 export async function PlatformSubscriptionsAsync(){
    try{

        const res= await executeWithRetry(()=>PlatformSubscription.get("/subscriptions-options"),RetryPolicies.ReadFast);
         
        return{success: true,
            data: res.data.data,
            message: "Platform subscriptions retrieved successfully."}

    }
    catch{

        return{success:false,data:null,message:"Platform subscriptions retriving failed"}


    }
 }

 export async function GetPricingCyclesAsync() {
       try{

        const res= await executeWithRetry(()=>PlatformSubscription.get('/pricng-cycles'),RetryPolicies.ReadFast);
         
        return{success: true,
            data: res.data.data,
            message: "Platform pricing cycles retrieved successfully."}

    }
    catch{

        return{success:false,data:null,message:"Platform pricing cycles retriving failed"}


    }
 } 
  
 export async function CreateNewSubscriptionAsync(data) {
    
 try {
    const res = await executeWithRetry(() => PlatformSubscription.post("/subscribe",data ), RetryPolicies.ReadFast);
    return {data: res.data.data, message: res.data?.message|| "Creating subscription seccessfuly ", success: true} ;
  } catch {
    return {
      success: false,
      message: "Create subscription failed.",
      data: null  
    };
  }        

    }
  
    export async function SetDescoveryPlatform(data) {
    
 try {
    const res = await executeWithRetry(() => PlatformSubscription.post("/subscription-descovery",data ), RetryPolicies.ReadFast);
    return {data: res.data.data, message: res.data?.message|| "", success: true} ;
  } catch {
    return {
      success: false,
      message: "Set descovery platform failed.",
      data: null  
    };
  }        

    }


     export const TenantHaveUsedHisFreePlanAsync = async (id) => {
  try {
    const res = await executeWithRetry(() => PlatformSubscription.get("/tenantUsedFreeTry",{ params: { TenantId:id } } ), RetryPolicies.ReadFast);
    console.log("tenantUserFreeTry.data",res.data.data);
    return {data: Boolean(res.data.data), message: res.data.data? "Tenant used his try  .":"Tenant did not use his try", success: true} ;
  } catch {
    return {
      success: false,
      message: "Failed to check if the tenant used his free try .",
      data: null  
    };
  }
};

     export const GetTenantActiveSubscriptionAsync = async (id) => {
  try {
    const res = await executeWithRetry(() => PlatformSubscription.get("/active-subscription",{ params: { TenantId:id } } ), RetryPolicies.ReadFast);
    return {data: res.data.data, message: res.data?.message|| "Getting  subscription seccessfuly", success: true} ;
  } catch {
    return {
      success: false,
      message: "Failed to fetch subscription .",
      data: null  
    };
  }



};
    export const GetSubscriptionStatusAsync = async (id) => {
  try {
    const res = await executeWithRetry(() => PlatformSubscription.get("/subscription-status",{ params: { PaymentId:id } } ), RetryPolicies.ReadFast);
    return {data:Boolean(res.data.data), message: "fetching subscription status  successfuly", success: true} ;
  } catch {
    return {
      success: false,
      message: "Failed to fetch subscription status .",
      data: null  
    };
  }



};

    export const GetPaymentProviderList = async () => {
  try {
    const res = await executeWithRetry(() => PlatformSubscription.get("/payment-providers" ), RetryPolicies.ReadFast);
    return {data: res.data.data, message: res.data?.message|| "Fetching payment providers seccessfuly", success: true} ;
  } catch {
    return {
      success: false,
      message: "Failed to fetching payment providers  .",
      data: null  
    };
  }



};

   export const GetMarkettingPlatformsAsync = async () => {
  try {
    const res = await executeWithRetry(() => PlatformSubscription.get("/marketting-platforms" ), RetryPolicies.ReadFast);
    return {data: res.data.data, message: res.data?.message|| "Fetching marketting platforms seccessfuly", success: true} ;
  } catch {
    return {
      success: false,
      message: "Failed to fetching marketting platforms  .",
      data: null  
    };
  }



};






