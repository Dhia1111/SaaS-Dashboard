
import axios from "axios";
import { executeWithRetry,RetryPolicies } from "./RetryPolicy/RetryPolicy";

const BusinessAnalyses = axios.create({
  baseURL: "http://localhost:7073/api/dashboard/business-analyses",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json"
  }
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
