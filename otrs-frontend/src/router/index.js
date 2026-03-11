import { createRouter, createWebHistory } from 'vue-router'
import LoginRegisterPage from '@/views/LoginRegisterPage.vue'
import ForgotPasswordPage from '@/views/ForgotPasswordPage.vue'
import ResetPasswordPage from '@/views/ResetPasswordPage.vue'
import Dashboard from '@/views/Dashboard.vue'
import ProblemReportHelpdesk from '@/views/ProblemReportHelpdesk.vue'
import ProblemReportClient from '@/views/ProblemReportClient.vue'
import ProfilePage from '@/views/ProfilePage.vue'
import PrivacyPage from '@/views/PrivacyPage.vue'
import TermsPage from '@/views/TermsPage.vue'
import TrademarksPage from '@/views/TrademarksPage.vue'
import AboutPage from '@/views/AboutPage.vue'
import ContactPage from '@/views/ContactPage.vue'
import AdminUsers from '@/views/AdminUsers.vue'
import AdminDashboard from '@/views/AdminDashboard.vue'
import AdminQueues from '@/views/AdminQueues.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    }
    return { top: 0, behavior: 'smooth' }
  },
routes: [
    {
      path: '/login',
      name: 'login',
      component: LoginRegisterPage,
      meta: { requiresGuest: true } 
    },
    {
      path: '/register',
      name: 'register',
      component: LoginRegisterPage,
      meta: { requiresGuest: true } 
    },
    {
      path: '/forgot-password',
      name: 'forgot-password',
      component: ForgotPasswordPage,
      meta: { requiresGuest: true } 
    },
    {
      path: '/reset-password',
      name: 'reset-password',
      component: ResetPasswordPage,
      meta: { requiresGuest: true } 
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: Dashboard,
      meta: { requiresAuth: true } 
    },
    {
      path: '/problemReportHelpdesk',
      name: 'problemReportHelpdesk',
      component: ProblemReportHelpdesk,
      meta: { requiresAuth: true } 
    },
    {
      path: '/problemReportClient',
      name: 'problemReportClient',
      component: ProblemReportClient,
      meta: { requiresAuth: true }
    },
    {
      path: '/profile',
      name: 'profile',
      component: ProfilePage,
      meta: { requiresAuth: true }
    },
    {
      path: '/privacy',
      name: 'privacy',
      component: PrivacyPage,
    },
    {
      path: '/terms',
      name: 'terms',
      component: TermsPage,
    },
    {
      path: '/trademark',
      name: 'trademark',
      component: TrademarksPage,
    },
    {
      path: '/about',
      name: 'about',
      component: AboutPage,
    },
    {
      path: '/contact',
      name: 'contact',
      component: ContactPage,
    },
    {
      path: '/',
      redirect: '/dashboard'
    },
   {
      path: '/admin',
      name: 'admin',
      component: AdminDashboard,
      meta: { requiresAuth: true, requiresAdmin: true }
    },
    {
      path: '/admin/users',
      name: 'admin-users',
      component: AdminUsers,
      meta: { requiresAuth: true, requiresAdmin: true }
    },
    {
      path: '/admin/queues',
      name: 'admin-queues',
      component: AdminQueues,
      meta: { requiresAuth: true, requiresAdmin: true }
    }
  ],
})

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  
  if (to.meta.requiresAuth && !token) {
    return next('/login')
  }

  if (to.meta.requiresAdmin) {
    // Proste wyciągnięcie roli z tokena (bez bibliotek)
    const payload = JSON.parse(atob(token.split('.')[1]))
    const roles = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
    
    // Sprawdzamy czy role to tablica czy pojedynczy string i czy zawiera Admin
    const isAdmin = Array.isArray(roles) ? roles.includes('Admin') : roles === 'Admin'
    
    if (!isAdmin) {
      return next('/dashboard') // Brak uprawnień -> wykop na dashboard
    }
  }

  next()
})

export default router