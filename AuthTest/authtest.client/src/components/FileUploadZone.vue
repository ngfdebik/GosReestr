<!-- src/components/FileUploadZone.vue -->
<template>
  <div ref="rootElement" v-show="isDragOver" class="upload-zone">
    <h5 class="upload-title">Загрузка данных</h5>
    <div class="drop-zone"
         @click="triggerFileInput"
         :class="{ 'drop-zone--active': isDragging }">
      
      <!-- Контент когда НЕ загружается -->
      <div v-if="!isUploading" class="upload-content">
        <p>
          {{ isDragging ? 'Отпустите файл' : 'Перетащите сюда XML или ZIP' }}
          <br />
          <small class="file-hint">Поддерживаются .xml и .zip</small>
        </p>
      </div>
      
      <!-- Контент когда загружается -->
      <div v-else class="uploading-content">
        <div class="loading-animation">
          <div class="loading-spinner"></div>
          <p class="loading-text">Загрузка данных...</p>
          <span v-if="uploadProgress > 0" class="progress-text">{{ uploadProgress }}%</span>
        </div>
      </div>
      
      <input type="file"
             ref="fileInput"
             class="file-input"
             @change="handleFileSelect"
             accept=".xml,.zip" />
    </div>
    <div v-if="statusMessage" class="status-alert" :class="statusClass">
      {{ statusMessage }}
    </div>
  </div>
</template>

<script>
import UploadApiSrvice from '@/services/UploadApiSrvice';

export default {
  name: 'FileUploadZone',
  props: {
    externalUploading: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      isDragging: false,
      isUploading: false,
      isDragOver: false,
      uploadProgress: 0,
      statusMessage: '',
      statusClass: '',
      dragCounter: 0,
      activeUpload: false,
      eventHandlers: null
    };
  },
  computed: {
    actualUploading() {
      return this.externalUploading || this.isUploading;
    }
  },
  watch: {
    externalUploading(newVal) {
      if (newVal) {
        this.activeUpload = true;
      }
    },
    isUploading(newVal) {
      if (newVal) {
        this.activeUpload = true;
      } else {
        setTimeout(() => {
          this.activeUpload = false;
        }, 100);
      }
    }
  },
  mounted() {
    
    // Инициализация с небольшой задержкой для гарантии готовности DOM
    setTimeout(() => {
      this.initializeEventListeners();
    }, 100);
  },
  beforeUnmount() {
    this.removeEventListeners();
  },
  methods: {
    initializeEventListeners() {
      
      // Удаляем старые обработчики
      this.removeEventListeners();
      
      // Создаем объект с обработчиками
      this.eventHandlers = {
        dragenter: this.handleGlobalDragEnter.bind(this),
        dragover: this.handleGlobalDragOver.bind(this),
        dragleave: this.handleGlobalDragLeave.bind(this),
        drop: this.handleGlobalDrop.bind(this)
      };
      
      // Добавляем обработчики на все уровни с capture phase
      const levels = [window, document, document.documentElement, document.body];
      const options = { capture: true, passive: false };
      
      Object.entries(this.eventHandlers).forEach(([event, handler]) => {
        levels.forEach(level => {
          level.addEventListener(event, handler, options);
        });
      });
    },
    
    removeEventListeners() {
      if (!this.eventHandlers) return;
      
      const levels = [window, document, document.documentElement, document.body];
      
      Object.entries(this.eventHandlers).forEach(([event, handler]) => {
        levels.forEach(level => {
          level.removeEventListener(event, handler, true);
        });
      });
      
      this.eventHandlers = null;
    },
    
    handleGlobalDragEnter(e) {
      
      e.preventDefault();
      e.stopImmediatePropagation();
      
      this.dragCounter++;
      
      if (this.dragCounter === 1 && !this.isUploading) {
        this.isDragOver = true;
        this.isDragging = true;
      }
    },
    
    handleGlobalDragOver(e) {
      // Не логируем - слишком много событий
      e.preventDefault();
      e.stopImmediatePropagation();
      
      if (e.dataTransfer) {
        e.dataTransfer.dropEffect = 'copy';
      }
    },
    
    handleGlobalDragLeave(e) {
      
      e.preventDefault();
      e.stopImmediatePropagation();
      
      // Проверяем, вышли ли мы за пределы окна
      const isLeavingWindow = (
        e.clientY <= 0 ||
        e.clientX <= 0 ||
        e.clientX >= window.innerWidth ||
        e.clientY >= window.innerHeight
      );
      
      if (isLeavingWindow) {
        this.dragCounter = Math.max(0, this.dragCounter - 1);
        
        if (this.dragCounter === 0) {
          this.isDragOver = false;
          this.isDragging = false;
        }
      }
    },
    
    handleGlobalDrop(e) {
      
      e.preventDefault();
      e.stopImmediatePropagation();
      
      // Сбрасываем состояние
      this.isDragOver = false;
      this.isDragging = false;
      this.dragCounter = 0;
      
      // Получаем файлы
      const files = e.dataTransfer.files;
      
      if (files.length > 0) {
        const file = files[0];
        
        // Валидация файла
        if (this.isValidFile(file)) {
          // Немедленная обработка
          setTimeout(() => {
            this.uploadFile(file);
          }, 0);
        } else {
          this.showMessage('Неверный формат файла. Только .xml или .zip.', 'alert-danger');
        }
      }
      
      return false;
    },
    
    triggerFileInput() {
      if (!this.isUploading) {
        this.$refs.fileInput.click();
      }
    },
    
    handleFileSelect(event) {
      const file = event.target.files[0];
      if (file && !this.isUploading) {
        if (this.isValidFile(file)) {
          this.uploadFile(file);
        } else {
          this.showMessage('Неверный формат файла. Только .xml или .zip.', 'alert-danger');
        }
      }
    },
    
    isValidFile(file) {
      if (!file) return false;
      const validExtensions = ['.xml', '.zip'];
      const fileName = file.name.toLowerCase();
      return validExtensions.some(ext => fileName.endsWith(ext));
    },
    
    async uploadFile(file) {
      
      if (file.size === 0) {
        this.showMessage('Файл пустой', 'alert-danger');
        return;
      }
      
      this.isUploading = true;
      this.activeUpload = true;
      this.uploadProgress = 0;
      this.isDragOver = false; // Скрываем зону загрузки
      
      const formData = new FormData();
      formData.append('file', file);
      
      try {
        const response = await UploadApiSrvice.uploadFile(formData);
        
        if (window.showGlobalAlert) {
          window.showGlobalAlert("OK", response.name || "Файл успешно загружен");
        }
        
        // Эмитим событие успеха
        this.$emit('upload-success', response);
        
      } catch (error) {
        console.error('Ошибка загрузки:', error);
        
        let errorMessage = 'Ошибка загрузки файла';
        if (error.response) {
          errorMessage = error.response.data?.message || 
                        `Ошибка сервера: ${error.response.status}`;
        } else if (error.request) {
          errorMessage = 'Сервер не отвечает';
        }
        
        if (window.showGlobalAlert) {
          window.showGlobalAlert("Error", errorMessage);
        }
        
        // Эмитим событие ошибки
        this.$emit('upload-error', error);
        
        // Показываем сообщение в компоненте
        this.showMessage(errorMessage, 'alert-danger');
        
      } finally {
        this.isUploading = false;
        this.uploadProgress = 0;
        this.resetFileInput();
        
        // Небольшая задержка для сброса состояния
        setTimeout(() => {
          this.activeUpload = false;
        }, 500);
      }
    },
    
    resetFileInput() {
      if (this.$refs.fileInput && this.$refs.fileInput.value) {
        this.$refs.fileInput.value = '';
      }
    },
    
    showMessage(message, className) {
      this.statusMessage = message;
      this.statusClass = className;
      
      // Автоматически скрываем сообщение через 5 секунд
      setTimeout(() => {
        this.statusMessage = '';
        this.statusClass = '';
      }, 5000);
    }
  }
};
</script>

