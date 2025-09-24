import * as Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'

const app = Vue.createApp()
app.use(Vuex)


export default new Vuex.createStore({
  state: {
    status: '',
    token: localStorage.getItem('token') || '',
    user : ''
  },
  mutations: {
    auth_request(state){
        state.status = 'loading'
    },
    auth_success(state, token){
        state.status = 'success'
        state.token = token
    },
    auth_error(state){
        state.status = 'error'
    },
    logout(state){
        state.status = ''
        state.token = ''
    },
  },
  actions: {
    login({commit}, user){
        return new Promise((resolve, reject) => {
            commit('aith_request')
            axios.post('https://localhost:7229/api/auth/login', {
                login: user.value.login,
                password: user.value.password
            }).then(resp => {
                commit('auth_success', 'token')
                resolve(resp)
            }).catch(err => {
                commit('auth_error')
                reject(err)
            })
        })
    }
  },
  getters: {
    isLoggedIn: state => !!state.token,
    authStatus: state => state.status,
  }
})