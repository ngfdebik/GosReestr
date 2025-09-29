// services/api.js
import axios from 'axios';
import authService from './authService';
import router from '@/router';

const API_URL = 'http://localhost:7229/api';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor
api.interceptors.request.use(
  async (config) => {
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
      // Перенаправляем на логин если не удалось получить токен
      if (!config.url.includes('/auth/login')) {
        router.push('/login');
      }
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Если ошибка 401 и это не запрос на обновление токена
    if (error.response?.status === 401 && 
        !originalRequest.url.includes('/auth/refresh') &&
        !originalRequest._retry) {
      
      originalRequest._retry = true;

      try {
        // Пытаемся обновить токен
        await authService.refreshAuthToken();
        
        // Повторяем оригинальный запрос с новым токеном
        const token = authService.accessToken;
        originalRequest.headers.Authorization = `Bearer ${token}`;
        return api(originalRequest);
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

export default api;