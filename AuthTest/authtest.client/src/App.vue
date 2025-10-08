
<template>
  <main>
    <GlobalLoader />
    <router-view>
    </router-view>
  </main>
  <!--<main>
    <Login />
  </main>-->
</template>

<script>
  import { mapActions, mapGetters, mapState } from 'vuex';
  import GlobalLoader from '@/components/layout/GlobalLoader.vue';

export default {
    name: 'App',
    components: {
      GlobalLoader //зарегистрировано
    },
  data() {
    return {
      authChecked: false,
    };
  },
  computed: {
    ...mapGetters(['isAuthenticated']),
    ...mapState(['isLoading']),
  },
  async created() {
    // Инициализация аутентификации при запуске приложения
    await this.initializeAuth();
    this.authChecked = true;
    
    // Запускаем периодическую проверку токена
    this.startTokenRefreshInterval();
  },
  beforeUnmount() {
    this.stopTokenRefreshInterval();
  },
  methods: {
    ...mapActions(['initializeAuth', 'logout', 'refreshToken']),
    
    async handleLogout() {
      await this.logout();
      this.$router.push('/login');
    },
    
    startTokenRefreshInterval() {
      // Проверяем токен каждые 10 минут
      this.tokenRefreshInterval = setInterval(async () => {
        if (this.isAuthenticated) {
          try {
            await this.refreshToken();
            console.log('Token refreshed successfully');
          } catch (error) {
            console.error('Background token refresh failed:', error);
          }
        }
      }, 10 * 60 * 1000); // 10 minutes
    },
    
    stopTokenRefreshInterval() {
      if (this.tokenRefreshInterval) {
        clearInterval(this.tokenRefreshInterval);
      }
    },
  },
};
</script>

<style scoped>
</style>
