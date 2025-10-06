<!-- src/components/layout/EGRLayout.vue -->
<template>
  <div class="egr-layout">
    <!-- user-status="userStatus"-раньше было нужно -->
    <Header :isAdmin="isAdmin"
            :isLoggedIn="isLoggedIn"
            @load-all="LoadAllTable"
            @load-ip="LoadIPTable"
            @load-ul="LoadULTable"
            @export-excel="exportToExcel"
            @export-doc="ExportToDoc" />
    <nav id="paginationShared" v-if="showSharedHeaders">
      <div class="d-flex justify-content-between mb-2">
        <!-- Пагинация -->
        <ul class="pagination justify-content-center flex-wrap">
          <!-- Кнопка "Предыдущая" -->
          <li class="page-item" :class="{ disabled: page <= 1 }">
            <button class="page-link" @click="ChangePageShared(page - 1)" :disabled="page <= 1">
              Предыдущая
            </button>
          </li>

          <!-- Страницы -->
          <li v-for="pageNum in visiblePagesShared"
              :key="pageNum"
              class="page-item"
              :class="{ active: page === pageNum }">
            <button class="page-link" @click="ChangePageShared(pageNum)">
              {{ pageNum }}
            </button>
          </li>

          <!-- Кнопка "Следующая" -->
          <li class="page-item" :class="{ disabled: page >= pagesCountShared }">
            <button class="page-link"
                    @click="ChangePageShared(page + 1)"
                    :disabled="page >= pagesCountShared">
              Следующая
            </button>
          </li>
        </ul>

        <!-- Переход по номеру -->
        <div class="d-flex align-items-center">
          <input type="number"
                 class="form-control me-2"
                 v-model.number="page_number"
                 min="1"
                 :max="pagesCountShared"
                 style="width: 80px" />
          <button type="button" class="btn btn-primary" @click="goToPage('shared')">
            Перейти
          </button>
        </div>
      </div>
    </nav>

    <!-- Для IP -->
    <nav id="paginationIP" v-if="showIPHeaders">
      <div class="d-flex justify-content-between mb-2">
        <ul class="pagination justify-content-center flex-wrap">
          <li class="page-item" :class="{ disabled: page <= 1 }">
            <button class="page-link" @click="ChangePageIP(page - 1)" :disabled="page <= 1">Предыдущая</button>
          </li>
          <li v-for="pageNum in visiblePagesIP"
              :key="pageNum"
              class="page-item"
              :class="{ active: page === pageNum }">
            <button class="page-link" @click="ChangePageIP(pageNum)">{{ pageNum }}</button>
          </li>
          <li class="page-item" :class="{ disabled: page >= pagesCountIP }">
            <button class="page-link" @click="ChangePageIP(page + 1)" :disabled="page >= pagesCountIP">Следующая</button>
          </li>
        </ul>
        <div class="d-flex align-items-center">
          <input type="number" class="form-control me-2" v-model.number="page_number" min="1" :max="pagesCountIP" style="width: 80px" />
          <button type="button" class="btn btn-primary" @click="goToPage('ip')">Перейти</button>
        </div>
      </div>
    </nav>

    <!-- Для UL -->
    <nav id="paginationUL" v-if="showULHeaders">
      <div class="d-flex justify-content-between mb-2">
        <ul class="pagination justify-content-center flex-wrap">
          <li class="page-item" :class="{ disabled: page <= 1 }">
            <button class="page-link" @click="ChangePageUL(page - 1)" :disabled="page <= 1">Предыдущая</button>
          </li>
          <li v-for="pageNum in visiblePagesUL"
              :key="pageNum"
              class="page-item"
              :class="{ active: page === pageNum }">
            <button class="page-link" @click="ChangePageUL(pageNum)">{{ pageNum }}</button>
          </li>
          <li class="page-item" :class="{ disabled: page >= pagesCountUL }">
            <button class="page-link" @click="ChangePageUL(page + 1)" :disabled="page >= pagesCountUL">Следующая</button>
          </li>
        </ul>
        <div class="d-flex align-items-center">
          <input type="number" class="form-control me-2" v-model.number="page_number" min="1" :max="pagesCountUL" style="width: 80px" />
          <button type="button" class="btn btn-primary" @click="goToPage('ul')">Перейти</button>
        </div>
      </div>
    </nav>
    <div class="container-fluid mt-3">
      <div class="row">
        <div class="col-9">
          <MainTableView :show-shared-headers="showSharedHeaders"
                         :show-i-p-headers="showIPHeaders"
                         :show-u-l-headers="showULHeaders"
                         :shared-i-p-rows="SharedTableContent.sharedIPTableContent"
                         :shared-u-l-rows="SharedTableContent.sharedULTableContent"
                         :ip-rows="IPTableContent"
                         :ul-rows="ULTableContent"
                         @open-modal="handleOpenModal" />
        </div>
        <div class="col-3">
          <FileUploadZone @upload-complete="getfileuploadzone" />
          <FiltersPanel :filters="filters"
                        :show-i-p-headers="showIPHeaders"
                        :show-u-l-headers="showULHeaders"
                        :show-shared-headers="showSharedHeaders"
                        @update:filters="filters = $event"
                        @apply-filters="FilterTable" />
        </div>
      </div>
    </div>

    <ModalDetails v-if="isModalOpen"
                  :entity-id="modalEntity.idЛицо"
                  :entity-inn="modalEntity.ИНН"
                  :entity-type="modalEntityType"
                  :entity-name="modalEntity.НаимСокр"
                  :is-visible="isModalOpen"
                  @close="isModalOpen = false"
                  @load-details="handleLoadDetails"
                  @load-logs="handleLoadLogs" />
  </div>
