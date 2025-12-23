<!--Версия два-->>
<template>
  <header class="page-header">
    <div class="header-content">
      <!-- Логотип -->
      <div class="logo-section">
        <img src="/svg/Logo.svg" class="logo-image" />
      </div>

      <!-- Кнопки фильтрации -->
      <div class="button-section">
        <button class="nav-button" @click="$emit('load-all')">
          <img src="/svg/All.svg" title="Все" class="button-icon" />
          <p class="button-text">Все</p>
        </button>
      </div>
      <div class="button-section">
        <button class="nav-button" @click="$emit('load-ip')">
          <img src="/svg/IP.svg" title="ИП" class="button-icon" />
          <p class="button-text">ИП</p>
        </button>
      </div>
      <div class="button-section">
        <button class="nav-button" @click="$emit('load-ul')">
          <img src="/svg/UL.svg" title="ЮЛ" class="button-icon" />
          <p class="button-text">ЮЛ</p>
        </button>
      </div>

      <!-- Кнопки экспорта -->
      <!-- Экспорт в Excel -->
      <div class="button-section">
        <button @click="exportToExcel"
                class="nav-button"
                :disabled="getUploadExcel">
          <img src="/svg/Export.svg" title="Экспорт в Excel" class="button-icon" />
          <div v-if="getUploadExcel" class="export-spinner"></div>
          <p v-else class="button-text">Экспорт в Excel</p>
        </button>
      </div>

      <!-- Экспорт в Word -->
      <div class="button-section">
        <button @click="exportToWord"
                class="nav-button"
                :disabled="getUploadDoc">
          <img src="/svg/Export.svg" title="Экспорт в Word" class="button-icon" />
          <div v-if="getUploadDoc" class="export-spinner"></div>
          <p v-else class="button-text">Экспорт в Word</p>
        </button>
      </div>

      <!-- Блок с пользователем -->
      <!-- Обновленный блок user-section -->
      <div class="user-section">
        <div v-if="isLoggedIn" class="user-container">
          <!-- Блок с кнопками действий -->
          <div class="user-actions">
            <div v-if="isAdmin" class="admin-actions">
              <label for="file-upload" class="custom-file-upload">
                Загрузка файлов
              </label>
              <input id="file-upload"
                     type="file"
                     ref="fileInput"
                     class="file-input"
                     @change="upload"
                     accept=".xml,.zip" />
            </div>
            <button class="action-button clear-button"
                    @click="$emit('clear-filters')"
                    type="button">
              Сброс фильтров
            </button>
          </div>

          <!-- Блок с информацией пользователя -->
          <div class="user-info-wrapper">
            <div class="user-main">
              <div class="user-name-container">
                <p class="user-name">{{ userFullName || 'Пользователь' }}</p>
                <button class="edit-button" @click="toggleEditing" title="Редактировать профиль">
                  <img src="/svg/Pen.svg" class="edit-icon" />
                </button>
              </div>

              <!-- Меню редактирования -->
              <div v-if="isEditingOpen" class="edit-menu-wrapper">
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

              <!-- Аватар с выпадающим меню -->
              <div class="avatar-wrapper" @click="toggleMenu">
                <div class="avatar-circle">
                  <p class="avatar-initial">{{ userInitial }}</p>
                </div>
                <div v-if="isMenuOpen" class="user-menu">
                  <div v-if="isAdmin" class="menu-item">
                    <button class="menu-link" @click="goToManage">Администрирование</button>
                  </div>
                  <div class="menu-item">
                    <button class="menu-link" @click="logout">Выйти</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </header>
</template>

