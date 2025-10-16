<template>
  <Transition name="modal-fade">
    <div v-show="modelValue"
         class="vue-modal"
         ref="modal"
         tabindex="-1"
         aria-modal="true"
         @click.self="hideModal">
      <div class="vue-modal-dialog">
        <div class="vue-modal-content">
          <div class="vue-modal-content-header">
            <h5 class="vue-title">Дополнительная информация</h5>
            <button type="button"
                    class="vue-button-close"
                    @click="hideModal"
                    aria-label="Закрыть">
              <svg xmlns="http://www.w3.org/2000/svg"
                   viewBox="0 0 16 16"
                   width="16"
                   height="16">
                <path d="M.293.293a1 1 0 011.414 0L8 6.586 14.293.293a1 1 0 111.414 1.414L9.414 8l6.293 6.293a1 1 0 01-1.414 1.414L8 9.414l-6.293 6.293a1 1 0 01-1.414-1.414L6.586 8 .293 1.707a1 1 0 010-1.414z"
                      fill="currentColor" />
              </svg>
            </button>
          </div>
          <h3 class="vue-h3">{{ entityName }}</h3>
          <div class="vue-modal-body">
            <div class="vue-p-2">
              <select v-model="localSelectedDetail" class="vue-select">
                <option disabled value="">Выберите раздел</option>
                <option v-for="section in availableSections"
                        :key="section.value"
                        :value="section.value">
                  {{ section.label }}
                </option>
              </select>
              <button class="vue-btn vue-btn--right" @click="emitLoadDetails">Показать</button>
              <button class="vue-btn" @click="emitLoadLogs">Показать Историю Изменений</button>
            </div>

            <div class="vue-table-container">
              <table v-if="detailsData.length > 0" class="tableModal-table">
                <thead>
                  <tr>
                    <th v-for="header in detailsHeaders" :key="header">
                      {{ header }}
                    </th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(row, i) in trimmedData" :key="i">
                    <td v-for="header in detailsHeaders" :key="header">
                      {{ formatCell(row[header]) }}
                    </td>
                  </tr>
                </tbody>
              </table>
              <h2 v-else>Нет данных для отображения</h2>
            </div>
          </div>
        </div>
      </div>
    </div>
  </Transition>
</template>

