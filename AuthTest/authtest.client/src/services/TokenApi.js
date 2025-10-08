// src/services/TokenApi.js
import axios from 'axios';
import store from '@/auth/store';
import authService from './TokenService';
import router from '@/router/Routers';

const apiClient = axios.create({
  baseURL: 'https://localhost:7229/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
});

let delayTimer = null;

const setGlobalLoading = (isLoading) => {
  if (isLoading) {
    delayTimer = setTimeout(() => {
      store.commit('SET_GLOBAL_LOADING', true);
    }, 300);
  } else {
    if (delayTimer) {
      clearTimeout(delayTimer);
      delayTimer = null;
    }
    store.commit('SET_GLOBAL_LOADING', false);
  }
};

// Request interceptor — с авторизацией И лоадером
apiClient.interceptors.request.use(
  async (config) => {
    setGlobalLoading(true);

    // Не добавляем токен для эндпоинтов аутентификации
    if (config.url.includes('/auth/') && !config.url.includes('/auth/refresh')) {
      return config;
    }

    try {
      const token = await authService.getValidToken();
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
    } catch (error) {
      console.error('Error getting valid token:', error);
      if (!config.url.includes('/auth/login')) {
        router.push('/login');
      }
    }

    return config;
  },
  (error) => {
    setGlobalLoading(false);
    return Promise.reject(error);
  }
);

// Response interceptor — с обработкой 401 И лоадером
apiClient.interceptors.response.use(
  (response) => {
    setGlobalLoading(false);
    return response;
  },
  async (error) => {
    setGlobalLoading(false);

    const originalRequest = error.config;

    if (error.response?.status === 401 &&
      !originalRequest.url.includes('/auth/refresh') &&
      !originalRequest._retry) {

      originalRequest._retry = true;

      try {
        await authService.refreshAuthToken();
        const token = authService.accessToken;
        originalRequest.headers.Authorization = `Bearer ${token}`;
        return apiClient(originalRequest);
      } catch (refreshError) {
        console.error('Token refresh failed:', refreshError);
        authService.clearTokens();
        router.push('/login');
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default apiClient;
