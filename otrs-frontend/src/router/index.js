import { createRouter, createWebHistory } from 'vue-router'
import LoginRegisterPage from '@/views/LoginRegisterPage.vue'
import ForgotPasswordPage from '@/views/ForgotPasswordPage.vue'
import ResetPasswordPage from '@/views/ResetPasswordPage.vue'

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
     {
      path: '/forgot-password',
      name: 'forgot-password',
      component: ForgotPasswordPage,
    },
     {
      path: '/reset-password',
      name: 'reset-password',
      component: ResetPasswordPage,
    },
  ],
})

export default router
