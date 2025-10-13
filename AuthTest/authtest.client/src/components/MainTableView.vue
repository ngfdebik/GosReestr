<!-- src/components/MainTableView.vue -->
<template>
  <div id="tableView" class="border border-1 border-dark overflow-auto" style="height:80vh; position: relative;">
    <!-- Shared Table -->
    <table v-show="showSharedHeaders" class="table table-hover table-bordered border-dark m-0" id="sharedTable">
      <thead>
        <tr>
          <th v-for="header in sharedHeaders" :key="header" class="table-secondary border-dark" style="font-family:'Times New Roman'; position: sticky; top: 0; padding: 8px;">
            {{ header }}
            <span v-if="header !== 'ДопИНФ'" class="filter-icon" @click.stop="toggleFilter(header, 'shared')">
              ⬇️
            </span>
            <div v-if="activeFilterColumn === header && activeFilterTable === 'shared'" class="filter-dropdown">
              <select v-model="tempFilter.mode" style="width:100%; margin-bottom:4px;">
                <option value="=">Равно</option>
                <option value="!=">Не равно</option>
                <option value="содержит">Содержит</option>
                <option value="начинается с">Начинается с</option>
                <option value="заканчивается на">Заканчивается на</option>
                <option value=">">Больше (дата)</option>
                <option value="<">Меньше (дата)</option>
              </select>
              <template v-if="isDateColumn(header)">
                <input type="date"
                       v-model="tempFilter.value"
                       style="width:100%; margin-bottom:4px;" />
              </template>
              <template v-else>
                <input v-model="tempFilter.value"
                       placeholder="Значение"
                       style="width:100%; margin-bottom:4px;" />
              </template>
              <button @click="applyColumnFilter(header)" style="width:100%;">Применить</button>
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
            <button @click="openModal(row, 'IP')">Подробнее</button>
          </td>
        </tr>
        <tr v-for="row in sharedULRows" :key="'ul-'+row.idЛицо">
          <td v-for="field in sharedFields" :key="field" :data-t="field === 'ДатаОГРН' || field === 'ДатаВып' ? 'd' : 's'">
            {{ field.includes('Дата') ? formatDate(row[field]) : row[field] }}
          </td>
          <td data-exclude="true">
            <button @click="openModal(row, 'UL')">Подробнее</button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- IP Table -->
    <table v-show="showIPHeaders" class="table table-hover table-bordered border-dark m-0" id="IPTable">
      <thead>
        <tr>
          <th v-for="header in ipHeaders" :key="header" class="table-secondary border-dark" style="font-family:'Times New Roman'; position: sticky; top: 0; padding: 8px;">
            {{ header }}
            <span v-if="header !== 'ДопИНФ'" class="filter-icon" @click.stop="toggleFilter(header, 'ip')">
              ⬇️
            </span>
            <div v-if="activeFilterColumn === header && activeFilterTable === 'ip'" class="filter-dropdown">
              <select v-model="tempFilter.mode" style="width:100%; margin-bottom:4px;">
                <option value="=">Равно</option>
                <option value="!=">Не равно</option>
                <option value="содержит">Содержит</option>
                <option value="начинается с">Начинается с</option>
                <option value="заканчивается на">Заканчивается на</option>
                <option value=">">Больше (дата)</option>
                <option value="<">Меньше (дата)</option>
              </select>
              <template v-if="isDateColumn(header)">
                <input type="date"
                       v-model="tempFilter.value"
                       style="width:100%; margin-bottom:4px;" />
              </template>
              <template v-else>
                <input v-model="tempFilter.value"
                       placeholder="Значение"
                       style="width:100%; margin-bottom:4px;" />
              </template>
              <button @click="applyColumnFilter(header)" style="width:100%;">Применить</button>
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
            <button @click="openModal(row, 'IP')">Подробнее</button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- UL Table -->
    <table v-show="showULHeaders" class="table table-hover table-bordered border-dark m-0" id="ULTable">
      <thead>
        <tr>
          <th v-for="header in ulHeaders" :key="header" class="table-secondary border-dark" style="font-family:'Times New Roman'; position: sticky; top: 0; padding: 8px;">
            {{ header }}
            <span v-if="header !== 'ДопИНФ'" class="filter-icon" @click.stop="toggleFilter(header, 'ul')">
              ⬇️
            </span>
            <div v-if="activeFilterColumn === header && activeFilterTable === 'ul'" class="filter-dropdown">
              <select v-model="tempFilter.mode" style="width:100%; margin-bottom:4px;">
                <option value="=">Равно</option>
                <option value="!=">Не равно</option>
                <option value="содержит">Содержит</option>
                <option value="начинается с">Начинается с</option>
                <option value="заканчивается на">Заканчивается на</option>
                <option value=">">Больше (дата)</option>
                <option value="<">Меньше (дата)</option>
              </select>
              <template v-if="isDateColumn(header)">
                <input type="date"
                       v-model="tempFilter.value"
                       style="width:100%; margin-bottom:4px;" />
              </template>
              <template v-else>
                <input v-model="tempFilter.value"
                       placeholder="Значение"
                       style="width:100%; margin-bottom:4px;" />
              </template>
              <button @click="applyColumnFilter(header)" style="width:100%;">Применить</button>
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
            <button @click="openModal(row, 'UL')">Подробнее</button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Кнопка "Загрузить ещё" -->
    <div v-if="showLoadMore" style="text-align: center; padding: 10px; background: #f8f9fa; border-top: 1px solid #dee2e6;">
      <button @click="$emit('load-more')" class="btn btn-outline-primary">Загрузить ещё</button>
    </div>
  </div>
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
      filters: Array, // для управления фильтрами из заголовков
    },
    emits: ['open-modal', 'load-more', 'update:filters', 'apply-filters'],
    data() {
      return {
        activeFilterColumn: null,
        activeFilterTable: null,
        tempFilter: {
          mode: '=',
          value: ''
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
        return dateString ? dateString.split('T')[0] : '';
      },
      openModal(row, type) {
        this.$emit('open-modal', { row, type });
      },
      isDateColumn(col) {
        return col && col.includes('Дата');
      },
      toggleFilter(column, tableType) {
        if (this.activeFilterColumn === column && this.activeFilterTable === tableType) {
          this.activeFilterColumn = null;
          this.activeFilterTable = null;
        } else {
          this.activeFilterColumn = column;
          this.activeFilterTable = tableType;
          this.tempFilter = { mode: '=', value: '' };
        }
      },
      applyColumnFilter(column) {
        if (!this.tempFilter.value) return;

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
  .filter-icon {
    cursor: pointer;
    font-size: 12px;
    margin-left: 6px;
    color: #6c757d;
  }

  .filter-dropdown {
    position: absolute;
    background: white;
    border: 1px solid #ccc;
    box-shadow: 0 2px 6px rgba(0,0,0,0.2);
    padding: 8px;
    z-index: 10;
    min-width: 200px;
    font-size: 14px;
  }
</style>
