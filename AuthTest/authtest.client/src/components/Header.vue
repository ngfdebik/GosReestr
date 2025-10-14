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
            <div class="button-section">
                <button @click="$emit('export-excel')" class="nav-button">
                    <img src="/svg/Export.svg" title="Экспорт в Excel" class="button-icon" />
                    <p class="button-text">Экспорт в Excel</p>
                </button>
            </div>
            <div class="button-section">
                <button @click="$emit('export-doc')" class="nav-button">
                    <img src="/svg/Export.svg" title="Экспорт в Word" class="button-icon" />
                    <p class="button-text">Экспорт в Word</p>
                </button>
            </div>

            <!-- Блок с пользователем -->
            <div class="user-section">
                <div v-if="isLoggedIn" class="user-info">
                    <!-- Имя пользователя и иконка редактирования -->
                    <div class="user-main">
                        <p class="user-name">{{ userFullName || 'Пользователь' }}</p>
                        <button class="edit-button"
                                @click="goToEdit">
                            <img src="/svg/Pen.svg" class="edit-icon" />
                        </button>
                        <div class="avatar-wrapper" @click="toggleMenu">
                            <img class="avatar-image" src="/svg/Avatar.svg" />
                            <p class="avatar-initial">{{ userInitial }}</p>
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

                    <!--НОВАЯ КНОПКА: Загрузка файлов (только для админа) -->
                    <div class="user-actions">
                        <button v-if="isAdmin"
                                class="action-button upload-button"
                                @click="toggleUploadZone"
                                type="button">
                            Загрузка файлов
                        </button>
                        <div v-if="isUploadOpen && isAdmin" class="upload-container">
                            <div class="upload-wrapper">
                                <FileUploadZone @upload-complete="handleUploadComplete" />
                            </div>
                        </div>
                        <button class="action-button clear-button"
                                @click="$emit('clear-filters')"
                                type="button">
                            Сбросить фильтры
                        </button>
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
  import FileUploadZone from '@/components/FileUploadZone.vue';

  export default {
    name: 'Header',
    components: {
      FileUploadZone
    },
    props: {
      isAdmin: Boolean,
      isLoggedIn: Boolean
    },
    emits: ['load-all', 'load-ip', 'load-ul', 'export-excel', 'export-doc', 'clear-filters'],
    data() {
      return {
        userFullName: '',
        isMenuOpen: false,
        isUploadOpen: false,
        router: useRouter()
      };
    },
    computed: {
      userInitial() {
        return this.userFullName ? this.userFullName[0]?.toUpperCase() || '?' : '?';
      }
    },
    async created() {
      if (this.isLoggedIn) {
        await this.loadCurrentUser();
      }
    },
    methods: {
      async loadCurrentUser() {
        try {
          const response = await userApi.getCurrent();
          this.userFullName = response.FullName;
        } catch (error) {
          console.error('Ошибка загрузки данных пользователя:', error);
        }
      },
      toggleMenu() {
        this.isMenuOpen = !this.isMenuOpen;
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
        this.isUploadOpen = !this.isUploadOpen;
      },
      handleUploadComplete(data) {
        console.log('Загрузка завершена:', data);
        // Опционально: this.isUploadOpen = false;
      },
      handleClickOutside(event) {
        const userBlock = this.$el.querySelector('.user-info');
        if (userBlock && !userBlock.contains(event.target)) {
          this.isMenuOpen = false;
        }

        const uploadBlock = this.$el.querySelector('.upload-container');
        if (userBlock && !userBlock.contains(event.target)) {
          this.isUploadOpen = false;
        }
      }
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
/* Header Styles */
.page-header {
    background-color: #e4e3e8;
    position: relative;
    width: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: row;
    height: 160px;
}

.header-content {
    display: flex;
    width: 100%;
    /* flex-wrap: wrap; */
    align-items: center;
    padding: 0 15px;
}

/* Logo Section */
.logo-section {
    /* flex: 0 0 16.666667%; */
    display: flex;
    align-items: center;
    padding: 0 15px;
}

.logo-image {
    height: 8vh;
    padding-left: 1rem;
}

/* Button Sections */
.button-section {
    flex: 0 0 8.333333%;
    display: flex;
    justify-content: center;
    padding: 0 15px;
}

.button-section:nth-child(2) {
    margin-left: 8.333333%;
}

.nav-button {
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 0.25rem;
    border: none;
    background: transparent;
    cursor: pointer;
    transition: all 0.3s ease;
    width: 100%;
}

.nav-button:hover {
    background-color: #d4d3d9;
    border-radius: 0.375rem;
}

.button-icon {
    height: 8vh;
    margin: 0 auto;
}

.button-text {
    margin: 0;
    font-size: 0.875rem;
    color: #212529;
    text-align: center;
}

/* User Section */
.user-section {
    /* flex: 0 0 16.666667%; */
    margin-left: 17%;
    display: flex;
    justify-content: flex-end;
    align-items: flex-start;
    padding: 0 15px;
}

.user-info {
    padding-top: 0.5rem;
    display: flex;
    flex-direction: column;
    align-items: flex-end;
    width: 100%;
}

.user-main {
    display: flex;
    align-items: center;
    margin-bottom: 0.25rem;
}

.user-name {
    margin: 0;
    text-align: right;
    margin-right: 0.5rem;
    font-size: 0.875rem;
    color: #212529;
}

.edit-button {
    padding: 0;
    border: none;
    background-color: #e4e3e8;
    cursor: pointer;
    margin-right: 0.5rem;
}

.edit-icon {
    height: 2.5vh;
}

.avatar-wrapper {
    position: relative;
    padding: 0;
    cursor: pointer;
    background: transparent;
    border: none;
}

.avatar-image {
    position: relative;
}

.avatar-initial {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    margin: 0;
    font-size: 0.75rem;
    color: #212529;
    font-weight: 500;
}

.user-menu {
    position: absolute;
    right: 0;
    top: 100%;
    background: white;
    border: 1px solid #dee2e6;
    border-radius: 0.375rem;
    padding: 0.5rem;
    box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
    z-index: 1000;
    min-width: 160px;
    margin-top: 0.25rem;
}

.menu-item {
    text-align: left;
}

.menu-link {
    border: none;
    background: transparent;
    padding: 0;
    color: #0d6efd;
    text-decoration: underline;
    cursor: pointer;
    font-size: 0.875rem;
}

.menu-link:hover {
    color: #0a58ca;
}

/* User Actions */
.user-actions {
    display: flex;
    flex-direction: row;
    align-items: flex-end;
    width: 100%;
}

.action-button {
    font-size: 0.875rem;
    padding: 0.25rem 0.5rem;
    border: 1px solid transparent;
    border-radius: 0.375rem;
    cursor: pointer;
    transition: all 0.3s ease;
    margin-top: 0.5rem;
}

.upload-button {
    color: #fff;
    background-color: #198754;
    border-color: #6c757d;
}

.upload-button:hover {
    background-color: #5c636a;
    border-color: #565e64;
}

.clear-button {
    color: #fff;
    background-color: #cd1010;
    border-color: #6c757d;
}

.clear-button:hover {
    background-color: #5c636a;
    border-color: #565e64;
}

/* Upload Container */
.upload-container {
    /* margin-top: 0.5rem; */
    /* width: 100%; */
    z-index: 1;
    border-radius: 15px;
    position: absolute;
    width: 30%;
    height: auto;
    top: 40vh;
    left: 50%;
    transform: translate(-50%, -50%);
    box-shadow: 0px 0px 0px 9999px rgba(0, 0, 0, 0.5);
}

.upload-wrapper {
    position: relative;
    z-index: 1000;
}

/* Responsive Design */
@media (max-width: 1200px) {
    .header-content {
        padding: 0 10px;
    }
    
    .button-section {
        flex: 0 0 10%;
        padding: 0 10px;
    }
    
    .logo-section {
        flex: 0 0 15%;
    }
    
    .user-section {
        flex: 0 0 20%;
        margin-left: 10%;
    }
}

@media (max-width: 768px) {
    .header-content {
        flex-direction: column;
        padding: 0.5rem;
    }
    
    .logo-section,
    .button-section,
    .user-section {
        flex: 0 0 100%;
        margin: 0;
        padding: 0.5rem;
        justify-content: center;
    }
    
    .button-section:nth-child(2) {
        margin-left: 0;
    }
    
    .user-info {
        align-items: center;
    }
    
    .user-main {
        justify-content: center;
    }
    
    .user-actions {
        align-items: center;
    }
    
    .user-menu {
        right: auto;
        left: 50%;
        transform: translateX(-50%);
    }
}
</style>
