<!-- src/components/Header.vue -->
<template>
  <header>
    <div class="row row-cols-auto" style="background-color: #e4e3e8; position: relative;">
      <!-- Логотип -->
      <div class="col-2 d-flex align-items-center">
        <img src="/svg/Logo.svg" style="height: 8vh;" class="ps-3" />
      </div>

      <!-- Кнопки фильтрации -->
      <div class="col-1 offset-1 d-flex justify-content-center">
        <button class="mainButton p-1" @click="$emit('load-all')">
          <img src="/svg/All.svg" title="Все" class="m-auto" style="height: 8vh;" />
          <p class="m-0">Все</p>
        </button>
      </div>
      <div class="col-1 d-flex justify-content-center">
        <button class="mainButton p-1" @click="$emit('load-ip')">
          <img src="/svg/IP.svg" title="ИП" class="m-auto" style="height: 8vh;" />
          <p class="m-0">ИП</p>
        </button>
      </div>
      <div class="col-1 d-flex justify-content-center">
        <button class="mainButton p-1" @click="$emit('load-ul')">
          <img src="/svg/UL.svg" title="ЮЛ" class="m-auto" style="height: 8vh;" />
          <p class="m-0">ЮЛ</p>
        </button>
      </div>

      <!-- Кнопки экспорта -->
      <div class="col-1 d-flex justify-content-center">
        <button @click="$emit('export-excel')" class="mainButton p-1">
          <img src="/svg/Export.svg" title="Экспорт в Excel" class="m-auto" style="height: 8vh;" />
          <p class="m-0">Экспорт в Excel</p>
        </button>
      </div>
      <div class="col-1 d-flex justify-content-center">
        <button @click="$emit('export-doc')" class="mainButton p-1">
          <img src="/svg/Export.svg" title="Экспорт в Word" class="m-auto" style="height: 8vh;" />
          <p class="m-0">Экспорт в Word</p>
        </button>
      </div>

      <!-- Блок с пользователем -->
      <div class="col-2 offset-2 d-flex justify-content-end align-items-start user-block">
        <div v-if="isLoggedIn" class="pt-2 d-flex flex-column align-items-end">
          <!-- Имя пользователя и иконка редактирования -->
          <div class="d-flex align-items-center mb-1">
            <p class="m-0 text-end me-2">{{ userFullName || 'Пользователь' }}</p>
            <button class="p-0"
                    style="border: none; background-color: #e4e3e8;"
                    @click="goToEdit">
              <img src="/svg/Pen.svg" style="height: 2.5vh;" />
            </button>
            <div class="btn p-0 avatarContainer" @click="toggleMenu">
              <img class="avatarContainer" src="/svg/Avatar.svg" />
              <p class="avatarText">{{ userInitial }}</p>
              <div v-if="isMenuOpen" class="user-menu mt-1">
                <div v-if="isAdmin" class="text-start">
                  <button class="btn btn-link p-0" @click="goToManage">Администрирование</button>
                </div>
                <div class="text-start">
                  <button class="btn btn-link p-0" @click="logout">Выйти</button>
                </div>
              </div>
            </div>
          </div>


          <!--НОВАЯ КНОПКА: Загрузка файлов (только для админа) -->
          <div>
            <button v-if="isAdmin"
                    class="btn btn-sm upload-files-btn mt-2"
                    @click="toggleUploadZone"
                    type="button">
              Загрузка файлов
            </button>
            <div v-if="isUploadOpen && isAdmin" class="upload-zone-container mt-2">
              <div class="upload-zone-wrapper">
                <FileUploadZone @upload-complete="handleUploadComplete" />
              </div>
            </div>
            <button class="btn btn-sm clear-filters-btn mt-2"
                    @click="$emit('clear-filters')"
                    type="button">
              Сбросить фильтры
            </button>
          </div>
          
        </div>
        
      </div>
    </div>

    <!--ФОРМА ЗАГРУЗКИ -->
    
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
        const userBlock = this.$el.querySelector('.user-block');
        if (userBlock && !userBlock.contains(event.target)) {
          this.isMenuOpen = false;
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
  .user-block {
    position: relative; /* Добавьте это свойство */
  }
  .clear-filters-btn {
    background-color: #dc3545; /* красный, как "очистить" */
    color: white;
    border: none;
    border-radius: 4px;
    padding: 4px 8px;
    font-size: 0.85rem;
    height: auto;
    min-width: 120px;
    text-align: center;
  }

    .clear-filters-btn:hover {
      background-color: #c82333;
    }
  .user-menu {
    position: absolute;
    top: 100%;
    right: 0;
    background: white;
    border: 1px solid #ccc;
    border-radius: 4px;
    padding: 8px;
    z-index: 1030;
    box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
    min-width: 180px;
  }

  .avatarContainer {
    position: relative;
    width: 32px;
    height: 32px;
  }

  .avatarText {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -25%);
    color: white;
    font-weight: bold;
    font-size: 0.8rem;
    margin: 0;
  }

  /*Стили для кнопки "Загрузка файлов" */
  .upload-files-btn {
    background-color: #007bff;
    color: white;
    border: none;
    border-radius: 4px;
    padding: 4px 8px;
    font-size: 0.85rem;
    height: auto;
    min-width: 100px;
    text-align: center;
  }

    .upload-files-btn:hover {
      background-color: #0056b3;
    }

  /*Контейнер формы загрузки */
  .upload-zone-container {
    position: absolute;
    top: 100%;
    right: 0;
    background: white;
    border: 1px solid #ccc;
    border-radius: 4px;
    padding: 8px;
    z-index: 1030;
    box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
    min-width: 180px;
  }

  .upload-zone-wrapper {
    max-width: 800px;
    margin: 0 auto;
  }
</style>
