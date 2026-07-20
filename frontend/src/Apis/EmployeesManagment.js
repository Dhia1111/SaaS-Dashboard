import axios from "axios";
import { store } from "../store";
import { refreshToken } from "./GenralAuth.js";
import{RetryPolicies,executeWithRetry} from './RetryPolicy/RetryPolicy.js'
import { Redirect } from "./RedirectPolicy/RedirectPolicy.js";

var Redirecting=false;
const Employee = axios.create({
  baseURL: "http://localhost:7073/api/employee",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json"
  }
});
Employee.interceptors.request.use(async (config) => {

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

export const ListUsersAsync = async () => {
 try{
 const res = await executeWithRetry(() => Employee.get(`/`), RetryPolicies.ReadFast)    ;
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
    const res = await executeWithRetry(() => Employee.get(`/${id}`), RetryPolicies.ReadFast);
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
    const res = await executeWithRetry(() => Employee.put(`/${data.id}`, data), RetryPolicies.WriteNormal)    ;
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
    const res = await executeWithRetry(() => Employee.delete(`/${id}`), RetryPolicies.WriteNormal);

    return { data: res.data.data, message: "User deleted successfully.", success: true };
  } catch {
    return {
      success: false,
        message: "Failed to delete user.",
        data: null
      };
    }
};

export const CreatePlatformEmployeeAsync=async(data)=>{

  try{
    const res=await executeWithRetry(()=>Employee.post('/add',data),RetryPolicies.None);
    return {data:res.data.data,success:true,message:"Employee added seccessfuly"}
  }catch(err){
    console.log("error",err?.message);
        return {data:null,success:false,message:"Employee adding failed"}
  }

}

export const GetUserRolesAsync = async () => {
  try {
    const res = await executeWithRetry(() => Employee.get(`/roles`), RetryPolicies.None)    ;
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
    const res = await executeWithRetry(() => Employee.get(`/authorization-options`), RetryPolicies.None)    ;
    return { data: res.data.data, message: "Authorization options retrieved successfully.", success: true };
  } catch {
    return {
      success: false,
      message: "Failed to get authorization options.",
      data: null
    };
  }
};  
