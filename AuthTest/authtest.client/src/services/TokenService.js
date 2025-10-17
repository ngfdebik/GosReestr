import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'https://localhost:7229/api';

class TokenService {
  constructor() {
    this.accessToken = localStorage.getItem('accessToken');
    this.refreshToken = localStorage.getItem('refreshToken');
    this.isRefreshing = false;
    this.failedRequests = [];
  }

  // Сохранение токенов
  setTokens(accessToken, refreshToken) {
    this.accessToken = accessToken;
    this.refreshToken = refreshToken;
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
  }

  // Очистка токенов
  clearTokens() {
    this.accessToken = null;
    this.refreshToken = null;
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }

  // Проверка срока действия токена
  isTokenExpired(token) {
    if (!token) return true;
    
    try {
      // Используем упрощенный парсинг
      const payload = this.parseJwtSimple(token);
      if (!payload || !payload.exp) return true;
      
      const exp = payload.exp * 1000; // Convert to milliseconds
      // Добавляем запас в 1 минуту до фактического истечения
      return Date.now() >= (exp - 60000);
    } catch (error) {
      console.error('Error checking token expiration:', error);
      return true;
    }
  }

  parseJwtSimple(token) {
    if (!token) return null;
    
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = atob(base64);
      return JSON.parse(jsonPayload);
    } catch (error) {
      console.error('Error parsing JWT (simple):', error);
      return null;
    }
  }

  // Обновление токена
  async refreshAuthToken() {
    if (!this.refreshToken || !this.accessToken) {
      throw new Error('No tokens available for refresh');
    }

    if (this.isRefreshing) {
      // Если уже происходит обновление, ждем его завершения
      return new Promise((resolve, reject) => {
        this.failedRequests.push({ resolve, reject });
      });
    }

    this.isRefreshing = true;

    try {
      const response = await axios.post(`${API_URL}/auth/refresh`, {
        accessToken: this.accessToken,
        refreshToken: this.refreshToken
      });
      const accessToken = response.data.AccessToken;
      const refreshToken = response.data.RefreshToken;
      this.setTokens(accessToken, refreshToken);
      
      // Обрабатываем ожидающие запросы
      this.failedRequests.forEach(({ resolve }) => resolve(accessToken));
      this.failedRequests = [];

      return accessToken;
    } catch (error) {
      // Отклоняем все ожидающие запросы
      this.failedRequests.forEach(({ reject }) => reject(error));
      this.failedRequests = [];
      
      this.clearTokens();
      throw error;
    } finally {
      this.isRefreshing = false;
    }
  }

  // Получение валидного токена
  async getValidToken() {
    if (!this.accessToken || this.isTokenExpired(this.accessToken)) {
      if (this.refreshToken) {
        return await this.refreshAuthToken();
      } else {
        throw new Error('No valid tokens available');
      }
    }
    return this.accessToken;
  }

  // Выход из системы
  async logout() {
    try {
      if (this.refreshToken) {
        await axios.post(`${API_URL}/auth/logout`, {
          refreshToken: this.refreshToken
        });
      }
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      this.clearTokens();
    }
  }
}

export default new TokenService();
