import axios from "axios";
import { store } from "../store";
import { refreshUserToken } from "./UserAuth";

const User = axios.create({
  baseURL: "http://localhost:7073/api/user",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json"
  }
});

User.interceptors.request.use(async (config) => {

  let token = store.getState().auth.accessToken;

  if (!token) {
    try {
      token = await refreshUserToken();
    }
    catch {
      token = null;
    }
  }

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

export const ListUsersAsync = async () => {
 try{
 const res = await User.get(`/`);
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
    const res = await User.get(`/${id}`);
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
    const res = await User.put(`/${data.id}`, data);
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
    const res = await User.delete(`/${id}`);

  return { data: res.data.data, message: "User deleted successfully.", success: true };
        }
    catch {      return {
        success: false,
        message: "Failed to delete user.",
        data: null
      };
    }
};


export const GetUserRolesAsync = async () => {
  try {
    const res = await User.get(`/roles`);
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
    const res = await User.get(`/authorization-options`);
    return { data: res.data.data, message: "Authorization options retrieved successfully.", success: true };
  } catch {
    return {
      success: false,
      message: "Failed to get authorization options.",
      data: null
    };
  }
};  
