import { store } from "../store";
import axios from "axios";
import { refreshToken } from "./tenantAuth";
  const User = axios.create({
  baseURL: "http://localhost:7073/api/user",
  withCredentials: true, 
  headers: {
    "Content-Type": "application/json"
  }
});
 // the method to inject the token
 
User.interceptors.request.use(async (config)  => {
  var token = store.getState().auth.accessToken;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
   }
 else{
    token= await refreshToken();
     if(!token){
       window.location.href = "/login";
    }
    else {
          config.headers.Authorization = `Bearer ${token}`;
     }
 }
 

  return config;
});



export const AddUser = async (data) => {
  const res = await User.post(`/.`, data );
  return res.data.data;
};

export const ListAsync = async () => {
  const res = await User.get(`/list` );
  return res.data.data;
};

export const FindUserAsync = async (id) => {
  const res = await User.get(`/${id}`  );
  return res.data.data;
};


export const UpdateUser = async (data) => {
  const res = await User.patch(`/${data.id}`, data );
  return res.data.data;
};

export const DeleteUser = async (id) => {
  const res = await User.delete(`/${id}`);
  return res.data.data;
};
