import { createApp } from 'vue'
import App from './App.vue'
import router from './router/Routers'
import store from './auth/store'

createApp(App).use(router).use(store)
  .mount('#app')
