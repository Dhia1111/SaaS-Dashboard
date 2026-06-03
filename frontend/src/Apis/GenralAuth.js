import axios from "axios";
import { store } from "../store";
import{RetryPolicies,executeWithRetry} from './RetryPolicy/RetryPolicy.js'
const Auth = axios.create({
  baseURL: "http://localhost:7073/api/auth",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json"
  }
});
Auth.interceptors.request.use((config) => {
  const token = store.getState().auth.accessToken;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

export const refreshToken = async () => {

 try{
  
  const res =  await  executeWithRetry(() => Auth.post(`/refresh`), RetryPolicies.Critical);

  return { data: res.data.data.accessToken, message: "Token refreshed successfully.", success: true };
} catch (err) {
  return {
    success: false,
    message: err.response?.data?.message || "An error occurred.",
    data: null
  };
}
};
