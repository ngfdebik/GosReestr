<!-- src/components/layout/EGRLayout.vue -->
<template>
  <div class="egr-layout">

    <!--:isUploadingExcel="isUploadingExcel"-->
    <Header :isAdmin="isAdmin"
            :isLoggedIn="isLoggedIn"
            @load-all="LoadAllTable"
            @load-ip="LoadIPTable"
            @load-ul="LoadULTable"
            @export-excel="exportToExcel"
            @export-doc="ExportToDoc"
            @clear-filters="ClearFilters" />
    <div class="content-wrapper">
      <div class="main-content">
        <div class="table-section">
          <MainTableView :show-shared-headers="showSharedHeaders"
                         :show-i-p-headers="showIPHeaders"
                         :show-u-l-headers="showULHeaders"
                         :shared-i-p-rows="SharedTableContent.sharedIPTableContent"
                         :shared-u-l-rows="SharedTableContent.sharedULTableContent"
                         :ip-rows="IPTableContent"
                         :ul-rows="ULTableContent"
                         :filters="filters"
                         :show-load-more="showLoadMore"
                         @open-modal="handleOpenModal"
                         @load-more="handleLoadMore"
                         @update:filters="filters = $event"
                         @apply-filters="FilterTable" />
        </div>
      </div>
    </div>

    <ModalDetails v-model="isModalOpen"
                  :entity-id="String(modalEntity.idЛицо)"
                  :entity-inn="modalEntity.ИНН"
                  :entity-type="modalEntityType"
                  :entity-name="modalEntity.НаимСокр"
                  :details-data="detailsTableData"
                  :details-headers="detailsTableHeaders"
                  @load-details="handleLoadDetails"
                  @load-logs="handleLoadLogs" />
  </div>
</template>

