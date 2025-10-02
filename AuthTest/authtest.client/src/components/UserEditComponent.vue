<template>
  <div>
    <div class="row">
      <button @click="goBack" class="ml-2 btn btn-link">Вернуться</button>
    </div>
    <div class="d-flex min-vh-98 justify-content-center align-items-center">
      <div class="justify-content-md-center">
        <form class="row border shadow p-3" @submit.prevent="editUser" style="width: 25vw; border-radius: 20px">
          <div v-if="Object.keys(errors).length > 0" class="text-danger mb-2">
            <div v-for="error in Object.values(errors)" :key="error">{{ error }}</div>
          </div>
          
          <label class="lead m-0 align-self-center mb-3">
            Редактирование пользователя {{ userData.login }}
          </label>
          
          <input type="hidden" v-model="userData.login" />

          <label class="mb-1" for="password">Новый пароль</label>
          <input type="password" id="password" v-model="userData.password" class="form-control" 
                 :class="{'is-invalid': errors.password}">
          <span v-if="errors.password" class="text-danger mb-2">{{ errors.password }}</span>

          <label class="mb-1 mt-2" for="confirmPassword">Подтвердите пароль</label>
          <input type="password" id="confirmPassword" v-model="userData.confirmPassword" class="form-control"
                 :class="{'is-invalid': errors.confirmPassword}">
          <span v-if="errors.confirmPassword" class="text-danger mb-2">{{ errors.confirmPassword }}</span>

          <label class="mb-1 mt-2" for="fullName">ФИО</label>
          <input type="text" id="fullName" v-model="userData.fullName" class="form-control mb-4">
          
          <button type="submit" class="btn btn-primary" :disabled="loading">
            {{ loading ? 'Загрузка...' : 'Изменить' }}
          </button>
          
          <div v-if="userEditStatusMessage" class="mt-2" 
               :class="userEditStatusMessage.includes('успеш') ? 'text-success' : 'text-danger'">
            {{ userEditStatusMessage }}
          </div>
        </form>
      </div>       
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
        
        if (response.ok) {
          const result = await response.json()
          userEditStatusMessage.value = result.message
          
          // Очистка полей пароля после успешного обновления
          if (result.success) {
            userData.password = ''
            userData.confirmPassword = ''
          }
        } else {
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