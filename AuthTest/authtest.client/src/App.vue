<template>
  <main>
    <!-- Глобальный алерт на уровне приложения -->
    <CustomAlert
      v-model:visible="globalAlert.show"
      :title="globalAlert.title"
      :message="globalAlert.message"
      :alert-class="globalAlert.alertClass"
      :header-color="globalAlert.headerColor"
      :content = "filesInfo"
    />
    
    <GlobalLoader />
    <router-view></router-view>

  </main>
</template>

<script>
  import { mapGetters, mapState, mapActions } from 'vuex';
  import GlobalLoader from '@/components/layout/GlobalLoader.vue';
  import authService from '@/services/TokenService'; // ← прямой импорт сервиса токенов
  import router from '@/router/Routers'; // ← если нужно для редиректа (опционально)
  import CustomAlert from '@/components/features/AlertComponent.vue';

  export default {
    name: 'App',
    components: {
      GlobalLoader,
      CustomAlert,
    },
    data() {
      return {
        authChecked: false,
        tokenRefreshInterval: null,
        isComponentMounted: false,
        // Глобальный алерт
        globalAlert: {
          show: false,
          title: '',
          message: '',
          alertClass: 'default',
          headerColor: '#4CAF50'
        },
        filesInfo: []
      };
    },
    computed: {
      ...mapGetters(['isAuthenticated']),
      ...mapState(['isLoading'])
    },
    async created() {
      try {
        await this.initializeAuth();
        this.authChecked = true;
      } catch (error) {
        console.error('Auth initialization failed:', error);
      }
    },
    mounted() {
      this.isComponentMounted = true;
      this.startTokenRefreshInterval();

      // Глобальная функция для показа алерта
      window.showGlobalAlert = this.showGlobalAlert;
    },
    beforeUnmount() {
      this.isComponentMounted = false;
      this.stopTokenRefreshInterval();

      window.showGlobalAlert = null;
    },
    methods: {
      // Только реальные Vuex-экшны
      ...mapActions(['initializeAuth', 'logout']),

      // Глобальная функция для показа алерта
      addInfo(info){
        this.filesInfo.unshift(info)
      },

      showGlobalAlert(status, name) {
        name = name.length <= 15 ? name : name.substring(0, 15) + "...";

        let object = {
          status : status,
          name : name 
        }
        this.addInfo(object)

        const alertConfig = {
          success: {
            title: 'Успех',
            alertClass: 'success',
            headerColor: '#4CAF50'
          },
          error: {
            title: 'Ошибка',
            alertClass: 'error',
            headerColor: '#f44336'
          },
          warning: {
            title: 'Предупреждение',
            alertClass: 'warning',
            headerColor: '#ff9800'
          },
          info: {
            title: 'Информация',
            alertClass: 'info',
            headerColor: '#2196F3'
          }
        };

        const config = /*alertConfig[type] || */alertConfig.info;
        
        this.globalAlert.title = config.title;
        this.globalAlert.alertClass = config.alertClass;
        this.globalAlert.headerColor = config.headerColor;
        this.globalAlert.content = this.filesInfo;
        this.globalAlert.show = true;
      },
      async handleLogout() {
        await this.logout();
        router.push('/login');
      },

      startTokenRefreshInterval() {
        // Обновляем токен каждые 10 минут, если пользователь авторизован
        this.tokenRefreshInterval = setInterval(async () => {
          if (this.isAuthenticated && this.isComponentMounted) {
            try {
              await authService.refreshAuthToken();
              console.log('Token refreshed successfully in background');
            } catch (error) {
              console.error('Background token refresh failed:', error);
              // Опционально: перенаправить на логин при неудаче
              authService.clearTokens();
              router.push('/login');
            }
          }
        }, 10 * 60 * 1000); // 10 минут
      },

      stopTokenRefreshInterval() {
        if (this.tokenRefreshInterval) {
          clearInterval(this.tokenRefreshInterval);
          this.tokenRefreshInterval = null;
        }
      }
    }
  };
</script>

<style scoped>
</style>