<script>
  import Header from '@/components/Header.vue';
  import MainTableView from '@/components/MainTableView.vue';
  //import FiltersPanel from '@/components/FiltersPanel.vue';
  import ModalDetails from '@/components/ModalDetails.vue';
  import FileUploadZone from '@/components/FileUploadZone.vue';
  import store from '@/auth/store';
  import api from '@/services/TokenApi';
  import ExcelJS from 'exceljs';
  import { saveAs } from 'file-saver';
  import JSZip from 'jszip';
  import {
    Document,
    Paragraph,
    Table,
    TableRow,
    TableCell,
    WidthType,
    HeadingLevel,
    TextRun,
    AlignmentType,
    VerticalAlign,
    BorderStyle,
    Packer
  } from 'docx';
  //const baseURL = import.meta.env.VITE_API_URL || window.location.origin;//Пока непонятно
  // ВСТАВЬТЕ СЮДА ВЕСЬ ВАШ JS-КОД ИЗ site.js (data, methods, computed)
  // Я покажу структуру

  export default {
    name: 'EGRLayout',
    components: {
      Header,
      MainTableView,
      //FiltersPanel,
      ModalDetails,
      FileUploadZone,
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
        filteredSharedIPTableContent: [], //
        filteredSharedULTableContent: [], //
        filteredIPTableContent: [],       //
        filteredULTableContent: [],       //
        //массив фильтров
        filters: [],
        //переменные для создания объекта фильтра
        filterColumn: "",
        filterMode: "",
        filterValue: "",

        //для пагинации
        //pagination_IP_items_total: 0,
        //pagination_UL_items_total: 0,
        //pagination_All_items_total: 0,
        //pagination_items_per_page: 50,
        //page: 1,
        //pagination_offset: 0,
        //pagesCountIP: 0,
        //pagesCountUL: 0,
        //pagesCountShared: 0,
        //page_number: 1,
        // ...все ваши данные из site.js...
        userStatus: 'Администратор',
        isModalOpen: false,
        modalEntity: {},
        modalEntityType: '',
        // ленивая загрузка
        loadedCount: 50,
      };
    },
    computed: {
      showLoadMore() {
        if (this.showIPHeaders) {
          return this.loadedCount < this.filteredIPTableContent.length;
        } else if (this.showULHeaders) {
          return this.loadedCount < this.filteredULTableContent.length;
        } else if (this.showSharedHeaders) {
          const total = this.filteredSharedIPTableContent.length + this.filteredSharedULTableContent.length;
          return this.loadedCount < total;
        }
        return false;
      },
      
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
      }
    },
    methods: {
      updateDisplayedRows() {
        if (this.showIPHeaders) {
          this.IPTableContent = this.filteredIPTableContent.slice(0, this.loadedCount);
        } else if (this.showULHeaders) {
          this.ULTableContent = this.filteredULTableContent.slice(0, this.loadedCount);
        } else if (this.showSharedHeaders) {
          const allSharedIP = this.filteredSharedIPTableContent;
          const allSharedUL = this.filteredSharedULTableContent;
          const total = allSharedIP.length + allSharedUL.length;
          const limit = Math.min(this.loadedCount, total);

          let ipToShow = Math.min(allSharedIP.length, limit);
          let ulToShow = limit - ipToShow;
          if (ulToShow < 0) {
            ipToShow += ulToShow;
            ulToShow = 0;
          }

          this.SharedTableContent.sharedIPTableContent = allSharedIP.slice(0, ipToShow);
          this.SharedTableContent.sharedULTableContent = allSharedUL.slice(0, ulToShow);
        }
      },
      handleLoadMore() {
        this.loadedCount += 50;
        this.updateDisplayedRows();

        // Прокручиваем к низу таблицы
        //this.$nextTick(() => {
        //  const tableView = document.getElementById('tableView');
        //  if (tableView) {
        //    tableView.scrollTop = tableView.scrollHeight;
        //  }
        //});
      },
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
      //async onChangeSelectedTable() {
      //  if (this.selectedTable == 'default' || this.selectedTable == "") {
      //    return;
      //  }

      //  //очищаем отображаемую основную таблицу
      //  this.IPTableHeaders = [];
      //  this.IPTableContent = [];
      //  this.ULTableHeaders = [];
      //  this.ULTableContent = [];
      //  this.showIPHeaders = false;
      //  this.showULHeaders = false;

      //  this.extraTableContent = [];
      //  this.extraTableHeaders = [];
      //  this.entityType = '';

      //  //подгружаем выбранную основную таблицу
      //  var url = baseURL + '/Home/GetExtraTable?table=' + this.selectedTable;
      //  let response = await fetch(url);
      //  let tableData = await response.json();

      //  this.extraTableContent = tableData;

      //  //удаляем неотображаемые поля
      //  delete tableData[0].Id;
      //  delete tableData[0].idЛицо;
      //  delete tableData[0].ИП;
      //  delete tableData[0].ЮрЛицо

      //  Object.keys(tableData[0]).forEach((element) => {
      //    this.extraTableHeaders.push(element);
      //  })

      //  this.extraTableHeaders.push('ДопИнф');

      //  //сохраняем таблицу в хранилище
      //  this.extraTableContent_buffer = this.extraTableContent
      //},
      async LoadAllTable() {
        this.ClearFilters();
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
          this.loadedCount = 50;
          this.updateDisplayedRows();
        } catch (error) {
          console.error('Ошибка загрузки данных:', error);
          // Можно показать уведомление пользователю
        }
      },

      async LoadIPTable() {
        this.ClearFilters();
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
          this.loadedCount = 50;
          this.updateDisplayedRows();
        } catch (error) {
          console.error('Ошибка загрузки данных:', error);
          // Можно показать уведомление пользователю
        }
      },

      async LoadULTable() {
        this.ClearFilters();
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
          this.loadedCount = 50;
          this.updateDisplayedRows();
        } catch (error) {
          console.error('Ошибка загрузки данных:', error);
          // Можно показать уведомление пользователю
        }
      },

      FilterTable() {
        if (this.filters.length == 0 && (this.sharedIPTableContent_buffer != null || this.sharedIPTableContent_buffer != undefined)) {
          this.filteredSharedIPTableContent = this.sharedIPTableContent_buffer;
          this.loadedCount = 50;
          this.updateDisplayedRows();
        }

        if (this.filters.length == 0 && (this.sharedULTableContent_buffer != null || this.sharedULTableContent_buffer != undefined)) {
          this.filteredSharedULTableContent = this.sharedULTableContent_buffer;
          this.loadedCount = 50;
          this.updateDisplayedRows();
        }

        if (this.filters.length == 0 && (this.IPTableContent_buffer != null || this.IPTableContent_buffer != undefined)) {
          this.filteredIPTableContent = this.IPTableContent_buffer;
          this.loadedCount = 50;
          this.updateDisplayedRows();
        }

        if (this.filters.length == 0 && (this.ULTableContent_buffer != null || this.ULTableContent_buffer != undefined)) {
          this.filteredULTableContent = this.ULTableContent_buffer;
          this.loadedCount = 50;
          this.updateDisplayedRows();
          return;
        }

        //if (this.filters.length == 0 && (this.extraTableContent_buffer != null || this.extraTableContent_buffer != undefined)) {
        //    this.extraTableContent = this.extraTableContent_buffer;
        //    return;
        //}
        this.filteredIPTableContent = [...this.IPTableContent_buffer];
        this.filteredULTableContent = [...this.ULTableContent_buffer];
        this.filteredSharedIPTableContent = [...this.sharedIPTableContent_buffer];
        this.filteredSharedULTableContent = [...this.sharedULTableContent_buffer];

        this.filters.forEach((filter) => {
          const isDateCol = filter.col.includes('Дата');

          const filterValue = isDateCol ? filter.value : filter.value; // уже строка

          // Функция для извлечения даты из строки вида "2023-07-03T00:00:00"
          const extractDatePart = (dateStr) => {
            if (!dateStr) return '';
            return dateStr.split('T')[0]; // "2023-07-03"
          };

          const applyFilter = (row) => {
            const cellValue = row[filter.col];
            if (cellValue == null) return false;

            const compareValue = isDateCol ? extractDatePart(cellValue) : String(cellValue);

            switch (filter.mode) {
              case '=':
                return compareValue === filterValue;
              case '!=':
                return compareValue !== filterValue;
              case 'содержит':
                return !isDateCol && compareValue.includes(filterValue);
              case 'начинается с':
                return !isDateCol && compareValue.startsWith(filterValue);
              case 'заканчивается на':
                return !isDateCol && compareValue.endsWith(filterValue);
              case '>':
                return compareValue > filterValue;
              case '<':
                return compareValue < filterValue;
              default:
                return true;
            }
          };

          this.filteredIPTableContent = this.filteredIPTableContent.filter(applyFilter);
          this.filteredULTableContent = this.filteredULTableContent.filter(applyFilter);
          this.filteredSharedIPTableContent = this.filteredSharedIPTableContent.filter(applyFilter);
          this.filteredSharedULTableContent = this.filteredSharedULTableContent.filter(applyFilter);
        });

        this.loadedCount = 50;
        this.updateDisplayedRows();
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

      async exportToExcel() {
        // Определяем данные и название листа
        let rows = [];
        let columns = [];
        let sheetName = 'ГосРеестр';

        const formatDate = (dateStr) => {
          if (!dateStr) return '';
          const d = new Date(dateStr);
          return isNaN(d) ? '' : d.toLocaleDateString('ru-RU'); // ДД.ММ.ГГГГ
        };

        if (this.showSharedHeaders) {
          const ipRows = this.filteredSharedIPTableContent.map(row => ({
            ИНН: row.ИНН || '',
            ОГРН: row.ОГРН || '',
            ДатаОГРН: formatDate(row.ДатаОГРН),
            НаимСокр: row.НаимСокр || '',
            ОКВЭДОсн: row.ОКВЭДОсн || '',
            ДатаВып: formatDate(row.ДатаВып)
          }));
          const ulRows = this.filteredSharedULTableContent.map(row => ({
            ИНН: row.ИНН || '',
            ОГРН: row.ОГРН || '',
            ДатаОГРН: formatDate(row.ДатаОГРН),
            НаимСокр: row.НаимСокр || '',
            ОКВЭДОсн: row.ОКВЭДОсн || '',
            ДатаВып: formatDate(row.ДатаВып)
          }));
          rows = [...ipRows, ...ulRows];
          columns = [
            { header: 'ИНН', key: 'ИНН', width: 15 },
            { header: 'ОГРН', key: 'ОГРН', width: 18 },
            { header: 'Дата ОГРН', key: 'ДатаОГРН', width: 14 },
            { header: 'Наименование', key: 'НаимСокр', width: 25 },
            { header: 'ОКВЭД', key: 'ОКВЭДОсн', width: 15 },
            { header: 'Дата выписки', key: 'ДатаВып', width: 14 }
          ];
          /*sheetName = '';*/
        }
        else if (this.showIPHeaders) {
          rows = this.filteredIPTableContent.map(row => ({
            ИНН: row.ИНН || '',
            ОГРНИП: row.ОГРН || '',
            ДатаОГРНИП: formatDate(row.ДатаОГРН),
            НаимВидИП: row.НаимВидИП || '',
            НаимСокр: row.НаимСокр || '',
            ОКВЭДОсн: row.ОКВЭДОсн || '',
            ДатаВып: formatDate(row.ДатаВып),
            КодВидИП: row.КодВидИП || '',
          }));
          columns = [
            { header: 'ИНН', key: 'ИНН', width: 15 },
            { header: 'ОГРНИП', key: 'ОГРНИП', width: 18 },
            { header: 'Дата ОГРНИП', key: 'ДатаОГРНИП', width: 14 },
            { header: 'Вид ИП', key: 'НаимВидИП', width: 20 },
            { header: 'Наименование', key: 'НаимСокр', width: 25 },
            { header: 'ОКВЭД', key: 'ОКВЭДОсн', width: 15 },
            { header: 'Дата выписки', key: 'ДатаВып', width: 14 },
            { header: 'Код вида', key: 'КодВидИП', width: 12 }
          ];
            /*sheetName = '';*/
        }
        else if (this.showULHeaders) {
          rows = this.filteredULTableContent.map(row => ({
            ИНН: row.ИНН || '',
            КПП: row.КПП || '',
            ОГРН: row.ОГРН || '',
            ДатаОГРН: formatDate(row.ДатаОГРН),
            ПолнНаимОПФ: row.ПолнНаимОПФ || '',
            НаимСокр: row.НаимСокр || '',
            ОКВЭДОсн: row.ОКВЭДОсн || '',
            СпрОПФ: row.СпрОПФ || '',
            КодОПФ: row.КодОПФ || '',
            ДатаВып: formatDate(row.ДатаВып)
          }));
          columns = [
            { header: 'ИНН', key: 'ИНН', width: 15 },
            { header: 'КПП', key: 'КПП', width: 12 },
            { header: 'ОГРН', key: 'ОГРН', width: 18 },
            { header: 'Дата ОГРН', key: 'ДатаОГРН', width: 14 },
            { header: 'Полное наименование', key: 'ПолнНаимОПФ', width: 30 },
            { header: 'Сокращённое', key: 'НаимСокр', width: 25 },
            { header: 'ОКВЭД', key: 'ОКВЭДОсн', width: 15 },
            { header: 'Справочник ОПФ', key: 'СпрОПФ', width: 18 },
            { header: 'Код ОПФ', key: 'КодОПФ', width: 12 },
            { header: 'Дата выписки', key: 'ДатаВып', width: 14 }
          ];
          /*sheetName = '';*/
        }
        else {
          alert('Нет данных для экспорта.');
          return;
        }

        if (rows.length === 0) {
          alert('Нет данных для экспорта.');
          return;
        }

        // Создаём книгу и лист
        const workbook = new ExcelJS.Workbook();
        const worksheet = workbook.addWorksheet(sheetName);

        // Настраиваем столбцы (ширина)
        worksheet.columns = columns;

        // Стиль заголовков: жирный + по центру + перенос
        const headerRow = worksheet.getRow(1);
        headerRow.eachCell((cell, colNumber) => {
          cell.value = columns[colNumber - 1].header;
          cell.font = { bold: true };
          cell.alignment = {
            horizontal: 'center',
            vertical: 'middle',
            wrapText: true
          };
          cell.fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'E0E0E0' } // светло-серый фон (опционально)
          };
        });

        // Добавляем данные
        worksheet.addRows(rows);

        // Применяем стиль ко всем ячейкам с данными: перенос текста
        for (let i = 2; i <= rows.length + 1; i++) {
          const row = worksheet.getRow(i);
          row.eachCell((cell) => {
            cell.alignment = {
              vertical: 'middle',
              wrapText: true
            };
            // Опционально: убрать перенос для дат/ИНН, если нужно
          });
        }

        // Сохраняем файл
        const buffer = await workbook.xlsx.writeBuffer();
        const blob = new Blob([buffer], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `ЕГР Экспорт.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      async ExportToDoc() {
        const formatDate = (dateStr) => {
          if (!dateStr) return '';
          const d = new Date(dateStr);
          return isNaN(d) ? '' : d.toLocaleDateString('ru-RU');
        };

        let headers = [];
        let rows = [];
        let title = 'Выписка из ЕГР';

        if (this.showSharedHeaders) {
          headers = ['ИНН', 'ОГРН', 'Дата ОГРН', 'Наименование', 'ОКВЭД', 'Дата выписки'/*, 'ДопИНФ'*/];
          const ipRows = this.filteredSharedIPTableContent.map(row => [
            row.ИНН || '',
            row.ОГРН || '',
            formatDate(row.ДатаОГРН),
            row.НаимСокр || '',
            row.ОКВЭДОсн || '',
            formatDate(row.ДатаВып)
            //,'...'
          ]);
          const ulRows = this.filteredSharedULTableContent.map(row => [
            row.ИНН || '',
            row.ОГРН || '',
            formatDate(row.ДатаОГРН),
            row.НаимСокр || '',
            row.ОКВЭДОсн || '',
            formatDate(row.ДатаВып)
            //,'...'
          ]);
          rows = [...ipRows, ...ulRows];
          title = 'Все лица';
        }
        else if (this.showIPHeaders) {
          headers = ['ИНН', 'ОГРНИП', 'Дата ОГРНИП', 'Вид ИП', 'Наименование', 'ОКВЭД', 'Дата выписки', 'Код вида'/*, 'ДопИНФ'*/];
          rows = this.filteredIPTableContent.map(row => [
            row.ИНН || '',
            row.ОГРН || '',
            formatDate(row.ДатаОГРН),
            row.НаимВидИП || '',
            row.НаимСокр || '',
            row.ОКВЭДОсн || '',
            formatDate(row.ДатаВып),
            row.КодВидИП || ''
            //,'...'
          ]);
          title = 'Индивидуальные предприниматели';
        }
        else if (this.showULHeaders) {
          headers = ['ИНН', 'КПП', 'ОГРН', 'Дата ОГРН', 'Полное наименование', 'Сокращённое', 'ОКВЭД', 'Спр. ОПФ', 'Код ОПФ', 'Дата выписки'/*, 'ДопИНФ'*/];
          rows = this.filteredULTableContent.map(row => [
            row.ИНН || '',
            row.КПП || '',
            row.ОГРН || '',
            formatDate(row.ДатаОГРН),
            row.ПолнНаимОПФ || '',
            row.НаимСокр || '',
            row.ОКВЭДОсн || '',
            row.СпрОПФ || '',
            row.КодОПФ || '',
            formatDate(row.ДатаВып)
            //,'...'
          ]);
          title = 'Юридические лица';
        }
        else {
          alert('Нет данных для экспорта.');
          return;
        }

        if (rows.length === 0) {
          alert('Нет данных для экспорта.');
          return;
        }

        const MAX_ROWS_PER_DOC = 10000;
        const totalRows = rows.length;

        // === Проверка на большое количество строк ===
        if (totalRows > MAX_ROWS_PER_DOC) {
          const confirmMsg = `Вы пытаетесь экспортировать ${totalRows} строк. 
Максимум 10 000 строк в одном файле.
Будет создан ZIP-архив с ${Math.ceil(totalRows / MAX_ROWS_PER_DOC)} файлами.
Продолжить?`;

          if (!confirm(confirmMsg)) {
            return; // пользователь отменил
          }
        }

        // === Вспомогательная функция: создать один документ ===
        const createDoc = (docRows, docIndex = 0) => {
          const headerRow = new TableRow({
            children: headers.map(text =>
              new TableCell({
                children: [new Paragraph({ text, heading: HeadingLevel.HEADING_6 })],
                verticalAlign: VerticalAlign.CENTER,
                margins: { top: 100, bottom: 100, left: 100, right: 100 },
                shading: { fill: "E0E0E0" }
              })
            )
          });

          const dataRows = docRows.map(row =>
            new TableRow({
              children: row.map(cell =>
                new TableCell({
                  children: [new Paragraph(String(cell ?? ''))], // защита от null/undefined
                  verticalAlign: VerticalAlign.TOP,
                  margins: { top: 100, bottom: 100, left: 100, right: 100 }
                })
              )
            })
          );

          const table = new Table({
            rows: [headerRow, ...dataRows],
            width: { size: 100, type: WidthType.PERCENTAGE },
            margins: { top: 200, bottom: 200 },
            borders: {
              top: { style: BorderStyle.SINGLE, size: 6, color: "000000" },
              bottom: { style: BorderStyle.SINGLE, size: 6, color: "000000" },
              left: { style: BorderStyle.SINGLE, size: 6, color: "000000" },
              right: { style: BorderStyle.SINGLE, size: 6, color: "000000" },
              insideHorizontal: { style: BorderStyle.SINGLE, size: 6, color: "000000" },
              insideVertical: { style: BorderStyle.SINGLE, size: 6, color: "000000" }
            }
          });

          return new Document({
            sections: [{
              properties: {
                page: {
                  margin: {
                    top: 1440,
                    bottom: 1440,
                    left: 1440,
                    right: 1440
                  },
                  size: {
                    orientation: "landscape"
                  }
                }
              },
              children: [
                new Paragraph({
                  text: `Выписка из ЕГР: ${title} (часть ${docIndex + 1})`,
                  heading: HeadingLevel.HEADING_1,
                  alignment: AlignmentType.CENTER,
                  spacing: { after: 300 }
                }),
                table
              ]
            }]
          });
        };

        // === Разбиваем на чанки ===
        const chunks = [];
        for (let i = 0; i < rows.length; i += MAX_ROWS_PER_DOC) {
          chunks.push(rows.slice(i, i + MAX_ROWS_PER_DOC));
        }

        if (chunks.length === 1) {
          // Обычный экспорт — как раньше
          const doc = createDoc(chunks[0]);
          const blob = await Packer.toBlob(doc);
          saveAs(blob, `ЕГР_Экспорт_${title}.docx`);
        } else {
          // === Множественный экспорт в ZIP ===
          const zip = new JSZip();
          const promises = chunks.map(async (chunk, index) => {
            const doc = createDoc(chunk, index);
            const blob = await Packer.toBlob(doc);
            const filename = `ЕГР_Экспорт_${title}_часть_${index + 1}.docx`;
            zip.file(filename, blob);
          });

          await Promise.all(promises);

          // Генерируем ZIP-файл
          const zipBlob = await zip.generateAsync({ type: 'blob' });
          saveAs(zipBlob, `ЕГР_Экспорт_${title}_архив.zip`);
        }
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
        try {
          const response = await api.get('/Home/Details', {
            params: { table: `${table}`, id: `${id}` }
          });
          const tableData = response.data;

          if (!tableData || tableData.length === 0) return;

          this.detailsTableData = tableData;

          // Формируем заголовки (без служебных полей)
          const firstRow = { ...tableData[0] };
          delete firstRow.Id;
          delete firstRow.idЛицо;
          delete firstRow.ИП;
          delete firstRow.ЮрЛицо;

          this.detailsTableHeaders = Object.keys(firstRow);
        } catch (error) {
          console.error('Ошибка загрузки данных:', error);
          // Можно показать уведомление пользователю
        }
      },
      

      async handleLoadLogs({ table, inn }) {
        // Очищаем
        this.detailsTableData = [];
        this.detailsTableHeaders = [];
        try {
          const response = await api.get('/Home/GetLogs', {
            params: { table: `${table}`, INN: `${inn}` }
          });
          const tableData = response.data;

          if (!tableData || tableData.length === 0) return;

          this.detailsTableData = tableData;

          const firstRow = { ...tableData[0] };
          delete firstRow.Id;

          this.detailsTableHeaders = Object.keys(firstRow);
        } catch (error) {
          console.error('Ошибка загрузки данных:', error);
          // Можно показать уведомление пользователю
        }
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

  .content-wrapper {
    width: 100%;
    /*padding-right: 15px;
    padding-left: 15px;*/
    margin-right: auto;
    margin-left: auto;
    margin-top: 1rem;
}

.main-content {
    display: flex;
    flex-wrap: wrap;
    /*margin-right: -15px;
    margin-left: -15px;*/
}

.table-section {
    width:100%;
    /*padding-right: 15px;
    padding-left: 15px;*/
}

/* Responsive Design */
@media (max-width: 1200px) {
    .table-section {
        flex: 0 0 83.333333%;
        max-width: 83.333333%;
    }
}

@media (max-width: 992px) {
    .table-section {
        flex: 0 0 100%;
        max-width: 100%;
    }
    
    .content-wrapper {
        padding-right: 10px;
        padding-left: 10px;
    }
    
    .main-content {
        margin-right: -10px;
        margin-left: -10px;
    }
}

@media (max-width: 768px) {
    .content-wrapper {
        padding-right: 5px;
        padding-left: 5px;
        margin-top: 0.5rem;
    }
    
    .main-content {
        margin-right: -5px;
        margin-left: -5px;
    }
    
    .table-section {
        padding-right: 5px;
        padding-left: 5px;
    }
}
</style>
