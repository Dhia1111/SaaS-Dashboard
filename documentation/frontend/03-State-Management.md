# 13 — State Management

## Redux Store

The application uses **Redux Toolkit** for global state management. The store is configured with a single slice for authentication:

```javascript
// store.js
import { configureStore } from '@reduxjs/toolkit';
import authReducer from './globalStates/AccessToken';

export const store = configureStore({
  reducer: {
    auth: authReducer,
  }
});
```

## Auth Slice (AccessToken.js)

The auth slice manages JWT access token state:

```javascript
const authSlice = createSlice({
  name: 'auth',
  initialState: {
    accessToken: null,
    // token metadata, decoding results
  },
  reducers: {
    setAccessToken: (state, action) => {
      state.accessToken = action.payload;
    },
    clearAccessToken: (state) => {
      state.accessToken = null;
    },
    // additional reducers for token parsing
  }
});
```

### Actions
- `setAccessToken(token)` — Store JWT token after successful auth
- `clearAccessToken()` — Clear token on logout
- Token decoding helpers to extract claims (roles, permissions)

## Provider Setup

The Redux `Provider` wraps the entire application in `main.jsx`:

```jsx
<Provider store={store}>
  <App />
</Provider>
```

## Local State

Component-level state is managed with React hooks (`useState`, `useEffect`) for:
- Form inputs and validation
- UI state (modals, toggles, loading indicators)
- Local data not requiring global access

## Data Flow

```
User Action → Component → Redux Dispatch / Axios Call
                                │
                    ┌───────────┴───────────┐
                    ▼                       ▼
              Redux Store             API Response
                    │                       │
                    ▼                       ▼
            All Components          Component State
            (via useSelector)       (via useState)
```
