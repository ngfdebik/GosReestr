@page
@model EGRModel

<div class="m-3" style="margin-bottom:1%">
  <div class="row pb-2">
    <div class="col-9 pe-0">
      <div id="tableView">
        <div class="border border-1 border-dark overflow-auto" style="height:80vh">
          <table class="table table-hover table-bordered border-dark m-0" id="IPTable" data-cols-width="15,17,15,43,50,12,15,10">
            <tr>
              @*
              <th class="table-secondary border-dark" v-for="header in IPTableHeaders"> {{ header }} </th> *@
              <th class="table-secondary border-dark" v-show="showIPHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ИНН</th>
              <th class="table-secondary border-dark" v-show="showIPHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ОГРНИП</th>
              <th class="table-secondary border-dark" v-show="showIPHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ДатаОГРНИП</th>
              <th class="table-secondary border-dark" v-show="showIPHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">НаимВидИП</th>
              <th class="table-secondary border-dark" v-show="showIPHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">НаимСокр</th>
              <th class="table-secondary border-dark" v-show="showIPHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ОКВЭДОсн</th>
              <th class="table-secondary border-dark" v-show="showIPHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ДатаВып</th>
              <th class="table-secondary border-dark" v-show="showIPHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">КодВидИП</th>
              <th class="table-secondary border-dark" v-show="showIPHeaders" data-exclude="true" style="font-family:'Times New Roman'; position: sticky; top: 0;">ДопИНФ</th>
            </tr>
            <tr v-for="row in IPTableContent">
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.ИНН }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.ОГРН }} </td>
              <td data-t="d" style="font-family:'Times New Roman';"> {{ row.ДатаОГРН.split('T')[0] }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.НаимВидИП }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.НаимСокр }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.ОКВЭДОсн }} </td>
              <td data-t="d" style="font-family:'Times New Roman';"> {{ row.ДатаВып.split('T')[0] }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.КодВидИП }} </td>
              <td data-exclude="true"> <button v-on:click="OpenModal(row.idЛицо, row.ИНН, 'IP', row.НаимСокр)" data-bs-toggle="modal" data-bs-target="#staticBackdrop">Подробнее</button></td>
            </tr>
          </table>
          <table class="table table-hover table-bordered border-dark m-0" id="ULTable" data-cols-width="15,15,15,15,72,75,11,9,10,15">
            <tr>
              @*
              <th class="table-secondary border-dark" v-for="header in ULTableHeaders"> {{ header }} </th> *@
              <th class="table-secondary border-dark" v-show="showULHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ИНН</th>
              <th class="table-secondary border-dark" v-show="showULHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">КПП</th>
              <th class="table-secondary border-dark" v-show="showULHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ОГРН</th>
              <th class="table-secondary border-dark" v-show="showULHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ДатаОГРН</th>
              <th class="table-secondary border-dark" v-show="showULHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ПолнНаимОПФ</th>
              <th class="table-secondary border-dark" v-show="showULHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">НаимСокр</th>
              <th class="table-secondary border-dark" v-show="showULHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ОКВЭДОсн</th>
              <th class="table-secondary border-dark" v-show="showULHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">СпрОПФ</th>
              <th class="table-secondary border-dark" v-show="showULHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">КодОПФ</th>
              <th class="table-secondary border-dark" v-show="showULHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ДатаВып</th>
              <th class="table-secondary border-dark" v-show="showULHeaders" data-exclude="true" style="font-family:'Times New Roman'; position: sticky; top: 0;">ДопИНФ</th>
            </tr>
            <tr v-for="row in ULTableContent">
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.ИНН }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.КПП }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.ОГРН }} </td>
              <td data-t="d" style="font-family:'Times New Roman';"> {{ row.ДатаОГРН.split('T')[0] }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.ПолнНаимОПФ }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.НаимСокр  }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.ОКВЭДОсн }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.СпрОПФ }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.КодОПФ }} </td>
              <td data-t="d" style="font-family:'Times New Roman';"> {{ row.ДатаВып.split('T')[0] }} </td>
              <td data-exclude="true" style="font-family:'Times New Roman';"> <button v-on:click="OpenModal(row.idЛицо, row.ИНН, 'UL', row.НаимСокр)" data-bs-toggle="modal" data-bs-target="#staticBackdrop">Подробнее</button></td>
            </tr>
          </table>
          <table class="table table-hover table-bordered border-dark m-0" id="sharedTable" data-cols-width="15,15,15,72,15,15">
            <tr>
              <th class="table-secondary border-dark" v-show="showSharedHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ИНН</th>
              <th class="table-secondary border-dark" v-show="showSharedHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ОГРН</th>
              <th class="table-secondary border-dark" v-show="showSharedHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ДатаОГРН</th>
              <th class="table-secondary border-dark" v-show="showSharedHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">НаимСокр</th>
              <th class="table-secondary border-dark" v-show="showSharedHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ОКВЭДОсн</th>
              <th class="table-secondary border-dark" v-show="showSharedHeaders" style="font-family:'Times New Roman'; position: sticky; top: 0;">ДатаВып</th>
              <th class="table-secondary border-dark" v-show="showSharedHeaders" data-exclude="true" style="font-family:'Times New Roman'; position: sticky; top: 0;">ДопИНФ</th>
            </tr>

            <tr v-for="row in sharedULTableContent">
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.ИНН }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.ОГРН }} </td>
              <td data-t="d" style="font-family:'Times New Roman';"> {{ row.ДатаОГРН.split('T')[0] }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.НаимСокр }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.ОКВЭДОсн }} </td>
              <td data-t="d" style="font-family:'Times New Roman';"> {{ row.ДатаВып.split('T')[0] }} </td>
              <td data-exclude="true" style="font-family:'Times New Roman';"> <button v-on:click="OpenModal(row.idЛицо, row.ИНН, 'UL', row.НаимСокр)" data-bs-toggle="modal" data-bs-target="#staticBackdrop">Подробнее</button></td>
            </tr>

            <tr v-for="row in sharedIPTableContent">
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.ИНН }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.ОГРН }} </td>
              <td data-t="d" style="font-family:'Times New Roman';"> {{ row.ДатаОГРН.split('T')[0] }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.НаимСокр }} </td>
              <td data-t="s" style="font-family:'Times New Roman';"> {{ row.ОКВЭДОсн }} </td>
              <td data-t="d" style="font-family:'Times New Roman';"> {{ row.ДатаВып.split('T')[0] }} </td>
              <td data-exclude="true"> <button v-on:click="OpenModal(row.idЛицо, row.ИНН, 'IP', row.НаимСокр)" data-bs-toggle="modal" data-bs-target="#staticBackdrop">Подробнее</button></td>
            </tr>
          </table>
        </div>
      </div>
    </div>
    <div class="col-3">
      <div class="border border-3 mt-2">
        <p class="lead m-0 text-center">Фильтры и параметры</p>

        <div class="border m-2">
          <div class="container" style="border-top-width: 1px; border-top-style: solid;">
            <div id="filter" class="row mt-2 mb-2">
              <div class="row gx-1 mb-1">
                <div class="col-4 ">
                  <select v-model="filterColumn" style="width:100%">
                    <option disabled value="">Столбец</option>
                    <option value="ИНН">ИНН</option>
                    <option value="ДатаВып">ДатаВып</option>
                    <option value="НаимСокр">НаимСокр</option>
                    <option value="ОКВЭДОсн">ОКВЭДОсн</option>
                    <option value="ОГРНИП" v-show="showIPHeaders">ОГРНИП</option>
                    <option value="ДатаОГРНИП" v-show="showIPHeaders">датаОГРНИП</option>
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
                  <select v-model="filterMode" style="width:100%">
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
                  <input v-model="filterValue" placeholder="Значение" style="width:100%; height:100%" />
                </div>
              </div>
              <button v-on:click="AddFilter()" class="mb-1">Добавить фильтр</button>
              <button v-on:click="FilterTable()" class="mb-1">Применить фильтры</button>
              @* <input type="checkbox" checked style="" /> <span style=""><b>Применить фильтр</b></span> *@
            </div>
            <div id="filterTable" class="tabcontent">
              <table class="mb-1">
                <tr v-for="filter in filters">
                  <td>{{ filter.col }}</td>
                  <td>{{ filter.mode }}</td>
                  <td>{{ filter.value }}</td>
                  <td><button type="button" class="btn-close" aria-label="Удалить фильтр" v-on:click="DeleteFilter(filter.id)"></button></td>
                </tr>
              </table>
              <button v-show="filters.length > 0" v-on:click="ClearFilters()" class="mb-2">Очистить фильтры</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</div>
