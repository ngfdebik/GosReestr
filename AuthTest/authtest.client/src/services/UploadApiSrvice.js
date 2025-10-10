// import api from './TokenApi'; // ← изменили импорт
// import store from '@/auth/store'
import axios from 'axios';

const access_token = localStorage.getItem('accessToken');

const uploadApi = axios.create({
    baseURL: 'https://localhost:7229/api',
    timeout: 60000,
    headers: {
        'Content-Type': 'multipart/form-data',
        'Authorization' : `Bearer ${access_token}`
    },
})
export default {
  uploadFile(data) {
    return uploadApi.post(`/home/upload`, data).then(response => response.data); // ← baseURL уже /api, логин не нужен в URL
  },
}