<script>
  export default {
    name: 'ModalDetails',
    props: {
      modelValue: { type: Boolean, default: false },
      entityId: { type: String, required: true },
      entityInn: { type: String, required: true,default:'' },
      entityType: { type: String, required: true },
      entityName: { type: String, default: '' },
      detailsData: { type: Array, default: () => [] },
      detailsHeaders: { type: Array, default: () => [] }
    },
    emits: ['update:modelValue', 'close', 'load-details', 'load-logs'],
    data() {
      return {
        localSelectedDetail: ''
      };
    },
    computed: {
      //isVisible() {
      //  return this.modelValue;
      //},
      trimmedData() {
        return this.detailsData.map(row => {
          const { Id, idЛицо, ИП, ЮрЛицо, ...rest } = row;
          return rest;
        });
      },
      availableSections()
      {
        const ipSections = [
          { value: 'EGRIPOKVED', label: 'ОКВЭД' },
          { value: 'EGRIPSvAdrMJ', label: 'АдресМЖ' },
          { value: 'EGRIPSvGrajd', label: 'Гражд' },
          { value: 'EGRIPSVFL', label: 'ФЛ' },
          { value: 'EGRIPSvLicense', label: 'Лицензия' },
          { value: 'EGRIPSvPrekras_', label: 'Прекращ' },
          { value: 'EGRIPSvRegIP', label: 'РегИП' },
          { value: 'EGRIPSvGegOrg', label: 'РегОрг' },
          { value: 'EGRIPSvRegPF', label: 'РегПФ' },
          { value: 'EGRIPSvRegFSS', label: 'РегФСС' },
          { value: 'EGRIPSvAccountingNO', label: 'УчетНО' }
        ];
        const ulSections = [
          { value: 'EGRULOKVED', label: 'ОКВЭД' },
          { value: 'EGRULSvAddressUL', label: 'АдресЮЛ' },
          { value: 'EGRULSvDerjRegistryAO', label: 'ДержРегистрАО' },
          { value: 'EGRULSvShareOOO', label: 'ДоляООО' },
          { value: 'EGRULSvZapEGRUL', label: 'ЗапЕГРЮЛ' },
          { value: 'EGRULSvLicense', label: 'Лицензия' },
          { value: 'EGRULSvNaimUL', label: 'НаимЮЛ' },
          { value: 'EGRULSvObrUL', label: 'ОбрЮЛ' },
          { value: 'EGRULSvPodrazd', label: 'Подразд' },
          { value: 'EGRULSvPredsh', label: 'Предш' },
          { value: 'EGRULSvPreem', label: 'Преем' },
          { value: 'EGRULSvPrekrUL', label: 'ПрекрЮЛ' },
          { value: 'EGRULSvRegOrg', label: 'РегОрг' },
          { value: 'EGRULSvRegPF', label: 'РегПФ' },
          { value: 'EGRULSvRegFSS', label: 'РегФСС' },
          { value: 'EGRULSvReorg', label: 'Реорг' },
          { value: 'EGRULSvStatus', label: 'Статус' },
          { value: 'EGRULSvUstKap', label: 'УстКап' },
          { value: 'EGRULSvAccountingNO', label: 'УчетНО' },
          { value: 'EGRULSvFounder', label: 'Учредит' },
          { value: 'EGRULSvUL', label: 'ЮЛ' },
          { value: 'EGRULSvedDoljnFL', label: 'ДолжнФЛ' }
        ];
        return this.entityType === 'IP' ? ipSections : ulSections;
      }
    },
    watch: {
      modelValue(newVal) {
        if (newVal) {
          this.localSelectedDetail = '';
          this.$nextTick(() => {
            this.$refs.modal?.focus();
          });
        }
      }
    },
    mounted() {
      document.addEventListener('keydown', this.handleKeydown);
    },
    beforeUnmount() {
      document.removeEventListener('keydown', this.handleKeydown);
    },
    methods: {
      handleKeydown(e) {
        if (e.key === 'Escape' && this.modelValue) {
          this.hideModal();
        }
      },
      formatCell(value) {
        if (typeof value !== 'string') return value;
        if (/^\d{4}-\d{2}-\d{2}T/.test(value)) {
          const date = new Date(value);
          if (!isNaN(date.getTime())) {
            return date.toLocaleDateString('ru-RU');
          }
        }
        return value;
      },
      hideModal() {
        this.$emit('update:modelValue', false);
        this.$emit('close');
      },
      emitLoadDetails() {
        if (!this.localSelectedDetail) return;
        this.$emit('load-details', {
          table: this.localSelectedDetail,
          id: this.entityId
        });
      },
      emitLoadLogs() {
        if (!this.localSelectedDetail) return;
        this.$emit('load-logs', {
          table: this.localSelectedDetail,
          inn: this.entityInn
        });
      }
    }
  };
</script>

