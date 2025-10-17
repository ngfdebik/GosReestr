<!-- src/components/MainTableView.vue -->
<template>
  <div id="tableView" class="table-container">
    <!-- Shared Table -->
    <table v-show="showSharedHeaders" class="tableView-table" id="sharedTable">
      <thead>
        <tr>
          <th v-for="header in sharedHeaders" :key="header" class="table-header">
            <div class="header-content">
              <span class="header-text">{{ header }}</span>
              <span v-if="header !== 'ДопИНФ'"
                    ref="filterIcons"
                    :data-column="header"
                    :data-table="'shared'"
                    class="filter-icon"
                    @click.stop="toggleFilter($event, header, 'shared')">
                ⬇️
              </span>
            </div>
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="row in sharedIPRows" :key="'ip-'+row.idЛицо">
          <td v-for="field in sharedFields" :key="field" :data-t="field === 'ДатаОГРН' || field === 'ДатаВып' ? 'd' : 's'">
            {{ field.includes('Дата') ? formatDate(row[field]) : row[field] }}
          </td>
          <td data-exclude="true">
            <button @click="openModal(row, 'IP')" class="detail-button">Подробнее</button>
          </td>
        </tr>
        <tr v-for="row in sharedULRows" :key="'ul-'+row.idЛицо">
          <td v-for="field in sharedFields" :key="field" :data-t="field === 'ДатаОГРН' || field === 'ДатаВып' ? 'd' : 's'">
            {{ field.includes('Дата') ? formatDate(row[field]) : row[field] }}
          </td>
          <td data-exclude="true">
            <button @click="openModal(row, 'UL')" class="detail-button">Подробнее</button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- IP Table -->
    <table v-show="showIPHeaders" class="tableView-table" id="IPTable">
      <thead>
        <tr>
          <th v-for="header in ipHeaders" :key="header" class="table-header">
            <div class="header-content">
              <span class="header-text">{{ header }}</span>
              <span v-if="header !== 'ДопИНФ'"
                    ref="filterIcons"
                    :data-column="header"
                    :data-table="'ip'"
                    class="filter-icon"
                    @click.stop="toggleFilter($event, header, 'ip')">
                ⬇️
              </span>
            </div>
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="row in ipRows" :key="row.idЛицо">
          <td v-for="field in ipFields" :key="field" :data-t="field.includes('Дата') ? 'd' : 's'">
            {{ field.includes('Дата') ? formatDate(row[field]) : row[field] }}
          </td>
          <td data-exclude="true">
            <button @click="openModal(row, 'IP')" class="detail-button">Подробнее</button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- UL Table -->
    <table v-show="showULHeaders" class="tableView-table" id="ULTable">
      <thead>
        <tr>
          <th v-for="header in ulHeaders" :key="header" class="table-header">
            <div class="header-content">
              <span class="header-text">{{ header }}</span>
              <span v-if="header !== 'ДопИНФ'"
                    ref="filterIcons"
                    :data-column="header"
                    :data-table="'ul'"
                    class="filter-icon"
                    @click.stop="toggleFilter($event, header, 'ul')">
                ⬇️
              </span>
            </div>
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="row in ulRows" :key="row.idЛицо">
          <td v-for="field in ulFields" :key="field" :data-t="field.includes('Дата') ? 'd' : 's'">
            {{ field.includes('Дата') ? formatDate(row[field]) : row[field] }}
          </td>
          <td data-exclude="true">
            <button @click="openModal(row, 'UL')" class="detail-button">Подробнее</button>
          </td>
        </tr>
      </tbody>
      <tfoot v-if="showLoadMore">
        <tr>
          <td :colspan="ulHeaders.length" class="load-more-cell">
            <button @click="$emit('load-more')" class="load-more-button">Загрузить ещё</button>
          </td>
        </tr>
      </tfoot>
    </table>
  </div>

  <!-- Teleport для Shared Filter -->
  <Teleport to="body" v-if="activeFilterColumn && activeFilterTable === 'shared'">
    <div class="filter-dropdown" :style="filterStyle">
      <select v-model="tempFilter.mode" class="filter-input">
        <option value="=">Равно</option>
        <option value="!=">Не равно</option>
        <option v-if="!activeFilterColumn.includes('Дата')" value="содержит">Содержит</option>
        <option v-if="!activeFilterColumn.includes('Дата')" value="начинается с">Начинается с</option>
        <option v-if="!activeFilterColumn.includes('Дата')" value="заканчивается на">Заканчивается на</option>
        <option v-if="activeFilterColumn.includes('Дата')" value=">">Больше (дата)</option>
        <option v-if="activeFilterColumn.includes('Дата')" value="<">Меньше (дата)</option>
      </select>
      <template v-if="isDateColumn(activeFilterColumn)">
        <input type="date" v-model="tempFilter.value" class="filter-input" />
      </template>
      <template v-else>
        <input v-model="tempFilter.value" placeholder="Значение" class="filter-input" />
      </template>
      <button @click="applyColumnFilter(activeFilterColumn)" class="filter-button">Применить</button>
    </div>
  </Teleport>

  <!-- Teleport для IP Filter -->
  <Teleport to="body" v-if="activeFilterColumn && activeFilterTable === 'ip'">
    <div class="filter-dropdown" :style="filterStyle">
      <select v-model="tempFilter.mode" class="filter-input">
        <option value="=">Равно</option>
        <option value="!=">Не равно</option>
        <option v-if="!activeFilterColumn.includes('Дата')" value="содержит">Содержит</option>
        <option v-if="!activeFilterColumn.includes('Дата')" value="начинается с">Начинается с</option>
        <option v-if="!activeFilterColumn.includes('Дата')" value="заканчивается на">Заканчивается на</option>
        <option v-if="activeFilterColumn.includes('Дата')" value=">">Больше (дата)</option>
        <option v-if="activeFilterColumn.includes('Дата')" value="<">Меньше (дата)</option>
      </select>
      <template v-if="isDateColumn(activeFilterColumn)">
        <input type="date" v-model="tempFilter.value" class="filter-input" />
      </template>
      <template v-else>
        <input v-model="tempFilter.value" placeholder="Значение" class="filter-input" />
      </template>
      <button @click="applyColumnFilter(activeFilterColumn)" class="filter-button">Применить</button>
    </div>
  </Teleport>

  <!-- Teleport для UL Filter -->
  <Teleport to="body" v-if="activeFilterColumn && activeFilterTable === 'ul'">
    <div class="filter-dropdown" :style="filterStyle">
      <select v-model="tempFilter.mode" class="filter-input">
        <option value="=">Равно</option>
        <option value="!=">Не равно</option>
        <option v-if="!activeFilterColumn.includes('Дата')" value="содержит">Содержит</option>
        <option v-if="!activeFilterColumn.includes('Дата')" value="начинается с">Начинается с</option>
        <option v-if="!activeFilterColumn.includes('Дата')" value="заканчивается на">Заканчивается на</option>
        <option v-if="activeFilterColumn.includes('Дата')" value=">">Больше (дата)</option>
        <option v-if="activeFilterColumn.includes('Дата')" value="<">Меньше (дата)</option>
      </select>
      <template v-if="isDateColumn(activeFilterColumn)">
        <input type="date" v-model="tempFilter.value" class="filter-input" />
      </template>
      <template v-else>
        <input v-model="tempFilter.value" placeholder="Значение" class="filter-input" />
      </template>
      <button @click="applyColumnFilter(activeFilterColumn)" class="filter-button">Применить</button>
    </div>
  </Teleport>
