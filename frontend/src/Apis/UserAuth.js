import axios from "axios";
import { store } from "../store";
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

    const res = await UserAuth.post(`/login`, data);
    return res;

  } catch (err) {

    return {
      state: 0,
      message: err.response?.data?.message || "An error occurred."
    };
  }
};

export const VerifyUserAsync = async (data) => {

  try {

    const res = await UserAuth.post(`/complete-registration`, data);
    return res;

  } catch (err) {

    return {
      state: 0,
      message: err.response?.data?.message || "An error occurred."
    };
  }
};

export const refreshUserToken = async () => {

  const res = await UserAuth.post(`/refresh`);

  return res.data.data.accessToken;
};

export const LogoutUserAsync = async () => {

  const res = await UserAuth.post(`/logout`);

  return res.data;
};

export const SendInvitationAsync = async (data) => {
  try{
const res = await UserAuth.post(`/invitations`, data);
  return res;
  }catch{
return {
  state: 0,
  message: "Failed to send invitation."
};
  }
};
