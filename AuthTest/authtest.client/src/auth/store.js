import Vuex from 'vuex'
import api from '../services/TokenApi';
import TokenService from '@/services/TokenService';

// const app = Vue.createApp()
// app.use(Vuex)


export default new Vuex.createStore({
  state: {
    status: '',
    accessToken: localStorage.getItem('accessToken') || '',
    refreshToken: localStorage.getItem('refreshToken') || '',
    user : localStorage.getItem('login') || '',
    isAdmin : localStorage.getItem('isAdmin') || '',
    isAuthenticated: false,
    isLoading: false,
    globalLoading: false,
  },
  mutations: {
    SET_GLOBAL_LOADING(state, status) {
      state.globalLoading = status;
    },
    SET_USER(state, user) {
      state.user = user;
    },
    SET_AUTHENTICATED(state, status) {
      state.isAuthenticated = status;
    },
    SET_LOADING(state, status) {
      state.isLoading = status;
    },
    auth_request(state){
        state.status = 'loading'
    },
    auth_success(state, {access_token, refresh_token, isAdmin, login}){
        state.status = 'success'
        state.accessToken = access_token
        state.refreshToken = refresh_token
        state.isAdmin = isAdmin
        localStorage.setItem('isAdmin', isAdmin)
        state.user = login
        localStorage.setItem('login', login)
    },
    auth_error(state){
        state.status = 'error'
    },
    logout(state){
        state.status = ''
        state.token = ''
        state.user= ''
        state.isAdmin = false
    }
  },
  actions: {
    async login({ commit }, credentials) {
      commit('SET_LOADING', true);
      try {
        const response = await api.post('/auth/login', credentials);
        const { access_token, refresh_token } = response.data;
        
        TokenService.setTokens(access_token, refresh_token);
        commit('auth_success', response.data)
        //commit('SET_USER', user);
        commit('SET_AUTHENTICATED', true);
        
        return response.data;
      } catch (error) {
        commit('SET_AUTHENTICATED', false);
        commit('SET_USER', null);
        throw error;
      } finally {
        commit('SET_LOADING', false);
      }
    },

    async refreshToken({ commit }) {
      try {
        const token = await TokenService.refreshAuthToken();
        // Можно обновить информацию о пользователе
        // const userResponse = await api.get('/auth/me');
        // commit('SET_USER', userResponse.data);
        return token;
      } catch (error) {
        commit('SET_AUTHENTICATED', false);
        commit('SET_USER', null);
        throw error;
      }
    },

    // async logout({ commit }) {
    //   commit('SET_LOADING', true);
    //   try {
    //     await TokenService.logout();
    //   } catch (error) {
    //     console.error('Logout error:', error);
    //   } finally {
    //     commit('SET_USER', null);
    //     commit('SET_AUTHENTICATED', false);
    //     commit('SET_LOADING', false);
    //   }
    // },

    async checkAuth({ commit }) {
      try {
        const token = await TokenService.getValidToken();
        if (token) {
          // Получаем информацию о пользователе
          // const response = await api.get('/auth/me');
          // commit('SET_USER', response.data);
          commit('SET_AUTHENTICATED', true);
          return true;
        }
      } catch (error) {
        console.error('Auth check failed:', error);
        commit('SET_AUTHENTICATED', false);
        commit('SET_USER', null);
      }
      return false;
    },

    initializeAuth({ dispatch }) {
      // Проверяем наличие токенов при запуске приложения
      const accessToken = localStorage.getItem('accessToken');
      const refreshToken = localStorage.getItem('refreshToken');
      
      if (accessToken && refreshToken) {
        return dispatch('checkAuth');
      }
      return Promise.resolve(false);
    },
    logout({ commit }) {
      return new Promise((resolve) => {
        commit('logout')
        commit('SET_AUTHENTICATED', false);
        TokenService.clearTokens();
        commit('SET_USER', null);
        //localStorage.removeItem('token')
        //delete axios.defaults.headers.common['Authorization']
        resolve()
      })
    }
  },
  getters: {
    isLoggedIn: state => !!state.accessToken,
    authStatus: state => state.status,
    isAdmin: state => state.isAdmin,
    login: state => state.user,
    user: state => state.user,
    isAuthenticated: state => state.isAuthenticated,
    isLoading: state => state.isLoading,
    isGlobalLoading: state => state.globalLoading,
  }
})
