<!-- src/components/FiltersPanel.vue -->
<template>
  <div class="border border-3 mt-2">
    <p class="lead m-0 text-center">Фильтры и параметры</p>
    <div class="border m-2">
      <div class="container" style="border-top-width: 1px; border-top-style: solid;">
        <div class="row mt-2 mb-2">
          <div class="row gx-1 mb-1">
            <div class="col-4">
              <select v-model="localFilterColumn" style="width:100%">
                <option disabled value="">Столбец</option>
                <option value="ИНН">ИНН</option>
                <option value="ДатаВып">ДатаВып</option>
                <option value="НаимСокр">НаимСокр</option>
                <option value="ОКВЭДОсн">ОКВЭДОсн</option>
                <option value="ОГРНИП" v-show="showIPHeaders">ОГРНИП</option>
                <option value="ДатаОГРНИП" v-show="showIPHeaders">ДатаОГРНИП</option>
                <option value="НаимВидИП" v-show="showIPHeaders">НаимВидИП</option>
                <option value="КодВидИП" v-show="showIPHeaders">КодВидИП</option>
                <option value="КПП" v-show="showULHeaders">КПП</option>
                <option value="ОГРН" v-show="showULHeaders || showSharedHeaders">ОГРН</option>
                <option value="ДатаОГРН" v-show="showULHeaders || showSharedHeaders">ДатаОГРН</option>
                <option value="ПолнНаимОПФ" v-show="showULHeaders">ПолнНаимОПФ</option>
                <option value="СпрОПФ" v-show="showULHeaders">СпрОПФ</option>
                <option value="КодОПФ" v-show="showULHeaders">КодОПФ</option>
              </select>
            </div>
            <div class="col-4">
              <select v-model="localFilterMode" style="width:100%">
                <option value="=">Равно</option>
                <option value="!=">Не равно</option>
                <option value="содержит">Содержит</option>
                <option value="начинается с">Начинается с</option>
                <option value="заканчивается на">Заканчивается на</option>
                <option value=">">Больше (Даты ГГГГ-ММ-ДД)</option>
                <option value="<">Меньше (Даты ГГГГ-ММ-ДД)</option>
              </select>
            </div>
            <div class="col-4">
              <input v-model="localFilterValue" placeholder="Значение" style="width:100%; height:100%" />
            </div>
          </div>
          <button @click="addFilter" class="mb-1">Добавить фильтр</button>
          <button @click="applyFilters" class="mb-1">Применить фильтры</button>
        </div>
        <div>
          <table class="mb-1">
            <tr v-for="filter in filters" :key="filter.id">
              <td>{{ filter.col }}</td>
              <td>{{ filter.mode }}</td>
              <td>{{ filter.value }}</td>
              <td><button type="button" class="btn-close" @click="deleteFilter(filter.id)"></button></td>
            </tr>
          </table>
          <button v-if="filters.length > 0" @click="clearFilters" class="mb-2">Очистить фильтры</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'FiltersPanel',
    props: {
      filters: Array,
      showIPHeaders: Boolean,
      showULHeaders: Boolean,
      showSharedHeaders: Boolean,
    },
    emits: ['update:filters', 'apply-filters'],
    data() {
      return {
        localFilterColumn: '',
        localFilterMode: '',
        localFilterValue: ''
      };
    },
    methods: {
      addFilter() {
        if (!this.localFilterColumn || !this.localFilterMode || !this.localFilterValue) return;
        const newFilter = {
          id: Date.now(),
          col: this.localFilterColumn,
          mode: this.localFilterMode,
          value: this.localFilterValue
        };
        this.$emit('update:filters', [...this.filters, newFilter]);
        this.localFilterColumn = '';
        this.localFilterMode = '';
        this.localFilterValue = '';
      },
      deleteFilter(id) {
        this.$emit('update:filters', this.filters.filter(f => f.id !== id));
      },
      clearFilters() {
        this.$emit('update:filters', []);
      },
      applyFilters() {
        this.$emit('apply-filters');
      }
    }
  };
</script>
