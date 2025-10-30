// import api from './TokenApi'; // ← изменили импорт
// import store from '@/auth/store'
import axios from 'axios';
import authService from './TokenService';
import router from '@/router/Routers';

const uploadApi = axios.create({
    baseURL: 'https://localhost:7229/api',
    // timeout: 60000,
    headers: {
        'Content-Type': 'multipart/form-data',
    },
})

uploadApi.interceptors.request.use(
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

export default {
  uploadFile(data) {
    return uploadApi.post(`/home/upload`, data).then(response => response.data); // ← baseURL уже /api, логин не нужен в URL
  },
}