<script>
  import userApi from '@/services/UserApiService';
  import { useRouter } from 'vue-router';
  import store from '@/auth/store';
  import fileUpload from '@/components/FileUploadZone.vue';

  export default {
    name: 'Header',
    props: {
      isAdmin: Boolean,
      isLoggedIn: Boolean,
    },
    emits: ['load-all', 'load-ip', 'load-ul', 'export-excel', 'export-doc', 'clear-filters'],
    data() {
      return {
        isUploadInProgress: false,
        userFullName: '',
        isMenuOpen: false,
        isEditingOpen: false,
        isUploadOpen: false,
        router: useRouter(),
        // Состояния экспорта
        isExportingExcel: false,
        isExportingWord: false,
        loading: false,
        errors: {},
        userData: {
          login: '',
          password: '',
          confirmPassword: '',
          fullName: ''
        },
        userEditStatusMessage: '',
      };
    },
    computed: {
      userInitial() {
        return this.userFullName ? this.userFullName[0]?.toUpperCase() || '?' : '?';
      },
      getUploadExcel() {
        return this.isExportingExcel;
      },
      getUploadDoc() {
        return this.isExportingWord;
      },
      // getUploadButtonText() {
      //   if (this.isUploadInProgress) {
      //     return 'Идет загрузка...';
      //   }
      //   return this.isUploadOpen ? 'Закрыть загрузку' : 'Загрузка файлов';
      // }
    },
    async created() {
      if (this.isLoggedIn) {
        await this.loadCurrentUser();
      }
    },
    methods: {
      async editUser() {
        if (!this.validateForm())
          return;

        this.loading = true;
        this.userEditStatusMessage = '';

        try {
          const response = await userApi.edit(this.userData)

          if (response.success) {
            this.userEditStatusMessage = response.message

            // Очистка полей пароля после успешного обновления
            this.userData.password = ''
            this.userData.confirmPassword = ''
          }
          else {
            const error = await response.json()
            this.userEditStatusMessage = error.message || 'Ошибка изменения пользователя'
          }
        } catch (error) {
          this.userEditStatusMessage = 'Ошибка соединения с сервером'
          console.error('Ошибка:', error)
        } finally {
          this.loading = false
        }
      },
      validateForm() {
        Object.keys(this.errors).forEach(key => delete this.errors[key])

        let isValid = true

        if (this.userData.password && this.userData.password !== this.userData.confirmPassword) {
          this.errors.confirmPassword = 'Пароли не совпадают'
          isValid = false
        }

        if (this.userData.password && this.userData.password.length < 6) {
          this.errors.password = 'Пароль должен содержать не менее 6 символов'
          isValid = false
        }

        return isValid;
      },
      async loadCurrentUser() {
        try {
          const response = await userApi.getCurrent();
          this.userFullName = response.FullName;
          this.userData.login = response.Login;
          this.userData.fullName = response.FullName;
        } catch (error) {
          console.error('Ошибка загрузки данных пользователя:', error);
        }
      },
      toggleMenu() {
        this.isMenuOpen = !this.isMenuOpen;
        if (this.isMenuOpen) {
          this.isUploadOpen = false;
        }
      },
      toggleEditing() {
        this.isEditingOpen = !this.isEditingOpen;
        if (this.isEditingOpen) {
          this.isUploadOpen = false;
        }
      },
      goToEdit() {
        this.router.push('/UserEdit');
      },
      goToManage() {
        this.router.push('/Manage');
      },
      logout() {
        store.dispatch('logout').then(() => {
          this.router.push({ name: 'Home' });
        });
      },
      toggleUploadZone() {
        // this.isUploadOpen = !this.isUploadOpen;
        // if (this.isUploadOpen) {
        //   this.isMenuOpen = false;
        // }
      },
      handleUploadComplete(uploadData) {
        console.log('Загрузка завершена в Header:', uploadData);
        this.isUploadInProgress = false;
        if (uploadData.success) {
          setTimeout(() => {
            this.isUploadOpen = false;
          }, 1000);
        }
      },
      handleClickOutside(event) {
        const editButton = this.$el.querySelector('.edit-button');
        if (editButton && editButton.contains(event.target)) {
          return;
        }
        const userBlock = this.$el.querySelector('.user-info');
        const uploadBlock = this.$el.querySelector('.upload-container');
        const uploadButton = this.$el.querySelector('.upload-button');

        const isEditingBlock = this.$el.querySelector('.user-menu');

        const isUploadButtonClick = uploadButton && uploadButton.contains(event.target);

        if (userBlock && !userBlock.contains(event.target)) {
          this.isMenuOpen = false;
        }
        if (isEditingBlock && !isEditingBlock.contains(event.target)) {
          this.isEditingOpen = false;
        }

        if (uploadBlock && !uploadBlock.contains(event.target) && !isUploadButtonClick) {
          this.isUploadOpen = false;
        }
      },

      handleFileSelect(event) {
        const fileList = event.target.files;
        Array.from(fileList).forEach((file) => {
          if (file && !this.isUploading) {
            if (this.isValidFile(file)) {
              this.uploadFile(file);
            } else {
              this.showMessage('Неверный формат файла. Только .xml или .zip.', 'alert-danger');
            }
            this.resetFileInput();
          }
        })
      },
      
      isValidFile(file) {
        if (!file) return false;
        const validExtensions = ['.xml', '.zip'];
        const fileName = file.name.toLowerCase();
        return validExtensions.some(ext => fileName.endsWith(ext));
      },

      resetFileInput() {
        if (this.$refs.fileInput && this.$refs.fileInput.value) {
          this.$refs.fileInput.value = '';
        }
      },

      // === Методы экспорта с управлением состоянием ===
      async exportToExcel() {
        this.isExportingExcel = true;
        try {
          await this.$emit('export-excel');
        } finally {
          this.isExportingExcel = false;
        }
      },
      async exportToWord() {
        this.isExportingWord = true;
        try {
          await this.$emit('export-doc');
        } finally {
          this.isExportingWord = false;
        }
      },
    },
    mounted() {
      document.addEventListener('click', this.handleClickOutside);
    },
    beforeUnmount() {
      document.removeEventListener('click', this.handleClickOutside);
    }
  };
