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
          <p v-if="!isUploading">
              {{ isDragging ? 'Отпустите файл' : 'Перетащите сюда XML или ZIP' }}
              <br />
              <small class="file-hint">Поддерживаются .xml и .zip</small>
          </p>
          <p v-else>
              Загрузка...
              <span v-if="uploadProgress > 0">{{ uploadProgress }}%</span>
          </p>
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
    data() {
      return {
        isDragging: false,
        isUploading: false,
        uploadProgress: 0,
        statusMessage: '',
        statusClass: '',
      };
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
        this.$refs.fileInput.click();
      },

      handleFileSelect(event) {
        const file = event.target.files[0];
        if (file) {
          this.uploadFile(file);
        }
      },

      handleDragOver(event) {
        this.preventDragDrop(event);
        if (!this.isDragging) {
          this.isDragging = true;
        }
        // Указываем браузеру, что мы хотим скопировать файл
        event.dataTransfer.dropEffect = 'copy';
      },

      handleDragEnter(event) {
        event.preventDefault();
        event.stopPropagation();
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
        this.isDragging = false;
        const file = event.dataTransfer.files[0];
        if (this.isValidFile(file)) {
          this.uploadFile(file);
        } else {
          this.showMessage('Неверный формат файла. Только .xml или .zip.', 'alert-danger');
        }
      },

      isValidFile(file) {
        const validExtensions = ['.xml', '.zip'];
        const ext = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();
        return validExtensions.includes(ext);
      },

      async uploadFile(file) {
        this.isUploading = true;
        this.uploadProgress = 0;
        this.statusMessage = '';
        const formData = new FormData();
        formData.append('file', file);

          UploadApiSrvice.uploadFile(formData)
          .then(resp => {
            this.showMessage('Файл успешно загружен и обработан!', 'alert-success');
            // Опционально: уведомить родителя о завершении
            this.$emit('upload-complete');
            console.log(resp);
          })
          .catch(err => {
            console.error('Upload error:', err);
            this.showMessage('Не удалось подключиться к серверу', 'alert-danger')
          })

          this.isUploading = false;
          this.$refs.fileInput.value = '';
      },

      showMessage(message, className) {
        this.statusMessage = message;
        this.statusClass = className;
        setTimeout(() => {
          this.statusMessage = '';
        }, 5000);
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

  /* .dropzone {
    border: 2px dashed #ced4da;
    border-radius: 10px;
    text-align: center;
    cursor: pointer;
    transition: all 0.2s;
    min-height: 120px;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
  } */

    /* .dropzone:hover {
      border-color: #007bff;
      background-color: #e9f2ff;
    }

  .dropzone--dragover {
    border-color: #28a745;
    background-color: #e6f9ee;
  }

  .alert {
    padding: 8px 12px;
    border-radius: 4px;
    font-size: 0.9em;
  } */

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
  }
</style>
