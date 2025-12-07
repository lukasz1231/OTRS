import { createRouter, createWebHistory } from 'vue-router'
import LoginRegisterPage from '@/views/LoginRegisterPage.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: LoginRegisterPage,
    },
    {
      path: '/register',
      name: 'register',
      component: LoginRegisterPage,
    },
  ],
})

export default router
