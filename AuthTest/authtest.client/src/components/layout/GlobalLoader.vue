<!-- src/components/GlobalLoader.vue -->
<template>
  <div v-if="isGlobalLoading" class="global-loader-overlay">
    <div class="global-loader-spinner"></div>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  name: 'GlobalLoader',
  computed: {
    ...mapGetters(['isGlobalLoading'])
  }
};
</script>

<style scoped>
  .global-loader-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    background-color: rgba(0, 0, 0, 0.3);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 9999;
  }

  .global-loader-spinner {
    width: 48px;
    height: 48px;
    border: 5px solid rgba(255, 255, 255, 0.3);
    border-radius: 50%;
    border-top-color: #007bff;
    animation: spin 1s ease-in-out infinite;
  }

  @keyframes spin {
    to {
      transform: rotate(360deg);
    }
  }
</style>
