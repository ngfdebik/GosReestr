// src/services/UserApiService.js
import api from './TokenApi'; // ← изменили импорт
import store from '@/auth/store'

export default {
  getCurrent() {
    return api.get(`/user/current/${store.getters.login}`).then(response => response.data); // ← baseURL уже /api, логин не нужен в URL
  },
  edit(userData) {
    return api.put('/user/edit', userData).then(response => response.data);//.then(response => response.data)
  }
}