</template>

<script>
  export default {
    name: 'MainTableView',
    props: {
      showSharedHeaders: Boolean,
      showIPHeaders: Boolean,
      showULHeaders: Boolean,
      sharedIPRows: Array,
      sharedULRows: Array,
      ipRows: Array,
      ulRows: Array,
      showLoadMore: Boolean,
      filters: Array,
    },
    emits: ['open-modal', 'load-more', 'update:filters', 'apply-filters'],
    data() {
      return {
        activeFilterColumn: null,
        activeFilterTable: null,
        tempFilter: {
          mode: '=',
          value: ''
        },
        filterStyle: {
          position: 'absolute',
          top: '0px',
          left: '0px',
          zIndex: 1000
        }
      };
    },
    computed: {
      sharedHeaders() {
        return ['ИНН', 'ОГРН', 'ДатаОГРН', 'НаимСокр', 'ОКВЭДОсн', 'ДатаВып', 'ДопИНФ'];
      },
      ipHeaders() {
        return ['ИНН', 'ОГРНИП', 'ДатаОГРНИП', 'НаимВидИП', 'НаимСокр', 'ОКВЭДОсн', 'ДатаВып', 'КодВидИП', 'ДопИНФ'];
      },
      ulHeaders() {
        return ['ИНН', 'КПП', 'ОГРН', 'ДатаОГРН', 'ПолнНаимОПФ', 'НаимСокр', 'ОКВЭДОсн', 'СпрОПФ', 'КодОПФ', 'ДатаВып', 'ДопИНФ'];
      },
      sharedFields() {
        return ['ИНН', 'ОГРН', 'ДатаОГРН', 'НаимСокр', 'ОКВЭДОсн', 'ДатаВып'];
      },
      ipFields() {
        return ['ИНН', 'ОГРН', 'ДатаОГРН', 'НаимВидИП', 'НаимСокр', 'ОКВЭДОсн', 'ДатаВып', 'КодВидИП'];
      },
      ulFields() {
        return ['ИНН', 'КПП', 'ОГРН', 'ДатаОГРН', 'ПолнНаимОПФ', 'НаимСокр', 'ОКВЭДОсн', 'СпрОПФ', 'КодОПФ', 'ДатаВып'];
      }
    },
    methods: {
      formatDate(dateString) {
        return dateString ? dateString.split('T')[0].split("-").reverse().join(".") : '';
      },
      openModal(row, type) {
        this.$emit('open-modal', { row, type });
      },
      isDateColumn(col) {
        return col && col.includes('Дата');
      },
      toggleFilter(event, column, tableType) {
        event.stopPropagation();

        if (this.activeFilterColumn === column && this.activeFilterTable === tableType) {
          this.activeFilterColumn = null;
          this.activeFilterTable = null;
          return;
        }

        // Закрываем предыдущий
        this.activeFilterColumn = null;
        this.activeFilterTable = null;

        this.$nextTick(() => {
          const icon = event.currentTarget;
          if (!icon) return;

          const rect = icon.getBoundingClientRect();
          const dropdownWidth = 220; // Примерная ширина фильтра (можно уточнить)
          const spaceRight = window.innerWidth - rect.right;
          const spaceLeft = rect.left;

          let left = rect.left + window.scrollX;

          // Если недостаточно места справа — прижимаем к правому краю иконки
          if (spaceRight < dropdownWidth) {
            if (spaceLeft > dropdownWidth) {
              // Есть место слева — показываем слева от иконки
              left = rect.right - dropdownWidth + window.scrollX;
            } else {
              // Нет места ни слева, ни справа — прижимаем к левому краю экрана
              left = 10 + window.scrollX;
            }
          }

          this.filterStyle = {
            position: 'absolute',
            top: rect.bottom + window.scrollY + 'px',
            left: left + 'px',
            zIndex: 1000,
            minWidth: '200px',
            maxWidth: '300px' // Ограничение ширины
          };

          this.activeFilterColumn = column;
          this.activeFilterTable = tableType;
          this.tempFilter = { mode: '=', value: '' };
        });
      },
      applyColumnFilter(column) {
        if (!this.tempFilter.value.trim()) {
          this.activeFilterColumn = null;
          this.activeFilterTable = null;
          return;
        }

        const newFilter = {
          id: Date.now(),
          col: column,
          mode: this.tempFilter.mode,
          value: this.tempFilter.value
        };

        const updatedFilters = [...this.filters, newFilter];
        this.$emit('update:filters', updatedFilters);
        this.$emit('apply-filters');

        this.activeFilterColumn = null;
        this.activeFilterTable = null;
      }
    }
  };
