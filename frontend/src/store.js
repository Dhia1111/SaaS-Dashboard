// store.js
import { configureStore } from '@reduxjs/toolkit';
import authReducer from './globalStates/AccessToken';

export const store = configureStore({
  reducer: {
    auth: authReducer,
  }
});