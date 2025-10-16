<!-- src/views/Login.vue -->
<template>
  <div class="login-page">
      <form class="login-form" @submit.prevent="login"><!--</form> style="width: 25vw; border-radius: 20px">-->
        <img src="/svg/LogoText.svg" class="logo" alt="Логотип" />
        <div v-if="errors.form" class="error-label">{{ errors.form }}</div>
        <b class="form-name-label">Вход в систему</b>

        <label class="field-name-label">Логин</label>
        <input type="text" v-model="credentials.login"/>
        <div v-if="errors.login" class="error-label error-login">{{ errors.login }}</div>

        <label class="field-name-label">Пароль</label>
        <input type="password" v-model="credentials.password"/>
        <div v-if="errors.password" class="error-label error-password">{{ errors.password }}</div>

        <button type="submit" class="login-button">Вход</button>
      </form>
  </div>
</template>

<script setup>
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';
  import store from '../auth/store'
  import '@/css/site.css'

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

<style>
  .login-page{
    display: flex; 
    flex-direction :column; 
    min-width: 100vh;
    min-height: 100vh;
    justify-content: center; 
    align-items: center;
  }

  .login-form{
    width: 25vw;
    border-radius: 20px;
    border: 1px solid #dee2e6 !important;
    box-shadow: 0 .5rem 1rem rgba(0, 0, 0, .15) !important;
    box-sizing: border-box;
    display: flex;
    flex-direction: column;
    padding: 1rem;
    flex-wrap: wrap;
    gap: .5rem;
  }

  .logo{
    max-width: 100%;
  }

  .error-label{
    color: #dc3545;
  }

  .form-name-label{
    align-self: center;
  }

  .field-name-label{
    margin-left: .5rem;
  }

  .error-login{
    margin-bottom: .5rem;
  }

  .error-password{
    margin-bottom: 1.5rem;
  }

  .login-button{
    color: #fff;
    background-color: #0d6efd;
    border-color: #0d6efd;
    display: inline-block;
    font-weight: 400;
    line-height: 1.5;
    text-align: center;
    text-decoration: none;
    vertical-align: middle;
    cursor: pointer;
    user-select: none;
    border: 1px solid transparent;
    padding: .375rem .75rem;
    font-size: 1rem;
    border-radius: .25rem;
  }

  input{
    padding: .375rem .75rem;
    font-size: 1rem;
    font-weight: 400;
    line-height: 1.5;
    color: #212529;
    background-color: #fff;
    background-clip: padding-box;
    border: 1px solid #ced4da;
    border-radius: .25rem;
    box-sizing:border-box;
  }
  
</style>
