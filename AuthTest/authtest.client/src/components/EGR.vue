<template>
  <div class="m-3" style="margin-bottom:1%">
    <div class="row pb-2">
      <div class="col-9 pe-0">
        <div id="tableView" class="border border-1 border-dark overflow-auto" style="height:80vh">
          <!-- IP Table -->
          <table v-if="showIPHeaders" class="table table-hover table-bordered border-dark m-0" id="IPTable">
            <thead>
              <tr>
                <th v-for="header in ipHeaders" :key="header" class="table-secondary border-dark" style="font-family:'Times New Roman'; position: sticky; top: 0;">
                  {{ header }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="row in IPTableContent" :key="row.idЛицо">
                <td data-t="s" style="font-family:'Times New Roman';"> {{ row.ИНН }} </td>
                <td data-t="s"> {{ row.ОГРН }} </td>
                <td data-t="d"> {{ formatDate(row.ДатаОГРН) }} </td>
                <td data-t="s"> {{ row.НаимВидИП }} </td>
                <td data-t="s"> {{ row.НаимСокр }} </td>
                <td data-t="s"> {{ row.ОКВЭДОсн }} </td>
                <td data-t="d"> {{ formatDate(row.ДатаВып) }} </td>
                <td data-t="s"> {{ row.КодВидИП }} </td>
                <td data-exclude="true">
                  <button @click="OpenModal(row.idЛицо, row.ИНН, 'IP', row.НаимСокр)" class="btn btn-sm btn-outline-primary">
                    Подробнее
                  </button>
                </td>
              </tr>
            </tbody>
          </table>

          <!-- UL Table -->
          <table v-if="showULHeaders" class="table table-hover table-bordered border-dark m-0" id="ULTable">
            <thead>
              <tr>
                <th v-for="header in ulHeaders" :key="header" class="table-secondary border-dark" style="font-family:'Times New Roman'; position: sticky; top: 0;">
                  {{ header }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="row in ULTableContent" :key="row.idЛицо">
                <td> {{ row.ИНН }} </td>
                <td> {{ row.КПП }} </td>
                <td> {{ row.ОГРН }} </td>
                <td> {{ formatDate(row.ДатаОГРН) }} </td>
                <td> {{ row.ПолнНаимОПФ }} </td>
                <td> {{ row.НаимСокр }} </td>
                <td> {{ row.ОКВЭДОсн }} </td>
                <td> {{ row.СпрОПФ }} </td>
                <td> {{ row.КодОПФ }} </td>
                <td> {{ formatDate(row.ДатаВып) }} </td>
                <td>
                  <button @click="OpenModal(row.idЛицо, row.ИНН, 'UL', row.НаимСокр)" class="btn btn-sm btn-outline-primary">
                    Подробнее
                  </button>
                </td>
              </tr>
            </tbody>
          </table>

          <!-- Shared Table -->
          <table v-if="showSharedHeaders" class="table table-hover table-bordered border-dark m-0" id="sharedTable">
            <thead>
              <tr>
                <th v-for="header in sharedHeaders" :key="header" class="table-secondary border-dark" style="font-family:'Times New Roman'; position: sticky; top: 0;">
                  {{ header }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="row in sharedULTableContent" :key="row.idЛицо">
                <td> {{ row.ИНН }} </td>
                <td> {{ row.ОГРН }} </td>
                <td> {{ formatDate(row.ДатаОГРН) }} </td>
                <td> {{ row.НаимСокр }} </td>
                <td> {{ row.ОКВЭДОсн }} </td>
                <td> {{ formatDate(row.ДатаВып) }} </td>
                <td>
                  <button @click="OpenModal(row.idЛицо, row.ИНН, 'UL', row.НаимСокр)" class="btn btn-sm btn-outline-primary">
                    Подробнее
                  </button>
                </td>
              </tr>
              <tr v-for="row in sharedIPTableContent" :key="row.idЛицо">
                <td> {{ row.ИНН }} </td>
                <td> {{ row.ОГРН }} </td>
                <td> {{ formatDate(row.ДатаОГРН) }} </td>
                <td> {{ row.НаимСокр }} </td>
                <td> {{ row.ОКВЭДОсн }} </td>
                <td> {{ formatDate(row.ДатаВып) }} </td>
                <td>
                  <button @click="OpenModal(row.idЛицо, row.ИНН, 'IP', row.НаимСокр)" class="btn btn-sm btn-outline-primary">
                    Подробнее
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Filters -->
      <div class="col-3">
        <div class="border border-3 mt-2">
          <p class="lead m-0 text-center">Фильтры и параметры</p>
          <div class="border m-2">
            <div class="container">
              <div class="row mt-2 mb-2">
                <div class="row gx-1 mb-1">
                  <div class="col-4">
                    <select v-model="filterColumn" class="form-select">
                      <option disabled value="">Столбец</option>
                      <option value="ИНН">ИНН</option>
                      <option value="ДатаВып">ДатаВып</option>
                      <option value="НаимСокр">НаимСокр</option>
                      <option value="ОКВЭДОсн">ОКВЭДОсн</option>
                      <option value="ОГРНИП" v-if="showIPHeaders">ОГРНИП</option>
                      <option value="ДатаОГРНИП" v-if="showIPHeaders">ДатаОГРНИП</option>
                      <option value="НаимВидИП" v-if="showIPHeaders">НаимВидИП</option>
                      <option value="КодВидИП" v-if="showIPHeaders">КодВидИП</option>
                      <option value="КПП" v-if="showULHeaders">КПП</option>
                      <option value="ОГРН" v-if="showULHeaders || showSharedHeaders">ОГРН</option>
                      <option value="ДатаОГРН" v-if="showULHeaders || showSharedHeaders">ДатаОГРН</option>
                      <option value="ПолнНаимОПФ" v-if="showULHeaders">ПолнНаимОПФ</option>
                      <option value="СпрОПФ" v-if="showULHeaders">СпрОПФ</option>
                      <option value="КодОПФ" v-if="showULHeaders">КодОПФ</option>
                    </select>
                  </div>
                  <div class="col-4">
                    <select v-model="filterMode" class="form-select">
                      <option value="=">Равно</option>
                      <option value="!=">Не равно</option>
                      <option value="содержит">Содержит</option>
                      <option value="начинается с">Начинается с</option>
                      <option value="заканчивается на">Заканчивается на</option>
                      <option value=">">Больше</option>
                      <option value="<">Меньше</option>
                    </select>
                  </div>
                  <div class="col-4">
                    <input v-model="filterValue" placeholder="Значение" class="form-control" />
                  </div>
                </div>
                <button @click="AddFilter" class="btn btn-sm btn-primary mb-1">Добавить фильтр</button>
                <button @click="FilterTable" class="btn btn-sm btn-success mb-1">Применить фильтры</button>
              </div>

              <div v-if="filters.length > 0" class="mt-2">
                <table class="table table-sm">
                  <tr v-for="(filter, index) in filters" :key="index">
                    <td>{{ filter.col }}</td>
                    <td>{{ filter.mode }}</td>
                    <td>{{ filter.value }}</td>
                    <td><button type="button" class="btn-close" @click="DeleteFilter(index)"></button></td>
                  </tr>
                </table>
                <button @click="ClearFilters" class="btn btn-sm btn-outline-danger">Очистить фильтры</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- Modal -->
  <div class="modal fade" id="detailsModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-xl">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Дополнительная информация</h5>
          <h3 class="lead">{{ detailsEntityName }}</h3>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Закрыть"></button>
        </div>
        <div class="modal-body">
          <div class="mb-3">
            <select v-model="selectedDetail" class="form-select">
              <option disabled value="">Выберите раздел</option>
              <option v-for="opt in detailOptions" :key="opt.value" :value="opt.value" v-if="opt.show">
                {{ opt.label }}
              </option>
            </select>
            <div class="mt-2">
              <button class="btn btn-outline-primary me-2" @click="GetDetailsData">Показать</button>
              <button class="btn btn-outline-secondary" @click="GetLogsData">Показать Историю</button>
            </div>
          </div>

          <div class="Tableframe" style="overflow: auto;">
            <table v-if="detailsTableData.length > 0" class="table table-hover table-bordered">
              <thead>
                <tr>
                  <th v-for="header in detailsTableHeaders" :key="header" class="table-secondary">
                    {{ header }}
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(row, index) in trimmedDetailsTableData" :key="index">
                  <td v-for="(cell, idx) in row" :key="idx">{{ cell }}</td>
                </tr>
              </tbody>
            </table>
            <h5 v-else class="text-muted">Нет данных для отображения</h5>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import { ref, computed } from 'vue';
  import * as bootstrap from 'bootstrap';

  export default {
    name: 'EGRView',
    setup() {
      const IPTableContent = ref([]);
      const ULTableContent = ref([]);
      const sharedULTableContent = ref([]);
      const sharedIPTableContent = ref([]);

      const showIPHeaders = ref(true);
      const showULHeaders = ref(true);
      const showSharedHeaders = ref(false);

      const filterColumn = ref('');
      const filterMode = ref('=');
      const filterValue = ref('');
      const filters = ref([]);

      const entityID = ref(null);
      const entityINN = ref(null);
      const entityType = ref('');
      const detailsEntityName = ref('');
      const selectedDetail = ref('');
      const detailsTableData = ref([]);
      const detailsTableHeaders = ref([]);
      const trimmedDetailsTableData = ref([]);

      const modal = ref(null);

      const ipHeaders = [
        'ИНН', 'ОГРНИП', 'ДатаОГРНИП', 'НаимВидИП', 'НаимСокр',
        'ОКВЭДОсн', 'ДатаВып', 'КодВидИП', 'ДопИНФ'
      ];

      const ulHeaders = [
        'ИНН', 'КПП', 'ОГРН', 'ДатаОГРН', 'ПолнНаимОПФ', 'НаимСокр',
        'ОКВЭДОсн', 'СпрОПФ', 'КодОПФ', 'ДатаВып', 'ДопИНФ'
      ];

      const sharedHeaders = [
        'ИНН', 'ОГРН', 'ДатаОГРН', 'НаимСокр', 'ОКВЭДОсн', 'ДатаВып', 'ДопИНФ'
      ];

      const detailOptions = computed(() => [
        { value: 'EGRIPOKVED', label: 'ОКВЭД', show: entityType.value.includes('IP') },
        { value: 'EGRIPSvAdrMJ', label: 'АдресМЖ', show: entityType.value.includes('IP') },
        // ... остальные опции UL и IP
        { value: 'EGRULOKVED', label: 'ОКВЭД', show: entityType.value.includes('UL') },
        { value: 'EGRULSvAddressUL', label: 'АдресЮЛ', show: entityType.value.includes('UL') },
        // ... и так далее
      ]);

      const formatDate = (dateString) => {
        if (!dateString) return '';
        return dateString.split('T')[0];
      };

      const OpenModal = (id, inn, type, name) => {
        entityID.value = id;
        entityINN.value = inn;
        entityType.value = type;
        detailsEntityName.value = name;
        selectedDetail.value = '';
        detailsTableData.value = [];
        detailsTableHeaders.value = [];
        trimmedDetailsTableData.value = [];

        if (!modal.value) {
          modal.value = new bootstrap.Modal(document.getElementById('detailsModal'));
        }
        modal.value.show();
      };

      const GetDetailsData = async () => {
        if (!selectedDetail.value || !entityID.value) return;

        try {
          const response = await fetch(`/api/details/${selectedDetail.value}/${entityID.value}`);
          const data = await response.json();
          detailsTableData.value = data.rows || [];
          detailsTableHeaders.value = data.headers || [];
          trimmedDetailsTableData.value = detailsTableData.value.map(row => Object.values(row));
        } catch (err) {
          console.error('Ошибка загрузки данных:', err);
        }
      };

      const GetLogsData = async () => {
        if (!selectedDetail.value || !entityINN.value) return;

        try {
          const response = await fetch(`/api/logs/${selectedDetail.value}/${entityINN.value}`);
          const data = await response.json();
          detailsTableData.value = data.rows || [];
          detailsTableHeaders.value = data.headers || [];
          trimmedDetailsTableData.value = detailsTableData.value.map(row => Object.values(row));
        } catch (err) {
          console.error('Ошибка загрузки истории:', err);
        }
      };

      const AddFilter = () => {
        if (!filterColumn.value || !filterValue.value) return;
        filters.value.push({
          id: Date.now(),
          col: filterColumn.value,
          mode: filterMode.value,
          value: filterValue.value
        });
        filterColumn.value = '';
        filterMode.value = '=';
        filterValue.value = '';
      };

      const DeleteFilter = (index) => {
        filters.value.splice(index, 1);
      };

      const ClearFilters = () => {
        filters.value = [];
      };

      const FilterTable = () => {
        console.log('Применяем фильтры:', filters.value);
        // Здесь можно вызвать API с параметрами фильтров
      };

      // Загрузка данных при монтировании
      const loadData = async () => {
        try {
          const [ipRes, ulRes] = await Promise.all([
            fetch('/api/egr/ip').then(r => r.json()),
            fetch('/api/egr/ul').then(r => r.json())
          ]);
          IPTableContent.value = ipRes;
          ULTableContent.value = ulRes;
        } catch (err) {
          console.error('Ошибка загрузки таблиц:', err);
        }
      };

      loadData();

      return {

        IPTableContent,
        ULTableContent,
        sharedULTableContent,
        sharedIPTableContent,
        showIPHeaders,
        showULHeaders,
        showSharedHeaders,
        filterColumn,
        filterMode,
        filterValue,
        filters,
        entityID,
        entityINN,
        entityType,
        detailsEntityName,
        selectedDetail,
        detailsTableData,
        detailsTableHeaders,
        trimmedDetailsTableData,
        ipHeaders,
        ulHeaders,
        sharedHeaders,
        detailOptions,
        formatDate,
        OpenModal,
        GetDetailsData,
        GetLogsData,
        AddFilter,
        DeleteFilter,
        ClearFilters,
        FilterTable
      };
    }
  };
</script>