</script>

<style scoped>
  /* ... остальной CSS без изменений ... */
  .table-container {
    height: 80vh;
    position: relative;
    border: 1px solid #212529;
    overflow: auto;
  }

  .tableView-table {
    width: 100%;
    border-collapse: collapse;
    color: #212529;
    vertical-align: top;
    margin: 0;
    --table-striped-bg: rgba(0, 0, 0, 0.05);
    --table-active-bg: rgba(0, 0, 0, 0.1);
    --table-hover-bg: rgba(0, 0, 0, 0.075);
    --modal-header-bg: #e2e3e5;
  }

  .table-header {
    position: sticky;
    top: 0;
    padding: 8px;
    border: 1px solid #212529;
    background-color: #e2e3e5;
    color: #000;
    font-family: "Times New Roman", serif;
    font-weight: normal;
    /* z-index не обязателен при использовании Teleport, но оставим для надёжности */
    z-index: 10;
  }

  .tableView-table td {
    padding: 0.5rem;
    border: 1px solid #212529;
  }

  .tableView-table th {
    padding: 0.5rem;
    border: 1px solid #212529;
    background-color: var(--modal-header-bg);
    color: #000;
    font-weight: bold;
  }

  .tableView-table tbody tr:nth-child(odd) {
    background-color: var(--table-striped-bg);
  }

  .tableView-table tbody tr:hover {
    background-color: var(--table-hover-bg);
  }

  .load-more-cell {
    text-align: center;
    padding: 10px !important;
    background: #f8f9fa;
    border-top: 2px solid #dee2e6;
  }

  .load-more-button {
    display: block;
    width: 100%;
    padding: 0.75rem 1.5rem;
    font-size: 1rem;
    font-weight: 500;
    line-height: 1.5;
    color: #0d6efd;
    text-align: center;
    text-decoration: none;
    cursor: pointer;
    background-color: transparent;
    border: 2px solid #0d6efd;
    border-radius: 0.375rem;
    transition: all 0.2s ease-in-out;
  }

    .load-more-button:hover {
      color: #fff;
      background-color: #0d6efd;
      border-color: #0d6efd;
      transform: translateY(-1px);
      box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    }

    .load-more-button:active {
      transform: translateY(0);
    }

  .detail-button {
    padding: 0.25rem 0.5rem;
    font-size: 0.875rem;
    border: 1px solid #6c757d;
    border-radius: 0.2rem;
    background: transparent;
    color: #6c757d;
    cursor: pointer;
    transition: all 0.15s ease-in-out;
  }

    .detail-button:hover {
      background-color: #6c757d;
      color: #fff;
    }

  .header-content {
    display: flex;
    align-items: center;
    justify-content: space-between;
    white-space: nowrap;
    gap: 4px;
  }

  .header-text {
    overflow: hidden;
    text-overflow: ellipsis;
    flex: 1;
  }

  .filter-icon {
    cursor: pointer;
    font-size: 0.75rem;
    color: #6c757d;
    flex-shrink: 0; /* Не сжимать иконку */
  }

  /* Стили для выпадающего меню (теперь в body) */
  .filter-dropdown {
    position: absolute;
    background: #fff;
    border: 1px solid #ccc;
    border-radius: 0.25rem;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.2);
    padding: 0.5rem;
    min-width: 200px;
    max-width: 300px; /* ← добавьте это */
    width: max-content;/**/
    box-sizing: border-box;/**/
    font-size: 0.875rem;
    z-index: 1000;
  }

  .filter-input {
    width: 100%;
    margin-bottom: 0.25rem;
    padding: 0.25rem;
    border: 1px solid #ced4da;
    border-radius: 0.2rem;
    font-size: 0.875rem;
  }

  .filter-button {
    width: 100%;
    padding: 0.375rem;
    font-size: 0.875rem;
    background-color: #6c757d;
    color: #fff;
    border: none;
    border-radius: 0.2rem;
    cursor: pointer;
    transition: background-color 0.15s ease-in-out;
  }

    .filter-button:hover {
      background-color: #5a6268;
    }
</style>