<style scoped>
  /* === Глобальные переменные модального окна === */
  .vue-modal {
    --modal-backdrop: rgba(0, 0, 0, 0.5);
    --modal-bg: #fff;
    --modal-border-color: rgba(0, 0, 0, 0.2);
    --modal-border-radius: 0.3rem;
    --modal-padding: 1rem;
    --modal-padding-sm: 0.5rem;
    --modal-header-border: #dee2e6;
    --modal-primary: #0d6efd;
    --modal-primary-hover: #0a58ca;
    --modal-text: #212529;
    --modal-header-bg: #e2e3e5;
    --modal-close-opacity: 0.5;
    --modal-close-opacity-hover: 0.75;
    --modal-table-border: #000;
    --modal-font-family: 'Times New Roman', serif;
    --modal-z-index: 1055;
  }

  /* === Анимация появления === */
  .modal-fade-enter-active,
  .modal-fade-leave-active {
    transition: opacity 0.15s ease-in-out;
  }

  .modal-fade-enter-from,
  .modal-fade-leave-to {
    opacity: 0;
  }

  /* === Фон и контейнер === */
  .vue-modal {
    position: fixed;
    top: 0;
    left: 0;
    z-index: var(--modal-z-index);
    width: 100vw;
    height: 100vh;
    overflow: hidden;
    background-color: var(--modal-backdrop);
  }

  .vue-modal-dialog {
    position: relative;
    width: auto;
    max-width: 1140px;
    margin: 1.75rem auto;
    pointer-events: none;
  }

  .vue-modal-content {
    position: relative;
    display: flex;
    flex-direction: column;
    width: 100%;
    pointer-events: auto;
    background-color: var(--modal-bg);
    background-clip: padding-box;
    border: 1px solid var(--modal-border-color);
    border-radius: var(--modal-border-radius);
  }

  /* === Шапка === */
  .vue-modal-content-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: var(--modal-padding);
    border-bottom: 1px solid var(--modal-header-border);
    border-top-left-radius: calc(var(--modal-border-radius) - 1px);
    border-top-right-radius: calc(var(--modal-border-radius) - 1px);
  }

  .vue-title {
    margin: 0;
    line-height: 1.5;
    font-size: 1.25rem;
    font-weight: 500;
  }

  /* === Кнопка закрытия === */
  .vue-button-close {
    box-sizing: content-box;
    width: 1em;
    height: 1em;
    padding: var(--modal-padding-sm);
    margin: calc(-1 * var(--modal-padding-sm)) calc(-1 * var(--modal-padding-sm)) calc(-1 * var(--modal-padding-sm)) auto;
    background: none;
    border: none;
    border-radius: 0.25rem;
    opacity: var(--modal-close-opacity);
    cursor: pointer;
    color: currentColor;
  }

    .vue-button-close:hover {
      opacity: var(--modal-close-opacity-hover);
    }

    .vue-button-close:focus-visible {
      outline: 2px solid var(--modal-primary);
      outline-offset: 2px;
      border-radius: 0.25rem;
    }

  /* === Заголовок сущности === */
  .vue-h3 {
    padding-left: var(--modal-padding-sm);
    margin: 0.25rem 0;
    font-size: 1.25rem;
    font-weight: 300;
    color: var(--modal-text);
  }

  /* === Тело модалки === */
  .vue-modal-body {
    position: relative;
    flex: 1 1 auto;
    padding: var(--modal-padding);
  }

  .vue-p-2 {
    padding: var(--modal-padding-sm);
  }

  .vue-select {
    margin: var(--modal-padding-sm);
    padding: 0.375rem 0.75rem;
    font-size: 1rem;
    border: 1px solid #ced4da;
    border-radius: 0.25rem;
    background-color: #fff;
    color: var(--modal-text);
  }

    .vue-select:focus-visible {
      outline: none;
      border-color: var(--modal-primary);
      box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.25);
    }

  /* === Кнопки === */
  .vue-btn {
    display: inline-block;
    font-weight: 400;
    line-height: 1.5;
    text-align: center;
    text-decoration: none;
    vertical-align: middle;
    user-select: none;
    background-color: transparent;
    border: 1px solid var(--modal-primary);
    color: var(--modal-primary);
    padding: 0.375rem 0.75rem;
    font-size: 1rem;
    border-radius: 0.25rem;
    transition: color 0.15s, background-color 0.15s, border-color 0.15s;
    cursor: pointer;
  }

    .vue-btn:hover {
      background-color: rgba(13, 110, 253, 0.05);
    }

    .vue-btn:focus-visible {
      outline: 2px solid var(--modal-primary);
      outline-offset: 2px;
    }

  .vue-btn--right {
    margin-right: 1rem;
  }

  /* === Контейнер таблицы === */
  .vue-table-container {
    overflow: auto;
    max-height: 60vh;
    margin-top: var(--modal-padding-sm);
  }

  /* === Таблица === */
  .tableModal-table {
    width: 100%;
    color: var(--modal-text);
    vertical-align: top;
    border-collapse: collapse;
    font-family: var(--modal-font-family);
  }

    .tableModal-table th,
    .tableModal-table td {
      border: 1px solid var(--modal-table-border);
      padding: 0.5rem;
      text-align: left;
    }

    .tableModal-table th {
      background-color: var(--modal-header-bg);
      color: #000;
      font-weight: bold;
    }
</style>