<div class="modal fade" id="staticBackdrop" data-bs-backdrop="static" tabindex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
  <div class="modal-dialog modal-xl">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="staticBackdropLabel">Дополнительная информация</h5> <br />
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Закрыть"></button>
      </div>
      <h3 class="lead m-1 ps-2" id="staticBackdropLabel">{{ detailsEntityName }}</h3>
      <div class="modal-body">
        <div id="modalContent">
          <div id="modalButtons" class="p-2">
            <select v-model="selectedDetail" class="m-2">
              <option disabled value="">Выберите раздел</option>
              <option value="EGRIPOKVED" v-show="entityType.includes('IP')">ОКВЭД</option>
              <option value="EGRIPSvAdrMJ" v-show="entityType.includes('IP')">АдресМЖ</option>
              <option value="EGRIPSvGrajd" v-show="entityType.includes('IP')">Гражд</option>
              <option value="EGRIPSVFL" v-show="entityType.includes('IP')">ФЛ</option>
              <option value="EGRIPSvLicense" v-show="entityType.includes('IP')">Лицензия</option>
              <option value="EGRIPSvPrekras_" v-show="entityType.includes('IP')">Прекращ</option>
              <option value="EGRIPSvRegIP" v-show="entityType.includes('IP')">РегИП</option>
              <option value="EGRIPSvGegOrg" v-show="entityType.includes('IP')">РегОрг</option>
              <option value="EGRIPSvRegPF" v-show="entityType.includes('IP')">РегПФ</option>
              <option value="EGRIPSvRegFSS" v-show="entityType.includes('IP')">РегФСС</option>
              <option value="EGRIPSvAccountingNO" v-show="entityType.includes('IP')">УчетНО</option>

              <option value="EGRULOKVED" v-show="entityType.includes('UL')">ОКВЭД</option>
              <option value="EGRULSvAddressUL" v-show="entityType.includes('UL')">АдресЮЛ</option>
              <option value="EGRULSvDerjRegistryAO" v-show="entityType.includes('UL')">ДержРегистрАО</option>
              <option value="EGRULSvShareOOO" v-show="entityType.includes('UL')">ДоляООО</option>
              <option value="EGRULSvZapEGRUL" v-show="entityType.includes('UL')">ЗапЕГРЮЛ</option>
              <option value="EGRULSvLicense" v-show="entityType.includes('UL')">Лицензия</option>
              <option value="EGRULSvNaimUL" v-show="entityType.includes('UL')">НаимЮЛ</option>
              <option value="EGRULSvObrUL" v-show="entityType.includes('UL')">ОбрЮЛ</option>
              <option value="EGRULSvPodrazd" v-show="entityType.includes('UL')">Подразд</option>
              <option value="EGRULSvPredsh" v-show="entityType.includes('UL')">Предш</option>
              <option value="EGRULSvPreem" v-show="entityType.includes('UL')">Преем</option>
              <option value="EGRULSvPrekrUL" v-show="entityType.includes('UL')">ПрекрЮЛ</option>
              <option value="EGRULSvRegOrg" v-show="entityType.includes('UL')">РегОрг</option>
              <option value="EGRULSvRegPF" v-show="entityType.includes('UL')">РегПФ</option>
              <option value="EGRULSvRegFSS" v-show="entityType.includes('UL')">РегФСС</option>
              <option value="EGRULSvReorg" v-show="entityType.includes('UL')">Реорг</option>
              <option value="EGRULSvStatus" v-show="entityType.includes('UL')">Статус</option>
              <option value="EGRULSvUstKap" v-show="entityType.includes('UL')">УстКап</option>
              <option value="EGRULSvAccountingNO" v-show="entityType.includes('UL')">УчетНО</option>
              <option value="EGRULSvFounder" v-show="entityType.includes('UL')">Учредит</option>
              <option value="EGRULSvUL" v-show="entityType.includes('UL')">ЮЛ</option>
              <option value="EGRULSvedDoljnFL" v-show="entityType.includes('UL')">ДолжнФЛ</option>
            </select>
            <button class="btn btn-outline-primary me-3" v-on:click="GetDetailsData(selectedDetail, entityID)">Показать</button>
            <button class="btn btn-outline-primary" v-on:click="GetLogsData(selectedDetail, entityINN)">Показать Историю Изменений</button>
          </div>

          <div id="TableFrame" class="Tableframe" style="overflow: auto;">
            <table v-if="detailsTableData.length > 0" class="table table-hover table-bordered border-dark">
              <tr>
                <th class="table-secondary border-dark" v-for="header in detailsTableHeaders" style="font-family:'Times New Roman';"> {{ header }} </th>
              </tr>

              <tr v-for="row in trimmedDetailsTableData">
                <td v-for="cell in row" style="font-family:'Times New Roman';"> {{ cell }} </td>
              </tr>
            </table>
            <h2 v-else>Нет данных для отображения</h2>
          </div>
        </div>
      </div>
      <div class="modal-footer">
      </div>
    </div>
  </div>
</div>
<script type="text/javascript" src="https://unpkg.com/xlsx@0.15.1/dist/xlsx.full.min.js"></script>
<script src="~/js/vue.prod.js"></script>
<script type="module" src="~/js/site.js"></script>
<script src="~/js/dist/tableToExcel.js"></script>
<script>
  const mountedApp = app.mount('body');
</script>

<style>
  body {
    overflow: hidden;
  }

</style>
