import axios from "axios";
//import { store } from "../store";
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
  const res = await tenantAuth.post("/SignUp", data);
  return res;
};

// 🔹 LogIn
export const logIn = async (data) => {
  const res = await tenantAuth.post("/LogIn", data);
  return res.data.data.accessToken;
};

// 🔹 Refresh Token
export const refreshToken = async () => {
  const res = await tenantAuth.post("/RefreshToken");
  return res.data.data.accessToken;
};

// 🔹 LogOut
export const logOut = async () => {
  const res = await tenantAuth.post("/LogOut");
  return res.data;
};

// 🔹 Verify Email
export const verifyEmail = async (data) => {
  const res = await tenantAuth.patch(`/VerifyEmail`,  data );
  return res.data.data.accessToken;
};

// 🔹 Resend Code
export const resendCode = async (data) => {
  const res = await tenantAuth.post("/ReSendCode", data);
  return res.data;
};

