<!-- src/components/FileUploadZone.vue -->
<template>
  <div class="upload-zone">
    <h5 class="mb-3">Загрузка данных</h5>
    <div class="dropzone p-4"
         @dragover.prevent
         @dragenter.prevent
         @dragleave="isDragging = false"
         @drop="handleDrop"
         @click="triggerFileInput"
         :class="{ 'dropzone--dragover': isDragging }">
      <p v-if="!isUploading">
        {{ isDragging ? 'Отпустите файл' : 'Перетащите сюда XML или ZIP' }}
        <br />
        <small class="text-muted">Поддерживаются .xml и .zip</small>
      </p>
      <p v-else>
        Загрузка...
        <span v-if="uploadProgress > 0">{{ uploadProgress }}%</span>
      </p>
      <input type="file"
             ref="fileInput"
             style="display: none"
             @change="handleFileSelect"
             accept=".xml,.zip" />
    </div>

    <div v-if="statusMessage" class="mt-2 alert" :class="statusClass">
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
    methods: {
      triggerFileInput() {
        this.$refs.fileInput.click();
      },

      handleFileSelect(event) {
        const file = event.target.files[0];
        if (file) {
          this.uploadFile(file);
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

        // try {
          // Используем тот же origin, что и основной сайт
          // const response = await fetch('/api/Home/upload', {
          //   method: 'POST',
          //   body: formData,
          // });

          UploadApiSrvice.uploadFile(formData)
          .then(resp => {
            this.showMessage('Файл успешно загружен и обработан!', 'alert-success');
            // Опционально: уведомить родителя о завершении
            this.$emit('upload-complete');
            console.log(resp);
          })
          .catch(err => {
            //this.showMessage(`Ошибка: ${err || 'Неизвестная ошибка'}`, 'alert-danger');
            console.error('Upload error:', err);
            this.showMessage('Не удалось подключиться к серверу', 'alert-danger')
          })

          this.isUploading = false;
          this.$refs.fileInput.value = '';
        // }

        //   if (response.ok) {
        //     this.showMessage('Файл успешно загружен и обработан!', 'alert-success');
        //     // Опционально: уведомить родителя о завершении
        //     this.$emit('upload-complete');
        //   } else {
        //     const errorText = await response.text();
        //     this.showMessage(`Ошибка: ${errorText || 'Неизвестная ошибка'}`, 'alert-danger');
        //   }
        // } catch (err) {
        //   console.error('Upload error:', err);
        //   this.showMessage('Не удалось подключиться к серверу', 'alert-danger');
        // } finally {
        //    // сброс для повторной загрузки
        // }
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

  .dropzone {
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
  }

    .dropzone:hover {
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
  }
</style>
