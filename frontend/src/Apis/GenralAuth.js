import axios from "axios";
 import{RetryPolicies,executeWithRetry} from './RetryPolicy/RetryPolicy.js'
  const Auth = axios.create({
  baseURL: "http://localhost:7073/api/auth",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json"
  }
});
 

export const refreshToken = async () => {

 try{
  
  const res =  await  executeWithRetry(() => Auth.post(`/refreshToken`), RetryPolicies.Critical);
const accessToken = String(res.data.data);


    const payload = decodeJwt(accessToken);
    
    const userInfo = {
  userId: payload?.UserId ?? null,
  tenantId: payload?.TenantId ?? null,
  roles: payload?.Roles ?? null,
  authorization: payload?.Authorization ?? null,
  isAnEmployee: payload?.IsAnEmployee === "True",
  employeeRole: payload?.EmployeeRole ?? null,
  employeeAuthorizations: payload?.EmployeeAutherizations ?? null
};
 const tenantInfo = {
  tenantId: payload?.TenantId ?? payload?.sub ?? null,
  roles: payload?.Roles ?? null,
  isActive: payload?.IsActive === "True",
  isTheOwner: payload?.IsTheOwner === "True",
  platformRole: payload?.PlatformRole ?? null
};
     localStorage.removeItem("userInfo")
     localStorage.removeItem("tenantInfo")
    localStorage.setItem(
      "userInfo",
      JSON.stringify(userInfo)
    );
    localStorage.setItem(
      "tenantInfo",
      JSON.stringify(tenantInfo)
    );
  return { data:accessToken , message: "Token refreshed successfully.", success: true,status:200 };
} catch (err) {
   return {
    success: false,
    message: err.response?.data?.message || "An error occurred.",
    data: null,
    status:err?.status||err?.response?.status
  };
}
};


export const decodeJwt = (token) => {
  const base64Url = token.split(".")[1];
  const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");

  return JSON.parse(atob(base64));
};