</template>

<script>
  import Header from '@/components/Header.vue';
  import MainTableView from '@/components/MainTableView.vue';
  import FiltersPanel from '@/components/FiltersPanel.vue';
  import ModalDetails from '@/components/ModalDetails.vue';
  import FileUploadZone from '@/components/FileUploadZone.vue';
  import store from '@/auth/store';
  import api from '@/services/TokenApi';
  //const baseURL = import.meta.env.VITE_API_URL || window.location.origin;//Пока непонятно
  // ВСТАВЬТЕ СЮДА ВЕСЬ ВАШ JS-КОД ИЗ site.js (data, methods, computed)
  // Я покажу структуру

  export default {
    name: 'EGRLayout',
    components: {
      Header,
      MainTableView,
      FiltersPanel,
      ModalDetails,
      FileUploadZone
    },
    data() {
      return {
        //здесь основными таблицами зовутся таблицы, отображаемые на главной странице, содержащие столбец "ДопИНФ"

        //для select в модальном окне "Дополнительная информация"
        selectedDetail: "",
        //для select по основным таблицам
        selectedTable: "",
        //данные и шапка таблицы модального окна "Дополнительная информация"
        detailsTableData: [],
        detailsTableHeaders: [],
        selectedDetailTable: "",

        //для хранения idЛицо
        entityID: "",
        //для определения типа лица (ИП, ЮЛ)
        entityType: "",
        //для хранения ИНН
        entityINN: "",

        //шапки для всех основных таблиц
        IPTableHeaders: [],
        ULTableHeaders: [],
        extraTableHeaders: [],

        //данные всех основных таблиц
        IPTableContent: [],
        ULTableContent: [],
        
        SharedTableContent: {
          sharedULTableContent: [],
          sharedIPTableContent: []
        },
        extraTableContent: [],

        //хранилища данных таблиц (используются после очистки фильтров)
        IPTableContent_buffer: [],
        ULTableContent_buffer: [],
        sharedULTableContent_buffer: [],
        sharedIPTableContent_buffer: [],
        extraTableContent_buffer: [],

        showIPHeaders: false,
        showULHeaders: false,
        showSharedHeaders: false,

        //массив id элементов таблиц для экспорта
        exportTableID: [],

        //массив фильтров
        filters: [],
        //переменные для создания объекта фильтра
        filterColumn: "",
        filterMode: "",
        filterValue: "",

        //для пагинации
        pagination_IP_items_total: 0,
        pagination_UL_items_total: 0,
        pagination_All_items_total: 0,
        pagination_items_per_page: 50,
        page: 1,
        pagination_offset: 0,
        pagesCountIP: 0,
        pagesCountUL: 0,
        pagesCountShared: 0,

        //для перехода на страницу
        page_number: 1,
        // ...все ваши данные из site.js...
        userStatus: 'Администратор',
        isModalOpen: false,
        modalEntity: {},
        modalEntityType: '',
        // остальные данные...
      };
    },
    computed: {
      isAdmin() {
        return store.getters.isAdmin;
      },
      isLoggedIn() {
        return store.getters.isLoggedIn;
      },
      trimmedDetailsTableData: function () {
        this.detailsTableData.forEach((element) => {
          delete element.Id;
          delete element.idЛицо;
          delete element.ИП;
          delete element.ЮрЛицо;
        });

        return this.detailsTableData;
      },
      visiblePagesShared() {
        return this.getVisiblePages(this.page, this.pagesCountShared);
      },
      visiblePagesIP() {
        return this.getVisiblePages(this.page, this.pagesCountIP);
      },
      visiblePagesUL() {
        return this.getVisiblePages(this.page, this.pagesCountUL);
      }
    },
    methods: {
      getVisiblePages(current, total) {
        if (total <= 7) {
          return Array.from({ length: total }, (_, i) => i + 1);
        }

        const delta = 2;
        const range = [];
        const left = current - delta;
        const right = current + delta;

        for (let i = Math.max(1, left); i <= Math.min(total, right); i++) {
          range.push(i);
        }

        if (left > 1) {
          range.unshift(1);
          if (left > 2) range.unshift('...');
        }

        if (right < total) {
          range.push(total);
          if (right < total - 1) range.push('...');
        }

        return range;
      },

      goToPage(type) {
        const pageNum = this.page_number;
        if (!pageNum || pageNum < 1) return;

        switch (type) {
          case 'shared':
            if (pageNum <= this.pagesCountShared) this.ChangePageShared(pageNum);
            break;
          case 'ip':
            if (pageNum <= this.pagesCountIP) this.ChangePageIP(pageNum);
            break;
          case 'ul':
            if (pageNum <= this.pagesCountUL) this.ChangePageUL(pageNum);
            break;
        }
      },
      getfileuploadzone() {

      },
      // Все методы из site.js: LoadAllTable, FilterTable, exportToExcel и т.д.
      async GetDetailsData(table, id) {
        //очищаем отображаемую таблицу из ДопИНФ
        this.detailsTableData = [];
        this.detailsTableHeaders = [];

        //получаем данные по выбранной таблице
        var url = baseURL + '/Home/Details?table=' + table + '&id=' + id;
        let response = await fetch(url);
        let tableData = await response.json();
        this.detailsTableData = tableData;

        //удаляем неотображаемые поля
        delete tableData[0].Id;
        delete tableData[0].idЛицо;
        delete tableData[0].ИП;
        delete tableData[0].ЮрЛицо;

        Object.keys(tableData[0]).forEach((element) => {
          this.detailsTableHeaders.push(element);
        })
      },
      async GetLogsData(table, INN) {
        //очищаем отображаемую таблицу из ДопИНФ
        this.detailsTableData = [];
        this.detailsTableHeaders = [];

        //получаем данные по выбранной таблице
        var url = baseURL + '/Home/GetLogs?table=' + table + '&INN=' + INN;
        let response = await fetch(url);
        let tableData = await response.json();
        this.detailsTableData = tableData;

        //удаляем неотображаемое поле
        delete tableData[0].Id;

        Object.keys(tableData[0]).forEach((element) => {
          this.detailsTableHeaders.push(element);
        })
      },
      //TrimTableRow(row) {
      //    displayRow = $.extend(true, {}, row);

      //    delete displayRow.id;
      //    delete displayRow.idЛицо;
      //    delete displayRow.ип;
      //    delete displayRow.юрЛицо;

      //    return displayRow;
      //},
      OpenModal(entityID, entityINN, type, name) {
        this.modalEntity = {
          idЛицо: entityID,
          ИНН: entityINN,
          НаимСокр: name
        };
        this.modalEntityType = type;
        this.isModalOpen = true;

        // Сбросим данные модалки при открытии
        this.detailsTableData = [];
        this.detailsTableHeaders = [];
        this.selectedDetailTable = '';
      },
      async onChangeSelectedTable() {
        if (this.selectedTable == 'default' || this.selectedTable == "") {
          return;
        }

        //очищаем отображаемую основную таблицу
        this.IPTableHeaders = [];
        this.IPTableContent = [];
        this.ULTableHeaders = [];
        this.ULTableContent = [];
        this.showIPHeaders = false;
        this.showULHeaders = false;

        this.extraTableContent = [];
        this.extraTableHeaders = [];
        this.entityType = '';

        //подгружаем выбранную основную таблицу
        var url = baseURL + '/Home/GetExtraTable?table=' + this.selectedTable;
        let response = await fetch(url);
        let tableData = await response.json();

        this.extraTableContent = tableData;

        //удаляем неотображаемые поля
        delete tableData[0].Id;
        delete tableData[0].idЛицо;
        delete tableData[0].ИП;
        delete tableData[0].ЮрЛицо

        Object.keys(tableData[0]).forEach((element) => {
          this.extraTableHeaders.push(element);
        })

        this.extraTableHeaders.push('ДопИнф');

        //сохраняем таблицу в хранилище
        this.extraTableContent_buffer = this.extraTableContent
      },
      async LoadAllTable() {
        this.IPTableHeaders = [];
        this.IPTableContent = [];

        this.ULTableHeaders = [];
        this.ULTableContent = [];

        this.SharedTableContent.sharedULTableContent = [];
        this.SharedTableContent.sharedIPTableContent = [];

        this.extraTableContent = [];
        this.extraTableHeaders = [];

        this.filteredSharedIPTableContent = [];
        this.filteredSharedULTableContent = [];
        this.filteredIPTableContent = [];
        this.filteredULTableContent = [];

        this.IPTableContent_buffer = [];
        this.ULTableContent_buffer = [];
        this.sharedIPTableContent_buffer
        this.sharedULTableContent_buffer

        this.showIPHeaders = false;
        this.showULHeaders = false;
        this.showSharedHeaders = true;

        //this.selectedTable = 'default';

        this.exportTableID = [];
        //this.exportTableID.push('IPTable');
        //this.exportTableID.push('ULTable');
        this.exportTableID.push('sharedTable');
        this.entityType = 'IPUL';
        this.page_number = 1;
        try {
          const response = await api.get('/Home/AllTab', {
            params: { buttonId: 'Alltable' }
          });

          const tableData = response.data;

          // Заполняем IP-таблицу
          for (var i = 1; i <= 6; i++) {
            this.IPTableHeaders.push(Object.keys(tableData.ЕГРИП_СвИП[0])[i]);
          }

          //Object.keys(tableData.егриП_СвИП[0]).forEach((element) => {
          //    this.IPTableHeaders.push(element);
          //})

          tableData.ЕГРИП_СвИП.forEach((entity) => {
            if (entity.idЛицо != null) {
              this.SharedTableContent.sharedIPTableContent.push(entity);
            }
          });

          //Object.keys(tableData.ЕГРЮЛ_СвЮЛ[0]).forEach((element) => {
          //    this.ULTableHeaders.push(element);
          //})

          tableData.ЕГРЮЛ_СвЮЛ.forEach((entity) => {
            if (entity.idЛицо != null) {
              this.SharedTableContent.sharedULTableContent.push(entity);
            }
          });

          this.sharedIPTableContent_buffer = this.filteredSharedIPTableContent = this.SharedTableContent.sharedIPTableContent;
          this.sharedULTableContent_buffer = this.filteredSharedULTableContent = this.SharedTableContent.sharedULTableContent;

          //this.SharedTableContent.sharedIPTableContent = this.sharedIPTableContent_buffer;
          //this.SharedTableContent.sharedULTableContent = this.sharedULTableContent_buffer;
          this.ChangePageShared(1);
          this.pagesCountShared = Math.ceil((this.sharedIPTableContent_buffer.length + this.sharedULTableContent_buffer.length) / this.pagination_items_per_page);
        } catch (error) {
          console.error('Ошибка загрузки данных:', error);
          // Можно показать уведомление пользователю
        }
      },

      async LoadIPTable() {
        this.IPTableHeaders = [];
        this.IPTableContent = [];

        this.ULTableHeaders = [];
        this.ULTableContent = [];

        this.SharedTableContent.sharedIPTableContent = [];
        this.SharedTableContent.sharedULTableContent = [];

        this.extraTableContent = [];
        this.extraTableHeaders = [];

        this.showIPHeaders = true;
        this.showULHeaders = false;
        this.showSharedHeaders = false;

        this.sharedIPTableContent_buffer = [];
        this.sharedULTableContent_buffer = [];
        this.ULTableContent_buffer = [];
        //this.SharedTableContent = [];

        this.filteredSharedIPTableContent = [];
        this.filteredSharedULTableContent = [];
        this.filteredIPTableContent = [];
        this.filteredULTableContent = [];

        this.selectedTable = 'default';

        this.exportTableID = [];
        this.exportTableID.push('IPTable');
        this.entityType = 'IP';
        this.page_number = 1;
        try {
          const response = await api.get('/Home/AllTab', {
            params: { buttonId: 'IPtable' }
          });

          const tableData = response.data;

          // Заполняем IP-таблицу
          this.IPTableContent = tableData.ЕГРИП_СвИП.filter(entity => entity.idЛицо != null);//Id -idЛицо

          // Если нужно — заполняем shared часть (но для IP, возможно, только sharedIP)
          this.SharedTableContent.sharedIPTableContent = [...this.IPTableContent]; // или нужные данные
          this.SharedTableContent.sharedULTableContent = []; // пусто для IP

          // Буферы
          this.IPTableContent_buffer = [...this.IPTableContent];
          this.filteredIPTableContent = [...this.IPTableContent];
          this.ChangePageIP(1);
          this.pagesCountIP = Math.ceil(this.IPTableContent_buffer.length / this.pagination_items_per_page);
        } catch (error) {
          console.error('Ошибка загрузки данных:', error);
          // Можно показать уведомление пользователю
        }
      },

      async LoadULTable() {
        this.IPTableHeaders = [];
        this.IPTableContent = [];

        this.ULTableHeaders = [];
        this.ULTableContent = [];

        this.SharedTableContent.sharedULTableContent = [];
        this.SharedTableContent.sharedIPTableContent = [];

        this.extraTableContent = [];
        this.extraTableHeaders = [];

        this.showIPHeaders = false;
        this.showULHeaders = true;
        this.showSharedHeaders = false;

        this.sharedIPTableContent_buffer = [];
        this.sharedULTableContent_buffer = [];
        this.IPTableContent_buffer = [];

        this.filteredSharedIPTableContent = [];
        this.filteredSharedULTableContent = [];
        this.filteredIPTableContent = [];
        this.filteredULTableContent = [];

        this.selectedTable = 'default';

        this.exportTableID = [];
        this.exportTableID.push('ULTable');
        this.entityType = 'UL';
        this.page_number = 1;
        try {
          const response = await api.get('/Home/AllTab', {
            params: { buttonId: 'ULtable' }
          });

          const tableData = response.data;
          this.ULTableContent = tableData.ЕГРЮЛ_СвЮЛ.filter(entity => entity.idЛицо != null);//Id -idЛицо

          // Если нужно — заполняем shared часть (но для IP, возможно, только sharedIP)
          this.SharedTableContent.sharedIPTableContent = []; // или нужные данные
          this.SharedTableContent.sharedULTableContent = [...this.ULTableContent]; // пусто для IP

          // Буферы
          this.ULTableContent_buffer = [...this.ULTableContent];
          this.filteredULTableContent = [...this.ULTableContent];
          this.ChangePageUL(1);
          this.pagesCountUL = Math.ceil(this.ULTableContent_buffer.length / this.pagination_items_per_page);
        } catch (error) {
          console.error('Ошибка загрузки данных:', error);
          // Можно показать уведомление пользователю
        }
      },

      FilterTable() {
        if (this.filters.length == 0 && (this.sharedIPTableContent_buffer != null || this.sharedIPTableContent_buffer != undefined)) {
          this.filteredSharedIPTableContent = this.sharedIPTableContent_buffer;
        }

        if (this.filters.length == 0 && (this.sharedULTableContent_buffer != null || this.sharedULTableContent_buffer != undefined)) {
          this.filteredSharedULTableContent = this.sharedULTableContent_buffer;
        }

        if (this.filters.length == 0 && (this.IPTableContent_buffer != null || this.IPTableContent_buffer != undefined)) {
          this.filteredIPTableContent = this.IPTableContent_buffer;
        }

        if (this.filters.length == 0 && (this.ULTableContent_buffer != null || this.ULTableContent_buffer != undefined)) {
          this.filteredULTableContent = this.ULTableContent_buffer;
          return;
        }

        //if (this.filters.length == 0 && (this.extraTableContent_buffer != null || this.extraTableContent_buffer != undefined)) {
        //    this.extraTableContent = this.extraTableContent_buffer;
        //    return;
        //}
        this.filteredIPTableContent = this.IPTableContent_buffer;
        this.filteredULTableContent = this.ULTableContent_buffer;

        this.filteredSharedIPTableContent = this.sharedIPTableContent_buffer;
        this.filteredSharedULTableContent = this.sharedULTableContent_buffer;

        //let filteredExtraTableContent;

        this.filters.forEach((filter) => {
          switch (filter.mode) {
            case '=':
              this.filteredIPTableContent = this.filteredIPTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col] == filter.value;
              })

              this.filteredULTableContent = this.filteredULTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col] == filter.value;
              })

              this.filteredSharedIPTableContent = this.filteredSharedIPTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col] == filter.value;
              })

              this.filteredSharedULTableContent = this.filteredSharedULTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col] == filter.value;
              })

              //filteredExtraTableContent = this.extraTableContent.filter(function (row) {
              //    return row[filter.col] == filter.value;
              //})
              break

            case '!=':
              this.filteredIPTableContent = this.filteredIPTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col] != filter.value;
              })

              this.filteredULTableContent = this.filteredULTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col] != filter.value;
              })

              this.filteredSharedIPTableContent = this.filteredSharedIPTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col] != filter.value;
              })

              this.filteredSharedULTableContent = this.filteredSharedULTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col] != filter.value;
              })

              //filteredExtraTableContent = this.extraTableContent.filter(function (row) {
              //    return row[filter.col] != filter.value;
              //})
              break

            case 'содержит':
              this.filteredIPTableContent = this.filteredIPTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col].includes(filter.value);
              })

              this.filteredULTableContent = this.filteredULTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col].includes(filter.value);
              })

              this.filteredSharedIPTableContent = this.filteredSharedIPTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col].includes(filter.value);
              })

              this.filteredSharedULTableContent = this.filteredSharedULTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col].includes(filter.value);
              })

              //filteredExtraTableContent = this.extraTableContent.filter(function (row) {
              //    return row[filter.col].includes(filter.value);
              //})
              break

            case 'начинается с':
              this.filteredIPTableContent = this.filteredIPTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col].startsWith(filter.value);
              })

              this.filteredULTableContent = this.filteredULTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col].startsWith(filter.value);
              })

              this.filteredSharedIPTableContent = this.filteredSharedIPTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col].startsWith(filter.value);
              })

              this.filteredSharedULTableContent = this.filteredSharedULTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col].startsWith(filter.value);
              })

              //filteredExtraTableContent = this.extraTableContent.filter(function (row) {
              //    return row[filter.col].startsWith(filter.value);
              //})
              break

            case 'заканчивается на':
              this.filteredIPTableContent = this.filteredIPTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col].endsWith(filter.value);
              })

              this.filteredULTableContent = this.filteredULTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col].endsWith(filter.value);
              })

              this.filteredSharedIPTableContent = this.filteredSharedIPTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col].endsWith(filter.value);
              })

              this.filteredSharedULTableContent = this.filteredSharedULTableContent.filter(function (row) {
                if (row[filter.col])
                  return row[filter.col].endsWith(filter.value);
              })

              //filteredExtraTableContent = this.extraTableContent.filter(function (row) {
              //    return row[filter.col].endsWith(filter.value);
              //})
              break

            case '>':
              this.filteredIPTableContent = this.filteredIPTableContent.filter(function (row) {
                if (row[filter.col])
                  return Date.parse(row[filter.col]) > Date.parse(filter.value);
              })

              this.filteredULTableContent = this.filteredULTableContent.filter(function (row) {
                if (row[filter.col])
                  return Date.parse(row[filter.col]) > Date.parse(filter.value);
              })

              this.filteredSharedIPTableContent = this.filteredSharedIPTableContent.filter(function (row) {
                if (row[filter.col])
                  return Date.parse(row[filter.col]) > Date.parse(filter.value);
              })

              this.filteredSharedULTableContent = this.filteredSharedULTableContent.filter(function (row) {
                if (row[filter.col])
                  return Date.parse(row[filter.col]) > Date.parse(filter.value);
              })

              //filteredExtraTableContent = this.extraTableContent.filter(function (row) {
              //    return Date.parse(row[filter.col]) > Date.parse(filter.value);
              //})
              break

            case '<':
              this.filteredIPTableContent = this.filteredIPTableContent.filter(function (row) {
                if (row[filter.col])
                  return Date.parse(row[filter.col]) < Date.parse(filter.value);
              })

              this.filteredULTableContent = this.filteredULTableContent.filter(function (row) {
                if (row[filter.col])
                  return Date.parse(row[filter.col]) < Date.parse(filter.value);
              })

              this.filteredSharedIPTableContent = this.filteredSharedIPTableContent.filter(function (row) {
                if (row[filter.col])
                  return Date.parse(row[filter.col]) < Date.parse(filter.value);
              })

              this.filteredSharedULTableContent = this.filteredSharedULTableContent.filter(function (row) {
                if (row[filter.col])
                  return Date.parse(row[filter.col]) < Date.parse(filter.value);
              })

              //filteredExtraTableContent = this.extraTableContent.filter(function (row) {
              //    return Date.parse(row[filter.col]) < Date.parse(filter.value);
              //})
              break
          }

          //this.IPTableContent = filteredIPTableContent;
          //this.ULTableContent = filteredULTableContent;

          //this.SharedTableContent.sharedIPTableContent = filteredSharedIPTableContent;
          //this.SharedTableContent.sharedULTableContent = filteredSharedULTableContent;

          if (this.showSharedHeaders) {
            this.pagesCountShared = Math.ceil((this.filteredSharedIPTableContent.length + this.filteredSharedULTableContent.length) / this.pagination_items_per_page);
            this.ChangePageShared(1);
          }

          if (this.showIPHeaders) {
            this.pagesCountIP = Math.ceil(this.filteredIPTableContent.length / this.pagination_items_per_page);
            this.ChangePageIP(1);
          }

          if (this.showULHeaders) {
            this.pagesCountUL = Math.ceil(this.filteredULTableContent.length / this.pagination_items_per_page);
            this.ChangePageUL(1);
          }

          //this.extraTableContent = filteredExtraTableContent;
        });
      },

      AddFilter() {
        let filter = {
          col: this.filterColumn,
          mode: this.filterMode,
          value: this.filterValue,
          id: this.filters.length - 1
        }
        this.filters.push(filter);
      },
      DeleteFilter(id) {
        this.filters.splice(this.filters.findIndex((filter) => filter.id == id), 1)
      },

      ClearFilters() {
        this.filters = [];
        this.FilterTable();
      },
      //TableToExcel.convert(document.getElementById("ULTable"));
      //var ULTable = document.getElementById("ULTable");
      //var IPTable = document.getElementById("IPTable");

      //book = TableToExcel.tableToBook(ULTable, { sheet: { name: "ЮрЛица" } });
      //TableToExcel.tableToSheet(book, IPTable, { sheet: { name: "ИП" } });
      //TableToExcel.save(book, "ЕГР Экспорт.xlsx")
      exportToExcel() {
        if (!this.exportTableID || !this.exportTableID.length) {
          alert('Нет таблицы для экспорта.');
          return;
        }

        const exportTable = document.getElementById(this.exportTableID[0]);
        if (!exportTable) {
          alert('Таблица с id="' + this.exportTableID[0] + '" не найдена.');
          return;
        }
        const book = TableToExcel.tableToBook(exportTable, { sheet: { name: "Лица" } });
        TableToExcel.save(book, "ЕГР Экспорт.xlsx");
      },

      //ExportToDocOLD(filename = '') {
      //    var header = "<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'><head><meta charset='utf-8'><title>Export HTML to Word Document with JavaScript</title></head><body style=\"font-family: serif;\">";

      //    var footer = "</body></html>";

      //    var html = header;
      //    html += document.getElementById('tableView').innerHTML;

      //    html += footer;

      //    var blob = new Blob(['\ufeff', html], {
      //        type: 'application/msword'
      //    });

      //    // Specify link url
      //    var url = 'data:application/vnd.ms-word;charset=utf-8,' + encodeURIComponent(html);

      //    // Specify file name
      //    filename = filename ? filename + '.docx' : 'document.docx';

      //    // Create download link element
      //    var downloadLink = document.createElement("a");

      //    document.body.appendChild(downloadLink);

      //    if (navigator.msSaveOrOpenBlob) {
      //        navigator.msSaveOrOpenBlob(blob, filename);
      //    } else {
      //        // Create a link to the file
      //        downloadLink.href = url;

      //        // Setting the file name
      //        downloadLink.download = filename;

      //        //triggering the function
      //        downloadLink.click();
      //    }

      //    document.body.removeChild(downloadLink);
      //},

      ExportToDoc() {
        var content = "<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'><head><meta charset='utf-8'><title>Export HTML to Word Document with JavaScript</title></head><body style=\"font-family: serif;\">";
        content += document.getElementById('tableView').innerHTML;
        content += "</body></html>";

        var converted = htmlDocx.asBlob(content, { orientation: 'landscape' });
        saveAs(converted, 'ЕГР Экспорт.docx');
      },
      ChangePageIP: function (page_num) {
        this.page = page_num;
        this.pagination_offset = (this.pagination_items_per_page * page_num) - this.pagination_items_per_page;
        this.IPTableContent = this.filteredIPTableContent.slice(this.pagination_offset, this.pagination_offset + this.pagination_items_per_page);
        window.scrollTo(0, 0);
      },
      ChangePageUL: function (page_num) {
        this.page = page_num;
        this.pagination_offset = (this.pagination_items_per_page * page_num) - this.pagination_items_per_page;
        this.ULTableContent = this.filteredULTableContent.slice(this.pagination_offset, this.pagination_offset + this.pagination_items_per_page);
        window.scrollTo(0, 0);
      },
      ChangePageShared: function (page_num) {
        this.page = page_num;
        this.pagination_offset = (this.pagination_items_per_page * page_num) - this.pagination_items_per_page;
        var count = this.pagination_offset + this.pagination_items_per_page - this.filteredSharedIPTableContent.length;
        if (count > 0) {
          if (this.pagination_offset <= this.filteredSharedIPTableContent.length) {
            this.SharedTableContent.sharedIPTableContent = this.filteredSharedIPTableContent.slice(this.pagination_offset, this.sharedIPTableContent_buffer.length);
          }
          else {
            this.SharedTableContent.sharedIPTableContent = [];
          }
          var pagination_offset_ul = this.pagination_offset - this.filteredSharedIPTableContent.length > 0 ? this.pagination_offset - this.filteredSharedIPTableContent.length : 0;
          this.SharedTableContent.sharedULTableContent = this.filteredSharedULTableContent.slice(pagination_offset_ul, count);
        }
        else {
          this.SharedTableContent.sharedIPTableContent = this.filteredSharedIPTableContent.slice(this.pagination_offset, this.pagination_offset + this.pagination_items_per_page);
          this.SharedTableContent.sharedULTableContent = [];
        }

        window.scrollTo(0, 0);
      },
      handleOpenModal({ row, type }) {
        this.OpenModal(row.idЛицо, row.ИНН, type, row.НаимСокр);
      },

      async handleLoadDetails({ table, id }) {
        // Сохраняем выбранную таблицу (нужно для логов)
        this.selectedDetailTable = table;

        // Очищаем
        this.detailsTableData = [];
        this.detailsTableHeaders = [];

        // Загружаем
        const url = `${baseURL}/Home/Details?table=${table}&id=${id}`;
        const response = await fetch(url);
        const tableData = await response.json();

        if (!tableData || tableData.length === 0) return;

        this.detailsTableData = tableData;

        // Формируем заголовки (без служебных полей)
        const firstRow = { ...tableData[0] };
        delete firstRow.Id;
        delete firstRow.idЛицо;
        delete firstRow.ИП;
        delete firstRow.ЮрЛицо;

        this.detailsTableHeaders = Object.keys(firstRow);
      },

      async handleLoadLogs({ table, inn }) {
        // Очищаем
        this.detailsTableData = [];
        this.detailsTableHeaders = [];

        const url = `${baseURL}/Home/GetLogs?table=${table}&INN=${inn}`;
        const response = await fetch(url);
        const tableData = await response.json();

        if (!tableData || tableData.length === 0) return;

        this.detailsTableData = tableData;

        const firstRow = { ...tableData[0] };
        delete firstRow.Id;

        this.detailsTableHeaders = Object.keys(firstRow);
      },
    }

  };
</script>

<style scoped>
  .egr-layout {
    height: 100%;
    overflow: hidden;
    /*min-width: 100%;
    min-height: 100%;
    position: relative;*/
  }
</style>
