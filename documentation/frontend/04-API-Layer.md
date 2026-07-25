# 14 — API Layer & Axios Interceptors

## Overview

All API communication uses **Axios** with a centralized configuration pattern. Each API module creates its own Axios instance pointing at the backend base URL (`http://localhost:7073/api/`).

## API Module Pattern

```javascript
// Apis/Platform.js (example)
import axios from 'axios';

const API_URL = 'http://localhost:7073/api/Platform';

const api = axios.create({
  baseURL: API_URL,
});

// Request interceptor — inject JWT token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor — handle 401 and refresh
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      // Attempt token refresh via GenralAuth
      const refreshed = await refreshToken();
      if (refreshed) {
        // Retry original request
        error.config.headers.Authorization = `Bearer ${getNewToken()}`;
        return axios(error.config);
      }
      // Redirect to login
    }
    return Promise.reject(error);
  }
);

export const getSubscriptionOptions = () => api.get('/subscription-options');
export const subscribe = (data) => api.post('/subscribe', data);
// ...
```

## API Modules

| Module | Base URL | Endpoints |
|---|---|---|
| `GenralAuth.js` | Shared | Token refresh, JWT decode, auth utilities |
| `Platform.js` | `/api/Platform` | Subscription options, pricing, checkout |
| `Subscriotions.js` | `/api/Subscriptions` | Plan CRUD |
| `Tenant.js` | `/api/Tenant` | Tenant info, client subscriptions |
| `tenantAuth.js` | `/api/TenantAuth` | Tenant login, register, verify |
| `UserAuth.js` | `/api/UserAuth` | User login, register, invite, verify |
| `Users.js` | `/api/User` | User CRUD, roles, permissions |
| `TenantPermissions.js` | `/api/Permissions` | Permission CRUD |
| `PricingCycles.js` | `/api/TenantPricingCycle` | Pricing cycle CRUD |
| `BusinessAnalyses.js` | `/api/BusinessAnalyses` | Analytics data |
| `EmployeesManagment.js` | `/api/EmployeesManagment` | Employee management |

## Interceptor Features

### Request Interceptor
- Automatically attaches `Authorization: Bearer <token>` header
- Reads token from `localStorage` or Redux store

### Response Interceptor
- **401 handling**: Attempts silent token refresh before redirecting to login
- **Retry logic**: Replays the original request after successful refresh
- **Error normalization**: Transforms API errors into consistent format

### Retry Policy
The `RetryPolicy/` directory contains logic for:
- Automatic retry on network failures
- Exponential backoff
- Maximum retry attempts

### Redirect Policy
The `RedirectPolicy/` directory handles:
- Redirect to login on authentication failure
- Redirect to appropriate dashboard after login
- Post-login redirect preservation
