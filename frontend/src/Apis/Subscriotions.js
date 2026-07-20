
import axios from "axios";
import { store } from "../store";
import{RetryPolicies,executeWithRetry} from './RetryPolicy/RetryPolicy.js'
import {refreshToken}from './GenralAuth.js'
import {Redirect}from './RedirectPolicy/RedirectPolicy.js'
var Redirecting=false;
 
const subscription = axios.create({
  baseURL: "http://localhost:7073/api/tenant/subscription",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json"
  }
});
subscription.interceptors.request.use(async (config) => {
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

export async function AddNewTenantSubscriptionAsync(data) {
    try {
        const res = await executeWithRetry(() => subscription.post(`/AddNewSubscription`, data), RetryPolicies.WriteNormal)      ;
        return { data: res.data.data, message: "Subscription plan added successfully.", success: true };
    } catch (error) {
        return {
            success: false,
            message: error.response?.data?.message || "Failed to add subscription plan.",
            data: null
        };
    }
}
export async function UpdateTenantSubscriptionAsync(data) {
    try {
        const res = await executeWithRetry(() => subscription.put(`/UpdateTenantSubscription`, data), RetryPolicies.WriteNormal)      ;
        return { data: res.data.data, message: "Subscription plan added successfully.", success: true };
    } catch (error) {
        return {
            success: false,
            message: error.response?.data?.message || "Failed to Update subscription plan.",
            data: null
        };
    }
}


export async function GetSubscriptionListAsync() {

    try{
        //const res= await executeWithRetry(()=>subscription.get('/GetSubscriptionList'),RetryPolicies.ReadFast);
       const res=await subscription.get('/GetSubscriptionList')
        return { data: res.data.data, message: "Subscription list fetched successfully.", success: true };

    }
    catch(error){
        return {
            success:false,
            data:null,
            message:error.message||"Faild To fetch List "
        }

    }
    

}

export async function DeleteTenantSubscriptionAsync(id){

    try{
        const res=await executeWithRetry(()=>subscription.delete('/Delete'/{id}),RetryPolicies.WriteNormal);
        return {success:true,data:res.data.data,message:"Delete subscription sucssesfuly"}

    }
    catch(error){
                return {success:false,data:null,message:error.message||"Delete subscription faild"}

    }



}

export async function GetActiveSubscriptionAsync(id) {

    try{
        //const res= await executeWithRetry(()=>subscription.get('/GetSubscriptionList'),RetryPolicies.ReadFast);
       const res=await subscription.get('/active-subscription',{params:{TenantId:id}})
        return { data: res.data.data, message: "Subscription list fetched successfully.", success: true };

    }
    catch(error){
        return {
            success:false,
            data:null,
            message:error.message||"Faild To fetch List "
        }

    }
    

}


export async function HavetenantUsedFreeTry  (id) {

    try{
        //const res= await executeWithRetry(()=>subscription.get('/GetSubscriptionList'),RetryPolicies.ReadFast);
       const res=await subscription.get('/tenantUsedFreeTry',{params:{TenantId:id}})
        return { data: res.data.data, message: "Subscription list fetched successfully.", success: true };

    }
    catch(error){
        return {
            success:false,
            data:null,
            message:error.message||"Faild To fetch List "
        }

    }
    

}
