<template>
  <main>
    <GlobalLoader />
    <router-view />
  </main>
</template>

<script>
  import { mapGetters, mapState, mapActions } from 'vuex';
  import GlobalLoader from '@/components/layout/GlobalLoader.vue';
  import authService from '@/services/TokenService'; // ← прямой импорт сервиса токенов
  import router from '@/router/Routers'; // ← если нужно для редиректа (опционально)

  export default {
    name: 'App',
    components: {
      GlobalLoader
    },
    data() {
      return {
        authChecked: false,
        tokenRefreshInterval: null,
        isComponentMounted: false
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
    },
    beforeUnmount() {
      this.isComponentMounted = false;
      this.stopTokenRefreshInterval();
    },
    methods: {
      // Только реальные Vuex-экшны
      ...mapActions(['initializeAuth', 'logout']),

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
  /* Стили по необходимости */
</style>