</script>

<style scoped>
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

  .export-spinner {
    width: 16px;
    height: 16px;
    border: 2px solid transparent;
    border-top-color: #0d6efd;
    border-radius: 50%;
    animation: spin 1s linear infinite;
    margin: 0 auto;
  }

  @keyframes spin {
    to {
      transform: rotate(360deg);
    }
  }

  .page-header {
    background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
    border-bottom: 1px solid rgba(0, 0, 0, 0.08);
    height: 70px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.03);
    position: sticky;
    top: 0;
    z-index: 1000;
  }

  .header-content {
    max-width: 1400px;
    margin: 0 auto;
    height: 100%;
    padding: 0 24px;
    display: flex;
    align-items: center;
    gap: 32px;
  }

  /* Логотип */
  .logo-section {
    flex-shrink: 0;
  }

  input[type="file"] {
    display: none;
  }

  .custom-file-upload {
    padding: 6px 12px;
    background: #10b981;
    color: white;
    border-radius: 6px;
    font-size: 12px;
    font-weight: 500;
    border: none;
    cursor: pointer;
    transition: all 0.2s ease;
  }

    .custom-file-upload:hover {
      background: #059669;
      transform: translateY(-1px);
    }

  .logo-image {
    height: 36px;
    transition: transform 0.3s ease;
  }

    .logo-image:hover {
      transform: scale(1.05);
    }

  /* Основные кнопки навигации */
  .button-section {
    position: relative;
  }

    .button-section::after {
      content: '';
      position: absolute;
      right: -16px;
      top: 50%;
      transform: translateY(-50%);
      height: 24px;
      width: 1px;
      background: rgba(0, 0, 0, 0.1);
    }

    .button-section:last-child::after {
      display: none;
    }

  .nav-button {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 4px;
    padding: 8px 12px;
    border: none;
    background: transparent;
    border-radius: 8px;
    cursor: pointer;
    transition: all 0.2s ease;
    min-width: 80px;
  }

    .nav-button:hover:not(:disabled) {
      background: rgba(13, 110, 253, 0.08);
      transform: translateY(-1px);
    }

    .nav-button:active:not(:disabled) {
      transform: translateY(0);
    }

    .nav-button:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

  .button-icon {
    height: 24px;
    width: 24px;
    object-fit: contain;
  }

  .button-text {
    font-size: 12px;
    font-weight: 500;
    color: #495057;
    margin: 0;
    line-height: 1.2;
  }

  /* User Section */
  .user-section {
    margin-left: auto;
    display: flex;
    align-items: center;
    gap: 16px;
  }
  .user-container {
    display: flex;
    align-items: center;
    gap: 20px;
  }
  .admin-actions {
    display: flex;
    align-items: center;
  }
  .user-info-wrapper {
    position: relative;
  }
  .user-info {
    display: flex;
    flex-direction: row;
    align-items: flex-end;
  }

  .user-main {
    display: flex;
    align-items: center;
    gap: 16px;
    background: rgba(255, 255, 255, 0.7);
    padding: 8px 16px;
    border-radius: 12px;
    border: 1px solid rgba(0, 0, 0, 0.08);
    backdrop-filter: blur(10px);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  }
  .user-name-container {
    display: flex;
    align-items: center;
    gap: 8px;
    padding-right: 12px;
    border-right: 1px solid rgba(0, 0, 0, 0.1);
  }
  .user-name {
    font-size: 14px;
    font-weight: 600;
    color: #2d3748;
    margin: 0;
    max-width: 180px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    letter-spacing: 0.01em;
    background: linear-gradient(135deg, #176ce3 0%, #1255b4 100%);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
  }

  .edit-button {
    background: rgba(13, 110, 253, 0.1);
    border: 1px solid rgba(13, 110, 253, 0.2);
    padding: 4px 6px;
    border-radius: 6px;
    cursor: pointer;
    transition: all 0.2s ease;
    display: flex;
    align-items: center;
    justify-content: center;
    width: 28px;
    height: 28px;
  }

    .edit-button:hover {
      background: rgba(13, 110, 253, 0.15);
      border-color: rgba(13, 110, 253, 0.3);
      transform: translateY(-1px);
    }

  .edit-icon {
    height: 14px;
    width: 14px;
    opacity: 0.8;
  }

  .avatar-wrapper {
    position: relative;
  }

  .avatar-image {
    display: none;
  }
  .avatar-circle {
    width: 36px;
    height: 36px;
    border-radius: 50%;
    background: linear-gradient(135deg, #176ce3 0%, #1255b4 100%);
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    border: 2px solid white;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
    transition: all 0.3s ease;
  }

    .avatar-circle:hover {
      transform: scale(1.05);
      box-shadow: 0 4px 12px rgba(79, 70, 229, 0.3);
    }
  .avatar-initial {
    color: white;
    font-weight: 600;
    font-size: 14px;
    margin: 0;
  }
  /* Позиционирование выпадающих меню */
  .edit-menu-wrapper {
    position: absolute;
    top: calc(100% + 10px);
    right: 0;
    z-index: 1002;
    min-width: 280px;
  }
  .user-menu {
    position: absolute;
    top: calc(100% + 8px);
    right: 0;
    background: white;
    border-radius: 8px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.12);
    border: 1px solid #e2e8f0;
    min-width: 200px;
    z-index: 1001;
    overflow: hidden;
  }

  .menu-item {
    border-bottom: 1px solid #f1f5f9;
  }

    .menu-item:last-child {
      border-bottom: none;
    }

  .menu-link {
    display: block;
    width: 100%;
    text-align: left;
    padding: 12px 16px;
    background: transparent;
    border: none;
    color: #4a5568;
    font-size: 14px;
    cursor: pointer;
    transition: background 0.2s ease;
  }

    .menu-link:hover {
      background: #f7fafc;
      color: #2d3748;
    }

  /* User Actions */
  .user-actions {
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 4px 0;
  }

  .action-button {
    padding: 6px 12px;
    border-radius: 6px;
    border: 1px solid;
    font-size: 12px;
    font-weight: 500;
    cursor: pointer;
    transition: all 0.2s ease;
  }

  .upload-button:hover {
    background-color: #5c636a;
    border-color: #565e64;
  }

  .clear-button {
    background: #dc2626;
    color: white;
    border-color: #dc2626;
  }

    .clear-button:hover {
      background: #b91c1c;
      transform: translateY(-1px);
    }

  /* Upload Container */
  .upload-container {
    width: 320px;
    right: 0;
    top: calc(100% + 8px);
    border-radius: 12px;
    border: 1px solid #e2e8f0;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
    background: white;
    z-index: 1001;
  }

  .upload-wrapper {
    position: relative;
    z-index: 1001;
  }

  /* Responsive Design */
  /* Адаптивность */
  @media (max-width: 1200px) {
    .header-content {
      gap: 20px;
      padding: 0 16px;
    }

    .nav-button {
      min-width: 70px;
      padding: 6px 8px;
    }

    .button-text {
      font-size: 11px;
    }
  }

  @media (max-width: 768px) {
    .page-header {
      height: 60px;
    }

    .header-content {
      gap: 12px;
      padding: 0 12px;
    }
    .user-container {
      gap: 12px;
    }
    .user-main {
      padding: 6px 12px;
      gap: 12px;
    }
    .logo-image {
      height: 28px;
    }

    .button-section {
      display: none;
    }

    .user-name {
      max-width: 120px;
      font-size: 13px;
    }
    .user-name-container {
      padding-right: 8px;
    }
    .user-actions {
      position: fixed;
      bottom: 16px;
      right: 16px;
      flex-direction: column;
    }

    .action-button, .custom-file-upload {
      padding: 8px 16px;
      font-size: 13px;
    }
  }
</style>
