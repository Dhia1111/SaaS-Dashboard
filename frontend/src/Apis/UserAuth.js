import axios from "axios";
import { store } from "../store";
import { refreshToken } from "./GenralAuth";
import{RetryPolicies,executeWithRetry} from './RetryPolicy/RetryPolicy.js'
const UserAuth = axios.create({
  baseURL: "http://localhost:7073/api/user/auth",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json"
  }
});
UserAuth.interceptors.request.use((config) => {
  const token = store.getState().auth.accessToken;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

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

export const refreshUserToken = async () => {

 const res = await refreshToken();

 return res;

}

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
