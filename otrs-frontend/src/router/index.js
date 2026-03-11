import { createRouter, createWebHistory } from 'vue-router'
import { useUserStore } from '@/stores/user'

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
    },
    {
      path: '/ticket/:id',
      name: 'ticket-details',
      component: () => import('../views/TicketDetailsView.vue'),
      props: true
    }
  ],
})


router.beforeEach(async (to, from, next) => {
  const userStore = useUserStore()

  if (!userStore.isSessionChecked) {
    await userStore.fetchCurrentUser()
  }

  const isAuthenticated = userStore.isAuthenticated
  const isAdmin = userStore.user?.roles?.includes('Admin') || false

  if (to.meta.requiresAuth && !isAuthenticated) {
    return next({ name: 'login' })
  }

  if (to.meta.requiresGuest && isAuthenticated) {
    return next({ name: 'dashboard' })
  }

  if (to.meta.requiresAdmin && !isAdmin) {
    return next({ name: 'dashboard' })
  }
  next()
})

export default router