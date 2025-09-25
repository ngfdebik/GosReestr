<!-- src/components/MainTableView.vue -->
<template>
  <div id="tableView" class="border border-1 border-dark overflow-auto" style="height:80vh">
    <!-- Shared Table -->
    <table v-show="showSharedHeaders" class="table table-hover table-bordered border-dark m-0" id="sharedTable">
      <thead>
        <tr>
          <th v-for="header in sharedHeaders" :key="header" class="table-secondary border-dark" style="font-family:'Times New Roman'; position: sticky; top: 0;">
            {{ header }}
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
          <th v-for="header in ipHeaders" :key="header" class="table-secondary border-dark" style="font-family:'Times New Roman'; position: sticky; top: 0;">
            {{ header }}
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
          <th v-for="header in ulHeaders" :key="header" class="table-secondary border-dark" style="font-family:'Times New Roman'; position: sticky; top: 0;">
            {{ header }}
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
    },
    emits: ['open-modal'],
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
      }
    }
  };
</script>
