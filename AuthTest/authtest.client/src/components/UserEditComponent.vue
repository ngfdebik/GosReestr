<style>
.edit-page{
    height: 100vh;
    width: 100vw;
    align-items: center;
    display: flex;
    justify-content: center;
    flex-direction: column;
}

.edit-component{
    align-items: center !important;
    justify-content: center !important;
    display: flex !important;
    box-sizing: border-box;
    flex-direction: column;
}

.edit-form{
    width: 25vw;
    border-radius: 20px;
    padding: 1rem;
    border: 1px solid #dee2e6;
    box-shadow: 0 .5rem 1rem rgba(0, 0, 0, .15);
    flex-wrap: wrap;
    display: flex;
    gap: .5rem;
}

.form-name-label{
    align-self: center;
    font-size: 1.25rem;
    font-weight: 300;
    display: inline-block;
    box-sizing: border-box;
    padding-left: .5rem;
}

.error-text{
    color: #dc3545;
}

.success-text{
    color: #198754
}

.success-label{
    margin-top: 1rem;
}

.submit-button{
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
    width: 100%;
}

label{
    padding-left: .5rem;
}

span{
    margin-bottom: .25rem;
    padding-left: .5rem;
}

input{
    display: block;
    width: 100%;
    padding: .375rem .75rem;
    font-size: 1rem;
    font-weight: 400;
    line-height: 1.5;
    color: #212529;
    background-color: #fff;
    background-clip: padding-box;
    border: 1px solid #ced4da;
    appearance: none;
    border-radius: .25rem;
}

</style>

<template>
  <div class="edit-page">
    <div class="">
      <button @click="goBack" class="">Вернуться</button>
    </div>
    <div class="edit-component">
        <form class="edit-form" @submit.prevent="editUser">
          <div v-if="Object.keys(errors).length > 0" class="text-danger mb-2">
            <div v-for="error in Object.values(errors)" :key="error">{{ error }}</div>
          </div>
          
          <label class="form-name-label">
            Редактирование пользователя {{ userData.login }}
          </label>
          
          <input type="hidden" v-model="userData.login" />

          <label class="" for="password">Новый пароль</label>
          <input type="password" id="password" v-model="userData.password" class="" 
                 :class="{'is-invalid': errors.password}">
          <span v-if="errors.password" class="error-text">{{ errors.password }}</span>

          <label class="" for="confirmPassword">Подтвердите пароль</label>
          <input type="password" id="confirmPassword" v-model="userData.confirmPassword" class=""
                 :class="{'is-invalid': errors.confirmPassword}">
          <span v-if="errors.confirmPassword" class="error-text">{{ errors.confirmPassword }}</span>

          <label class="" for="fullName">ФИО</label>
          <input type="text" id="fullName" v-model="userData.fullName" class="">
          
          <button type="submit" class="submit-button" :disabled="loading">
            {{ loading ? 'Загрузка...' : 'Изменить' }}
          </button>
          
          <div v-if="userEditStatusMessage" class="success-label" 
               :class="userEditStatusMessage.includes('успеш') ? 'success-text' : 'error-text'">
            {{ userEditStatusMessage }}
          </div>
        </form>   
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import UserApiService from '@/services/UserApiService'

export default {
  name: 'UserEdit',
  created(){
      this.getCurrentUser()
    },
  setup() {
    const router = useRouter()
    const loading = ref(false)
    
    const userData = reactive({
      login: '',
      password: '',
      confirmPassword: '',
      fullName: ''
    })
    
    const errors = reactive({})
    const userEditStatusMessage = ref('')

    // Получение данных текущего пользователя
    const getCurrentUser = async () => {
        UserApiService.getCurrent()
        .then(resp =>{
            userData.login = resp.Login
            userData.fullName = resp.FullName
        })
        .catch(error =>{
            console.error('Ошибка загрузки данных пользователя:', error)
        })
    }

    // Валидация формы
    const validateForm = () => {
      // Очистка предыдущих ошибок
      Object.keys(errors).forEach(key => delete errors[key])
      
      let isValid = true
      
      if (userData.password && userData.password !== userData.confirmPassword) {
        errors.confirmPassword = 'Пароли не совпадают'
        isValid = false
      }
      
      if (userData.password && userData.password.length < 6) {
        errors.password = 'Пароль должен содержать не менее 6 символов'
        isValid = false
      }
      
      return isValid
    }

    // Редактирование пользователя
    const editUser = async () => {
      if (!validateForm()) return
      
      loading.value = true
      userEditStatusMessage.value = ''
      
      try {
        const response = await UserApiService.edit(userData)
        
        if (response.success) {
            userEditStatusMessage.value = response.message
          
            // Очистка полей пароля после успешного обновления
            userData.password = ''
            userData.confirmPassword = ''
        }
        else {
          const error = await response.json()
          userEditStatusMessage.value = error.message || 'Ошибка изменения пользователя'
        }
      } catch (error) {
        userEditStatusMessage.value = 'Ошибка соединения с сервером'
        console.error('Ошибка:', error)
      } finally {
        loading.value = false
      }
    }

    // Возврат назад
    const goBack = () => {
      router.go(-1)
    }

    // Загрузка данных при монтировании компонента
    

    return {
      userData,
      errors,
      userEditStatusMessage,
      loading,
      editUser,
      goBack,
      getCurrentUser
    }
  }
}
</script>