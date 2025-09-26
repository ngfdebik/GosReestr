import * as Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'

const app = Vue.createApp()
app.use(Vuex)


export default new Vuex.createStore({
  state: {
    status: '',
    token: localStorage.getItem('token') || '',
    user : '',
    isAdmin : false,
  },
  mutations: {
    auth_request(state){
        state.status = 'loading'
    },
    auth_success(state, {token, isAdmin, login}){
        state.status = 'success'
        state.token = token
        state.isAdmin = isAdmin
        state.user = login
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
    login({commit}, user){
        return new Promise((resolve, reject) => {
            commit('auth_request')
            axios.post('https://localhost:7229/api/auth/login', {
                login: user.login,
                password: user.password
            }).then(resp => {
                const token = 'token'
                const isAdmin = resp.data.isAdmin
                const login = resp.data.login
                commit('auth_success', {token, isAdmin, login})
                resolve(resp)
            }).catch(err => {
                commit('auth_error')
                reject(err)
            })
        })
    },
    logout({ commit }) {
      return new Promise((resolve) => {
        commit('logout')
        //localStorage.removeItem('token')
        //delete axios.defaults.headers.common['Authorization']
        resolve()
      })
    }
  },
  getters: {
    isLoggedIn: state => !!state.token,
    authStatus: state => state.status,
    isAdmin: state => state.isAdmin,
    login: state => state.user,
  }
})
