<!-- src/components/Header.vue -->
<template>
  <header>
    <div class="row row-cols-auto" style="background-color:#e4e3e8;">
      <div class="col-2 d-flex align-items-center">
        <img src="/svg/Logo.svg" style="height:8vh;" class="ps-3" />
      </div>
      <div class="col-1 offset-1 d-flex justify-content-center">
        <button class="mainButton p-1" @click="$emit('load-all')">
          <img src="/svg/All.svg" title="Все" class="m-auto" style="height:8vh;" />
          <p class="m-0">Все</p>
        </button>
      </div>
      <div class="col-1 d-flex justify-content-center">
        <button class="mainButton p-1" @click="$emit('load-ip')">
          <img src="/svg/IP.svg" title="ИП" class="m-auto" style="height:8vh;" />
          <p class="m-0">ИП</p>
        </button>
      </div>
      <div class="col-1 d-flex justify-content-center">
        <button class="mainButton p-1" @click="$emit('load-ul')">
          <img src="/svg/UL.svg" title="ЮЛ" class="m-auto" style="height:8vh;" />
          <p class="m-0">ЮЛ</p>
        </button>
      </div>
      <div class="col-1 d-flex justify-content-center">
        <button @click="$emit('export-excel')" class="mainButton p-1">
          <img src="/svg/Export.svg" title="Экспорт в Excel" class="m-auto" style="height:8vh;" />
          <p class="m-0">Экспорт в Excel</p>
        </button>
      </div>
      <div class="col-1 d-flex justify-content-center">
        <button @click="$emit('export-doc')" class="mainButton p-1">
          <img src="/svg/Export.svg" title="Экспорт в Word" class="m-auto" style="height:8vh;" />
          <p class="m-0">Экспорт в Word</p>
        </button>
      </div>
      <div class="col-2 offset-2">
        <span>Пользователь: {{ userStatus }}</span>
      </div>
    </div>
  </header>
</template>

<script>
  export default {
    name: 'Header',
    props: {
      userStatus: { type: String, default: 'Пользователь' }
    },
    emits: ['load-all', 'load-ip', 'load-ul', 'export-excel', 'export-doc']
  };
</script>
