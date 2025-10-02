<!-- src/views/Login.vue -->
<template>
  <div class="d-flex flex-column min-vh-100 justify-content-center align-items-center">
    <div class="justify-content-md-center">
      <form class="row border shadow p-3" @submit.prevent="login" style="width: 25vw; border-radius: 20px">
        <img src="/svg/LogoText.svg" class="m-auto" alt="Логотип" />
        <div v-if="errors.form" class="text-danger">{{ errors.form }}</div>
        <b class="p-2 align-self-center">Вход в систему</b>

        <label class="mb-1">Логин</label>
        <input type="text" v-model="credentials.login" class="form-control" />
        <div v-if="errors.login" class="text-danger mb-2">{{ errors.login }}</div>

        <label class="mb-1">Пароль</label>
        <input type="password" v-model="credentials.password" class="form-control" />
        <div v-if="errors.password" class="text-danger mb-4">{{ errors.password }}</div>

        <button type="submit" class="btn btn-primary">Вход</button>
      </form>
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';
  import store from '../auth/store'
  //import axios from 'axios';
  import '../assets/lib/bootstrap/dist/css/bootstrap.min.css'
  import '../assets/css/site.css'

  const credentials = ref({
    login: '',
    password: ''
  });

  const errors = ref({
    login: '',
    password: '',
    form: ''
  });

  const router = useRouter();

  const login = async () =>
  {
    // Сброс ошибок
    errors.value = { login: '', password: '', form: '' };

    let login = credentials.value.login
    let password = credentials.value.password
    store.dispatch('login', { login, password })
    .then(resp => 
      router.push(resp.redirectTo)
    ).catch(err =>{
      if(err){
        if (err.errors) {
          errors.value.login = err.errors.Login || '';
          errors.value.password = err.errors.Password || '';
        }
        if (err.message) {
          errors.value.form = err.message;
        }
      }else {
        errors.value.form = 'Ошибка соединения с сервером';
      }
    })
  };
</script>

<style scoped>
  /* Можно вынести в отдельный CSS файл */
</style>
