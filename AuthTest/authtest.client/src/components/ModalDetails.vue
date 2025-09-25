<!-- src/components/ModalDetails.vue -->
<template>
  <div class="modal fade" id="modalDetails" ref="modal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-xl">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Дополнительная информация</h5>
          <button type="button" class="btn-close" @click="hideModal" aria-label="Закрыть"></button>
        </div>
        <h3 class="lead m-1 ps-2">{{ entityName }}</h3>
        <div class="modal-body">
          <div class="p-2">
            <select v-model="localSelectedDetail" class="m-2">
              <option disabled value="">Выберите раздел</option>
              <!-- IP sections -->
              <option value="EGRIPOKVED" v-show="entityType === 'IP'">ОКВЭД</option>
              <option value="EGRIPSvAdrMJ" v-show="entityType === 'IP'">АдресМЖ</option>
              <option value="EGRIPSvGrajd" v-show="entityType === 'IP'">Гражд</option>
              <option value="EGRIPSVFL" v-show="entityType === 'IP'">ФЛ</option>
              <option value="EGRIPSvLicense" v-show="entityType === 'IP'">Лицензия</option>
              <option value="EGRIPSvPrekras_" v-show="entityType === 'IP'">Прекращ</option>
              <option value="EGRIPSvRegIP" v-show="entityType === 'IP'">РегИП</option>
              <option value="EGRIPSvGegOrg" v-show="entityType === 'IP'">РегОрг</option>
              <option value="EGRIPSvRegPF" v-show="entityType === 'IP'">РегПФ</option>
              <option value="EGRIPSvRegFSS" v-show="entityType === 'IP'">РегФСС</option>
              <option value="EGRIPSvAccountingNO" v-show="entityType === 'IP'">УчетНО</option>

              <!-- UL sections -->
              <option value="EGRULOKVED" v-show="entityType === 'UL'">ОКВЭД</option>
              <option value="EGRULSvAddressUL" v-show="entityType === 'UL'">АдресЮЛ</option>
              <option value="EGRULSvDerjRegistryAO" v-show="entityType === 'UL'">ДержРегистрАО</option>
              <option value="EGRULSvShareOOO" v-show="entityType === 'UL'">ДоляООО</option>
              <option value="EGRULSvZapEGRUL" v-show="entityType === 'UL'">ЗапЕГРЮЛ</option>
              <option value="EGRULSvLicense" v-show="entityType === 'UL'">Лицензия</option>
              <option value="EGRULSvNaimUL" v-show="entityType === 'UL'">НаимЮЛ</option>
              <option value="EGRULSvObrUL" v-show="entityType === 'UL'">ОбрЮЛ</option>
              <option value="EGRULSvPodrazd" v-show="entityType === 'UL'">Подразд</option>
              <option value="EGRULSvPredsh" v-show="entityType === 'UL'">Предш</option>
              <option value="EGRULSvPreem" v-show="entityType === 'UL'">Преем</option>
              <option value="EGRULSvPrekrUL" v-show="entityType === 'UL'">ПрекрЮЛ</option>
              <option value="EGRULSvRegOrg" v-show="entityType === 'UL'">РегОрг</option>
              <option value="EGRULSvRegPF" v-show="entityType === 'UL'">РегПФ</option>
              <option value="EGRULSvRegFSS" v-show="entityType === 'UL'">РегФСС</option>
              <option value="EGRULSvReorg" v-show="entityType === 'UL'">Реорг</option>
              <option value="EGRULSvStatus" v-show="entityType === 'UL'">Статус</option>
              <option value="EGRULSvUstKap" v-show="entityType === 'UL'">УстКап</option>
              <option value="EGRULSvAccountingNO" v-show="entityType === 'UL'">УчетНО</option>
              <option value="EGRULSvFounder" v-show="entityType === 'UL'">Учредит</option>
              <option value="EGRULSvUL" v-show="entityType === 'UL'">ЮЛ</option>
              <option value="EGRULSvedDoljnFL" v-show="entityType === 'UL'">ДолжнФЛ</option>
            </select>
            <button class="btn btn-outline-primary me-3" @click="emitLoadDetails">Показать</button>
            <button class="btn btn-outline-primary" @click="emitLoadLogs">Показать Историю Изменений</button>
          </div>

          <div style="overflow: auto; max-height: 60vh;">
            <table v-if="detailsData.length > 0" class="table table-hover table-bordered border-dark">
              <thead>
                <tr>
                  <th v-for="header in detailsHeaders"
                      :key="header"
                      class="table-secondary border-dark"
                      style="font-family:'Times New Roman';">
                    {{ header }}
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(row, i) in trimmedData" :key="i">
                  <td v-for="(cell, j) in row"
                      :key="j"
                      style="font-family:'Times New Roman';">
                    {{ cell }}
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
</template>

<script>
  import * as bootstrap from 'bootstrap';

  export default {
    name: 'ModalDetails',
    props: {
      entityId: { type: String, required: true },
      entityInn: { type: String, required: true },
      entityType: { type: String, required: true },
      entityName: { type: String, default: '' },
      detailsData: { type: Array, default: () => [] },
      detailsHeaders: { type: Array, default: () => [] }
    },
    emits: ['close', 'load-details', 'load-logs'],
    data() {
      return {
        localSelectedDetail: '',
        modalInstance: null
      };
    },
    computed: {
      trimmedData() {
        return this.detailsData.map(row => {
          const { Id, idЛицо, ИП, ЮрЛицо, ...rest } = row;
          return Object.values(rest);
        });
      }
    },
    mounted() {
      this.modalInstance = new bootstrap.Modal(this.$refs.modal, {
        backdrop: 'static',
        keyboard: false
      });
      this.showModal();
    },
    unmounted() {
      if (this.modalInstance) {
        this.modalInstance.dispose();
      }
    },
    methods: {
      showModal() {
        this.modalInstance.show();
      },
      hideModal() {
        this.modalInstance.hide();
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
