import axios from "axios";
//import { store } from "../store";
import {refreshToken as GenralRefreshToken } from "./GenralAuth";
 const tenantAuth = axios.create({
  baseURL: "http://localhost:7073/api/auth/tenant",
  withCredentials: true, 
  headers: {
    "Content-Type": "application/json"
  }
});
 // the method to inject the token
 
/*tenantAuth.interceptors.request.use((config) => {
  const token = store.getState().auth.accessToken;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});*/

// 🔹 SignUp
 export const signUp = async (data) => {
 try{
 const res = await tenantAuth.post("/SignUp", data);
  return { data: res.data.data , message: res.data.message||"Sign up successful." ,success: true };
} catch  {
  return {
    success: false,
    message: "Failed to sign up.",
    data: null
  };
}
};

// 🔹 LogIn
export const logIn = async (data) => {
  try{
  const res = await tenantAuth.post("/LogIn", data);
  return { data: res.data.data , message: res.data.message||"Log in successful." ,success: true };
} catch {
  return {
    success: false,
    message: "Failed to log in.",
    data: null
  };
}
};

// 🔹 Refresh Token
export const refreshToken = async () => {
  return await GenralRefreshToken();
};

// 🔹 LogOut
export const logOut = async () => {
  try {
    const res = await tenantAuth.post("/LogOut");
    return { data: res.data.data , message: res.data.message||"Log out successful." ,success: true }  ;
  } catch {
    return {
      success: false,
      message: "Failed to logout.",
      data: null
    };
  }
};

// 🔹 Verify Email
export const verifyEmail = async (data) => {
  try {
    const res = await tenantAuth.patch(`/VerifyEmail`,  data );
    return { data: res.data.data , message: res.data.message||"Email verified successfully." ,success: true } ;
  } catch {
    return {
      success: false,
      message: "Failed to verify email.",
      data: null
    };
  }
};

// 🔹 Resend Code
export const resendCode = async (data) => {
  try {
    const res = await tenantAuth.post("/ReSendCode", data);
    return { data: res.data.data , message: res.data.message||"Code resent successfully." ,success: true } ;
  } catch {
    return {
      success: false,
      message: "Failed to resend code.",
      data: null
    };
  }
};
 export const IsNameUsed = async (tenantName) => {
  try {
    const res = await tenantAuth.get("/is-name-unique", { params: { tenantName:tenantName } });
    return {data: Boolean.parse(res.data.data), message: "Tenant name availability checked successfully.", success: true} ;
  } catch {
    return {
      success: false,
      message: "Failed to check tenant name availability.",
      data: null  
    };
  }
};
