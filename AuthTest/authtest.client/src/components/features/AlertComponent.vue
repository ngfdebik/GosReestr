<!-- src/components/CustomAlert.vue -->
<template>
  <div v-show="visible" class="custom-alert-icon" :class="{ 'expanded': isExpanded }" @click="toggleSize">
    <div v-if="!isExpanded" class="status-block">
      <img v-if="status" src="/src/assets/complete.png" class="status-image" />
      <img v-else src="/src/assets/error.png" class="status-image" />
    </div>
    <div v-else class="expanded-content">
      <div class="close-button-container">
        <button class="close-button" @click="close">
        X
        </button>
      </div>
      <div class="content-container">
        <div v-for="item in content" class="content-cell">
          <div class="item-status">
            {{ item.status }}
          </div>
          <div class="item-name">
            {{ item.name }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { toRaw } from 'vue'

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
    },
    content:{
      type: Array,
      default: () => [
        {
          name: "qwe",
          status: "OK"
        },
        {
          name: "ewq", 
          status: "OK"
        },
        {
          name: "fff",
          status: "Error"
        }
      ]
    }
  },
  emits: ['update:visible', 'close'],
  data() {
    return {
      isExpanded: false,
    }
  },
  computed: {
    headerStyle() {
      return {
        backgroundColor: this.headerColor
      }
    },
    status() {
      return this.content[this.content.length-1]?.status == "OK"
    }
  },
  methods: {
    toggleSize() {
      this.isExpanded = !this.isExpanded;  // переключаем состояние
    },

    close() {
      this.$emit('update:visible', false);
      this.$emit('close');
    }
  }
}
</script>

<style scoped>
/* .custom-alert-overlay {
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
} */
.status-block{
  display: flex;
  justify-content: center;
}

.custom-alert-icon {
  background: rgb(221, 221, 221);
  border-radius: 8px;
  height: 60px;
  position: absolute;
  width: 50px;
  bottom: 5px;
  right: 10px;
  z-index: 1000;
  align-items: center;
  justify-content: center;
  display: flex;
  /*min-width: 300px;
  max-width: 500px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  animation: slideIn 0.3s ease;*/
}

.custom-alert-icon:hover {
  opacity: 0.8;
}

.custom-alert-icon.expanded {
  width: 300px;           /* меняем ширину */
  height: 350px;           /* высота по содержимому */
  min-height: 150px;      /* минимальная высота */
  display: block;         /* меняем тип display */
}
.close-button-container{
  width: 100%;
  display: flex;
  justify-content: end;
}
.expanded-content{
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
}
.close-button{
  margin-top: 5px;
  margin-right: 5px;
}

.content-container{
  width: 90%;
  height: 88%;
  margin-top: 5px;
  display: flex;
  flex-direction: column;
  background-color: #efefe0;
  /* justify-content: center; */
  border-radius: 5px;
}

.content-cell{
  display: flex;
  margin: 5px;
  height: 40px;
  /* background-color: #bfbfbf; */
  flex-direction: row;
  justify-content:space-between;
  align-items: center;
  border-radius: 5px;
}

.item-status{
  height: 100%;
  background-color:coral;
  border-radius: 8px 0 0 8px;
  width: 30%;
  display: flex;
  justify-content: center;
  align-items: center;
}
.item-name{
  background-color: cadetblue;
  border-radius: 0 8px 8px 0;
  height: 100%;
  width: 70%;
  display: flex;
  justify-content: center;
  align-items: center;
}

.status-image{
  width: 80%;
  height: 80%;
}
/* 
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

 Адаптивность 
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
} */
</style>