<style scoped>
.upload-zone {
  background-color: #090d10cf;
  position: absolute;
  z-index: 2147483647; /* Максимальный z-index */
  top: 160px;
  height: 804px;
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.upload-title {
  font-size: 1.25rem;
  font-weight: 500;
  margin-bottom: 1rem;
  color: white;
  text-shadow: 0 2px 4px rgba(0,0,0,0.3);
}

.drop-zone {
  border: 2px dashed #dee2e6;
  border-radius: 0.375rem;
  padding: 1.5rem;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
  background-color: #f8f9fa;
  min-height: 120px;
  width: 90%;
  max-width: 500px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}

.drop-zone--active {
  border-color: #727272b6;
  background-color: #535353ab;
  border-style: solid;
}

.drop-zone p {
  margin: 0;
  color: #6c757d;
  font-size: 1rem;
  line-height: 1.5;
}

.file-hint {
  color: #6c757d !important;
  font-size: 0.875rem;
  margin-top: 0.5rem;
  display: block;
}

.file-input {
  display: none;
}

/* Стили для анимации загрузки */
.uploading-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 100%;
}

.loading-animation {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #e3e3e3;
  border-top: 4px solid #0d6efd;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

.loading-text {
  color: #0d6efd;
  font-weight: 500;
  margin: 0;
  font-size: 1rem;
}

.progress-text {
  color: #6c757d;
  font-size: 0.875rem;
  font-weight: 500;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Отключаем взаимодействие во время загрузки */
.drop-zone:has(.uploading-content) {
  cursor: not-allowed;
  background-color: #f8f9fa;
}

.drop-zone:has(.uploading-content):hover {
  border-color: #dee2e6;
  background-color: #f8f9fa;
}

/* Status Alert Styles */
.status-alert {
  padding: 0.75rem 1rem;
  margin-top: 0.5rem;
  border: 1px solid transparent;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  background: white;
  max-width: 500px;
  width: 90%;
}

.status-success {
  color: #0f5132;
  background-color: #d1e7dd;
  border-color: #badbcc;
}

.status-error {
  color: #842029;
  background-color: #f8d7da;
  border-color: #f5c2c7;
}

.status-warning {
  color: #664d03;
  background-color: #fff3cd;
  border-color: #ffecb5;
}

.status-info {
  color: #055160;
  background-color: #cff4fc;
  border-color: #b6effb;
}

/* Responsive Design */
@media (max-width: 576px) {
  .upload-zone {
    top: 0;
    height: 100vh;
    padding: 20px;
  }
  
  .drop-zone {
    padding: 1rem;
    min-height: 100px;
  }
  
  .drop-zone p {
    font-size: 0.9rem;
  }
  
  .file-hint {
    font-size: 0.8rem;
  }
  
  .loading-spinner {
    width: 32px;
    height: 32px;
    border-width: 3px;
  }
  
  .loading-text {
    font-size: 0.9rem;
  }
}
</style>