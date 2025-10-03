// src/router/index.js или Routers.js

import { createRouter, createWebHistory } from 'vue-router';
import { guestGuard } from './guard';
// Импортируем ваши компоненты
import Login from '../components/LoginComponent.vue'; // Убедитесь, что путь правильный
import EGR from '../components/layout/EGRLayout.vue'; // Убедитесь, что путь правильный
import UserEdit from '../components/UserEditComponent.vue';
import Manage from '../components/ManageComponent.vue'

// Определяем массив маршрутов
const routes = [
  {
    path: '/',
    name: 'Home',
    component: Login,
    meta: {requiresGuest: true}
  },
  {
    path: '/EGR',
    name: 'EGR',
    component: EGR,
    meta: {requiresGuest: false}
  },
  {
    path: '/UserEdit',
    name: 'UserEdit',
    component: UserEdit,
    meta: {requiresGuest: false}
  },
  {
    path: '/Manage',
    name: 'Manage',
    component: Manage,
    meta: {requiresGuest: false}
  }
  // Добавьте другие маршруты здесь
];

// Создаем экземпляр маршрутизатора
const router = createRouter({
  history: createWebHistory(), // Используем HTML5 History mode
  routes, // сокращение от `routes: routes`
});


router.beforeEach(guestGuard);

export default router;
