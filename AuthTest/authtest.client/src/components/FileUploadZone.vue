<!-- src/components/FileUploadZone.vue -->
<template>
  
  <div class="upload-zone">
    <h5 class="upload-title">Загрузка данных</h5>
      <div class="drop-zone"
           @dragover="handleDragOver"
           @dragenter="handleDragEnter"
           @dragleave="handleDragLeave"
           @drop="handleDrop"
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
      // Добавляем пропс для внешнего управления состоянием загрузки
      externalUploading: {
        type: Boolean,
        default: false
      }
    },
    data() {
      return {
        isDragging: false,
        isUploading: false,
        uploadProgress: 0,
        statusMessage: '',
        statusClass: '',
        dragCounter: 0, // Добавлено для корректной работы drag&drop

        // Локальное состояние для отслеживания активной загрузки
        activeUpload: false
      };
    },
    computed: {
      // Комбинируем внешнее и внутреннее состояние
      actualUploading() {
        return this.externalUploading || this.isUploading;
      }
    },
    watch: {
      // Следим за изменениями внешнего состояния
      externalUploading(newVal) {
        if (newVal) {
          this.activeUpload = true;
        }
      },
      isUploading(newVal) {
        if (newVal) {
          this.activeUpload = true;
        } else {
          // Сбрасываем activeUpload только когда загрузка полностью завершена
          setTimeout(() => {
            this.activeUpload = false;
          }, 100);
        }
      }
    },
    mounted() {
      // Критически важные обработчики на уровне документа
      document.addEventListener('dragover', this.preventDragDrop, false);
      document.addEventListener('drop', this.preventDragDrop, false);
    },
    beforeUnmount() {
      document.removeEventListener('dragover', this.preventDragDrop, false);
      document.removeEventListener('drop', this.preventDragDrop, false);
    },
    methods: {

      // Универсальный метод для предотвращения поведения по умолчанию
      preventDragDrop(e) {
        e.preventDefault();
        e.stopPropagation();
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
          this.uploadFile(file);
        }
      },

      handleDragOver(event) {
        this.preventDragDrop(event);
        if (!this.isDragging && !this.isUploading) {
          this.isDragging = true;
        }
        // Указываем браузеру, что мы хотим скопировать файл
        if (!this.isUploading) {
          event.dataTransfer.dropEffect = 'copy';
        }
      },

      handleDragEnter(event) {
        if (this.isUploading) return;
        event.preventDefault();
        event.stopPropagation();
        this.dragCounter++;
        this.isDragging = true;
      },

      handleDragLeave(event) {
        this.preventDragDrop(event);
        this.dragCounter--;
        
        // Сбрасываем состояние только когда вышли из элемента
        if (this.dragCounter === 0) {
          this.isDragging = false;
        }
      },

      handleDrop(event) {
        if (this.isUploading) return;
        this.isDragging = false;
        this.dragCounter = 0;
        const file = event.dataTransfer.files[0];
        if (this.isValidFile(file)) {
          this.uploadFile(file);
        } else {
          this.showMessage('Неверный формат файла. Только .xml или .zip.', 'alert-danger');
        }
      },

      isValidFile(file) {
        if (!file) return false;
        const validExtensions = ['.xml', '.zip'];
        const ext = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();
        return validExtensions.includes(ext);
      },

      async uploadFile(file) {
        this.isUploading = true;
        this.activeUpload = true;
        this.uploadProgress = 0;
        
        const formData = new FormData();
        formData.append('file', file);

          UploadApiSrvice.uploadFile(formData)
          .then(resp => {
            // this.showMessage('Файл успешно загружен и обработан!', 'success');
            // Опционально: уведомить родителя о завершении
            // this.$emit('upload-complete', 'Файл успешно загружен и обработан!', 'success');
            if (window.showGlobalAlert) {
              window.showGlobalAlert('Файл успешно загружен и обработан!', 'success');
            }
            console.log(resp);
          })
          .catch(err => {
            console.error('Upload error:', err);
            if (window.showGlobalAlert) {
              window.showGlobalAlert('Не удалось подключиться к серверу', 'error');
            }
            // this.emit('upload-complete', 'Не удалось подключиться к серверу', 'alert-danger')
            
            // this.showMessage('Не удалось подключиться к серверу', 'alert-danger')
          })
          .finally(() => {
            this.isUploading = false;
            this.uploadProgress = 0;
            this.resetFileInput();
          })
      },

      resetFileInput() {
        // Безопасный способ сброса file input
        if (this.$refs.fileInput && this.$refs.fileInput.value) {
          this.$refs.fileInput.value = '';
        }
      },

      
    },
  };
</script>

<style scoped>
  .upload-zone {
    background-color: #f8f9fa;
    border-radius: 8px;
    padding: 16px;
  }
  .upload-title {
    font-size: 1.25rem;
    font-weight: 500;
    margin-bottom: 1rem;
    color: #212529;
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
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
}

.drop-zone:hover {
    border-color: #0d6efd;
    background-color: #e9ecef;
}

.drop-zone--active {
    border-color: #0d6efd;
    background-color: #e7f1ff;
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
    .upload-container {
        padding: 0 15px;
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