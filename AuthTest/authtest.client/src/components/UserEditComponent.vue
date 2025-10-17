<style>
  .edit-page {
    height: 100vh;
    width: 100vw;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
  }

  .edit-component {
    display: flex;
    justify-content: center;
    align-items: center;
    box-sizing: border-box;
    flex-direction: column;
  }

  .edit-form {
    width: 25vw;
    border-radius: 20px;
    padding: 1.5rem;
    border: 1px solid #dee2e6;
    box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
    display: flex;
    flex-direction: column; /* ← важно: вертикальная колонка */
    gap: 0.75rem; /* ← равномерный отступ между всеми элементами */
  }

  .form-name-label {
    font-size: 1.25rem;
    font-weight: 500;
    text-align: center;
    margin: 0; /* убираем возможные отступы */
    padding: 0;
    display: block; /* ← обязательно block, а не inline-block! */
  }

  .error-text {
    color: #dc3545;
    font-size: 0.875rem;
  }

  .success-text {
    color: #198754;
  }

  .success-label {
    text-align: center;
    margin-top: 0.5rem;
  }

  .submit-button {
    color: #fff;
    background-color: #0d6efd;
    border-color: #0d6efd;
    font-weight: 400;
    line-height: 1.5;
    text-align: center;
    text-decoration: none;
    cursor: pointer;
    user-select: none;
    border: 1px solid transparent;
    padding: 0.5rem;
    font-size: 1rem;
    border-radius: 0.25rem;
    width: 100%;
  }

  /* Унифицируем стили для всех лейблов */
  label {
    font-weight: 500;
    margin: 0;
    padding: 0;
    display: block;
  }

  input {
    display: block;
    width: 100%;
    padding: 0.375rem 0.75rem;
    font-size: 1rem;
    font-weight: 400;
    line-height: 1.5;
    color: #212529;
    background-color: #fff;
    background-clip: padding-box;
    border: 1px solid #ced4da;
    appearance: none;
    border-radius: 0.25rem;
    box-sizing: border-box;
  }

  /* Кнопка "Вернуться" */
  .back-button {
    position: absolute;
    top: 1rem;
    left: 1rem;
    padding: 0.25rem 0.5rem;
    background: #f8f9fa;
    border: 1px solid #dee2e6;
    border-radius: 0.25rem;
    cursor: pointer;
  }
</style>

<template>
  <div class="edit-page">
    <button @click="goBack" class="back-button">Вернуться</button>

    <div class="edit-component">
      <form class="edit-form" @submit.prevent="editUser">
        <!-- Ошибки -->
        <div v-if="Object.keys(errors).length > 0" class="error-text">
          <div v-for="error in Object.values(errors)" :key="error">{{ error }}</div>
        </div>

        <!-- Заголовок -->
        <label class="form-name-label">
          Редактирование пользователя {{ userData.login }}
        </label>

        <!-- Скрытый логин -->
        <input type="hidden" v-model="userData.login" />

        <!-- Новый пароль -->
        <label for="password">Новый пароль</label>
        <input type="password"
               id="password"
               v-model="userData.password"
               :class="{ 'is-invalid': errors.password }" />
        <span v-if="errors.password" class="error-text">{{ errors.password }}</span>

        <!-- Подтверждение пароля -->
        <label for="confirmPassword">Подтвердите пароль</label>
        <input type="password"
               id="confirmPassword"
               v-model="userData.confirmPassword"
               :class="{ 'is-invalid': errors.confirmPassword }" />
        <span v-if="errors.confirmPassword" class="error-text">{{ errors.confirmPassword }}</span>

        <!-- ФИО -->
        <label for="fullName">ФИО</label>
        <input type="text" id="fullName" v-model="userData.fullName" />

        <!-- Кнопка -->
        <button type="submit" class="submit-button" :disabled="loading">
          {{ loading ? 'Загрузка...' : 'Изменить' }}
        </button>

        <!-- Сообщение об успехе/ошибке -->
        <div v-if="userEditStatusMessage"
             class="success-label"
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
