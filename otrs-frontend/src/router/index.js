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
import AdminStatuses from '@/views/AdminStatuses.vue'
import AdminPriorities from '@/views/AdminPriorities.vue'
import AdminCategories from '@/views/AdminCategories.vue'
import MyTickets from '@/views/MyTickets.vue'
import PendingTickets from '@/views/PendingTickets.vue'
import AdminClients from '@/views/AdminClients.vue'
import AdminTypes from '@/views/AdminTypes.vue'
import NotificationsPage from '@/views/NotificationsPage.vue'

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
      meta: { requiresGuest: true, title: 'Logowanie – OTRS' }
    },
    {
      path: '/register',
      name: 'register',
      component: LoginRegisterPage,
      meta: { requiresGuest: true, title: 'Rejestracja – OTRS' }
    },
    {
      path: '/forgot-password',
      name: 'forgot-password',
      component: ForgotPasswordPage,
      meta: { requiresGuest: true, title: 'Resetowanie hasła – OTRS' }
    },
    {
      path: '/reset-password',
      name: 'reset-password',
      component: ResetPasswordPage,
      meta: { title: 'Nowe hasło – OTRS' }
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: Dashboard,
      meta: { requiresAuth: true, title: 'Dashboard – OTRS' }
    },
    {
      path: '/problemReportHelpdesk',
      name: 'problemReportHelpdesk',
      component: ProblemReportHelpdesk,
      meta: { requiresAuth: true, title: 'Nowe zgłoszenie (Helpdesk) – OTRS' }
    },
    {
      path: '/problemReportClient',
      name: 'problemReportClient',
      component: ProblemReportClient,
      meta: { requiresAuth: true, title: 'Nowe zgłoszenie – OTRS' }
    },
    {
      path: '/profile',
      name: 'profile',
      component: ProfilePage,
      meta: { requiresAuth: true, title: 'Profil – OTRS' }
    },
    {
      path: '/privacy',
      name: 'privacy',
      component: PrivacyPage,
      meta: { title: 'Polityka prywatności – OTRS' }
    },
    {
      path: '/terms',
      name: 'terms',
      component: TermsPage,
      meta: { title: 'Regulamin – OTRS' }
    },
    {
      path: '/trademark',
      name: 'trademark',
      component: TrademarksPage,
      meta: { title: 'Znaki towarowe – OTRS' }
    },
    {
      path: '/about',
      name: 'about',
      component: AboutPage,
      meta: { title: 'O systemie – OTRS' }
    },
    {
      path: '/contact',
      name: 'contact',
      component: ContactPage,
      meta: { title: 'Kontakt – OTRS' }
    },
    {
      path: '/',
      redirect: '/dashboard'
    },
    {
      path: '/admin',
      name: 'admin',
      component: AdminDashboard,
      meta: { requiresAuth: true, requiresAdmin: true, title: 'Administracja – OTRS' }
    },
    {
      path: '/admin/users',
      name: 'admin-users',
      component: AdminUsers,
      meta: { requiresAuth: true, requiresAdmin: true, title: 'Użytkownicy – OTRS' }
    },
    {
      path: '/admin/queues',
      name: 'admin-queues',
      component: AdminQueues,
      meta: { requiresAuth: true, requiresAdmin: true, title: 'Kolejki – OTRS' }
    },
    {
      path: '/admin/statuses',
      name: 'admin-statuses',
      component: AdminStatuses,
      meta: { requiresAuth: true, requiresAdmin: true, title: 'Statusy – OTRS' }
    },
    {
      path: '/admin/priorities',
      name: 'admin-priorities',
      component: AdminPriorities,
      meta: { requiresAuth: true, requiresAdmin: true, title: 'Priorytety – OTRS' }
    },
    {
      path: '/admin/categories',
      name: 'admin-categories',
      component: AdminCategories,
      meta: { requiresAuth: true, requiresAdmin: true, title: 'Kategorie – OTRS' }
    },
    {
      path: '/admin/clietns',
      name: 'admin-clients',
      component: AdminClients,
      meta: { requiresAuth: true, requiresAdmin: true, title: 'Klienci – OTRS' }
    },
    {
      path: '/admin/types',
      name: 'admin-types',
      component: AdminTypes,
      meta: { requiresAuth: true, requiresAdmin: true, title: 'Typy – OTRS' }
    },
    {
      path: '/ticket/:id',
      name: 'ticket-details',
      component: () => import('../views/TicketDetailsView.vue'),
      props: true,
      meta: { title: 'Szczegóły zgłoszenia – OTRS' }
    },
    {
      path: '/my-tickets',
      name: 'myTickets',
      component: MyTickets,
      meta: { requiresAuth: true, title: 'Moje zgłoszenia – OTRS' }
    },
    {
      path: '/pending-tickets',
      name: 'pendingTickets',
      component: PendingTickets,
      meta: { requiresAuth: true, title: 'Oczekujące zgłoszenia – OTRS' }
    },
    {
      path: '/notifications',
      name: 'notifications',
      component: NotificationsPage,
      meta: { requiresAuth: true, title: 'Powiadomienia – OTRS' }
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
   if (to.name === 'pendingTickets') {
    const hasPendingAccess = userStore.user?.roles?.some(role => 
      ['Admin', 'Helpdesk'].includes(role)
    )
    if (!hasPendingAccess) {
      return next({ name: 'dashboard' })
    }
  }

  if (to.name === 'problemReportHelpdesk') {
    const hasHelpdeskAccess = userStore.user?.roles?.some(role => 
      ['Admin', 'Helpdesk', 'Technik'].includes(role)
    )
    if (!hasHelpdeskAccess) {
      return next({ name: 'problemReportClient' })
    }
  }

  next()
})

router.afterEach((to) => {
  document.title = to.meta?.title ?? 'OTRS'
})

export default router