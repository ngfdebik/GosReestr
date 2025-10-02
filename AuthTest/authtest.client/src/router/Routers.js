// src/router/index.js или Routers.js

import { createRouter, createWebHistory } from 'vue-router';
// Импортируем ваши компоненты
import Login from '../components/LoginComponent.vue'; // Убедитесь, что путь правильный
import EGR from '../components/layout/EGRLayout.vue'; // Убедитесь, что путь правильный
import UserEdit from '../components/UserEditComponent.vue'

// Определяем массив маршрутов
const routes = [
  {
    path: '/',
    name: 'Home',
    component: Login
  },
  {
    path: '/EGR',
    name: 'EGR',
    component: EGR,
    meta: {requiresGuest: true}
  },
  {
    path: '/UserEdit',
    name: 'UserEdit',
    component: UserEdit,
    meta: {requiresGuest: true}
  }
  // Добавьте другие маршруты здесь
];

// Создаем экземпляр маршрутизатора
const router = createRouter({
  history: createWebHistory(), // Используем HTML5 History mode
  routes, // сокращение от `routes: routes`
});

export default router;
