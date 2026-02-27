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


const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  
  // kontrola przewijania - scroll na gore strony lub powrot w to samo miejsce po przycisku wstecz
  scrollBehavior(to, from, savedPosition) {
    // jesli wstecz to wroc
    if (savedPosition) {
      return savedPosition
    }
    // jezeli nie to na sama gore
    return { top: 0, behavior: 'smooth' }
  },
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
    {
      path: '/dashboard',
      name: 'dashboard',
      component: Dashboard,
    },
    {
      path: '/problemReportHelpdesk',
      name: 'problemReportHelpdesk',
      component: ProblemReportHelpdesk,
    },
    {
      path: '/problemReportClient',
      name: 'problemReportClient',
      component: ProblemReportClient,
    },
    {
      path: '/profile',
      name: 'profile',
      component: ProfilePage,
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
  ],
})



export default router
