<!-- src/components/CustomAlert.vue -->
<template>
  <div v-if="visible" class="custom-alert-overlay" @click.self="close">
    <div class="custom-alert" :class="alertClass">
      <div class="alert-header" :style="headerStyle">
        <h3>{{ title }}</h3>
        <button v-if="closable" @click="close" class="close-btn">&times;</button>
      </div>
      <div class="alert-body">
        <p>{{ message }}</p>
      </div>
      <div class="alert-footer">
        <button @click="close" class="alert-btn">OK</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'CustomAlert',
  props: {
    visible: {
      type: Boolean,
      default: false
    },
    title: {
      type: String,
      default: 'Уведомление'
    },
    message: {
      type: String,
      default: ''
    },
    alertClass: {
      type: String,
      default: 'default'
    },
    closable: {
      type: Boolean,
      default: true
    },
    headerColor: {
      type: String,
      default: '#4CAF50'
    }
  },
  emits: ['update:visible', 'close'],
  computed: {
    headerStyle() {
      return {
        backgroundColor: this.headerColor
      }
    }
  },
  methods: {
    close() {
      this.$emit('update:visible', false);
      this.$emit('close');
    }
  }
}
</script>

<style scoped>
.custom-alert-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.custom-alert {
  background: white;
  border-radius: 8px;
  min-width: 300px;
  max-width: 500px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  animation: slideIn 0.3s ease;
}

.custom-alert.success {
  border-left: 4px solid #4CAF50;
}

.custom-alert.error {
  border-left: 4px solid #f44336;
}

.custom-alert.warning {
  border-left: 4px solid #ff9800;
}

.custom-alert.info {
  border-left: 4px solid #2196F3;
}

.alert-header {
  padding: 16px 20px;
  border-radius: 8px 8px 0 0;
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.alert-header h3 {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
}

.close-btn {
  background: none;
  border: none;
  color: white;
  font-size: 24px;
  cursor: pointer;
  padding: 0;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.close-btn:hover {
  opacity: 0.8;
}

.alert-body {
  padding: 20px;
}

.alert-body p {
  margin: 0;
  font-size: 16px;
  line-height: 1.5;
  color: #333;
}

.alert-footer {
  padding: 16px 20px;
  text-align: right;
  border-top: 1px solid #eee;
}

.alert-btn {
  background: #4CAF50;
  color: white;
  border: none;
  padding: 8px 20px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
}

.alert-btn:hover {
  background: #45a049;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Адаптивность */
@media (max-width: 576px) {
  .custom-alert {
    min-width: unset;
    width: 90%;
    margin: 0 20px;
  }
  
  .alert-header h3 {
    font-size: 16px;
  }
  
  .alert-body p {
    font-size: 14px;
  }
}
</style>