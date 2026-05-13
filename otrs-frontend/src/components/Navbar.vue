<template>
  <nav class="sticky top-0 z-50 w-full bg-white shadow-md">
    <div class="max-w-6xl mx-auto px-6 py-4 md:flex md:items-center">
      <div class="flex items-center justify-between w-full md:w-auto">
        <div
          @click="goToDashboard"
          class="text-2xl font-bold text-przyciskiNiebieski tracking-tight cursor-pointer whitespace-nowrap"
        >
          Hustletrack ITSM
        </div>

        <button
          @click="toggleMenu"
          class="md:hidden text-gray-500 hover:text-gray-700 p-1 focus:outline-none cursor-pointer"
        >
          <svg
            v-if="!isOpen"
            xmlns="http://www.w3.org/2000/svg"
            width="28"
            height="28"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <line x1="4" x2="20" y1="12" y2="12" />
            <line x1="4" x2="20" y1="6" y2="6" />
            <line x1="4" x2="20" y1="18" y2="18" />
          </svg>
          <svg
            v-else
            xmlns="http://www.w3.org/2000/svg"
            width="28"
            height="28"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <path d="M18 6 6 18" />
            <path d="m6 6 12 12" />
          </svg>
        </button>
      </div>

      <div
        v-if="isAuthenticated"
        :class="[
          'md:flex md:flex-1 md:items-center md:justify-between md:ml-8 md:opacity-100 md:max-h-full md:mt-0',
          'transition-all duration-300 ease-in-out overflow-hidden',
          isOpen ? 'max-h-[500px] opacity-100 mt-5' : 'max-h-0 opacity-0',
        ]"
      >
        <div class="flex flex-col gap-3 md:flex-row md:gap-2">
  <button 
    v-for="item in menuItems" 
    :key="item.id"
    @click="handleNavigation(item.id)"
    :class="[
      'flex items-center gap-2 px-4 py-2 text-sm font-medium rounded transition-all duration-200 w-full md:w-auto cursor-pointer',
      activeTab === item.id
        ? 'bg-przyciskiNiebieski text-white shadow-sm'
        : 'text-gray-500 hover:text-gray-800 hover:bg-gray-100',
      item.id === 'admin' ? 'border border-red-100 text-red-600 hover:bg-red-50' : '',
    ]"
  >
    <component :is="item.icon" />
    {{ item.label }}
  </button>

  <button
    @click="goToCreateTicket"
    :class="[
      'flex items-center gap-2 px-4 py-2 text-sm font-medium rounded transition-all duration-200 w-full md:w-auto cursor-pointer',
      isCreateTicketActive
        ? 'bg-przyciskiNiebieski text-white shadow-sm'
        : 'text-gray-500 hover:text-gray-800 hover:bg-gray-100'
    ]"
  >
    <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
      <path d="M5 12h14"/>
      <path d="M12 5v14"/>
    </svg>
    Utwórz zgłoszenie
  </button>
</div>

        <div class="flex flex-col md:flex-row items-center gap-4 mt-4 md:mt-0">
          <div class="border-t border-gray-300 w-full md:hidden my-2"></div>

          <div v-if="isAuthenticated" class="relative w-full md:w-auto flex justify-center md:block">
            <button
              @click="goToNotifications"
              class="relative flex items-center justify-center p-2 text-gray-500 hover:text-przyciskiNiebieski transition-colors rounded-full hover:bg-gray-100 cursor-pointer focus:outline-none"
              title="Powiadomienia"
            >
              <IconBell />
              <span v-if="unreadCount > 0" class="absolute top-0 right-0 inline-flex items-center justify-center px-1.5 py-0.5 text-[10px] font-bold leading-none text-white bg-red-500 rounded-full">
                {{ unreadCount > 99 ? '99+' : unreadCount }}
              </span>
            </button>
          </div>

          <button
            @click="goToProfile"
            class="flex items-center gap-2 px-4 py-2 text-sm font-medium text-gray-500 hover:text-przyciskiNiebieski transition-all duration-200 w-full md:w-auto cursor-pointer"
          >
            <IconUser />
            <span>Moje konto</span>
          </button>

          <button
            @click="handleLogout"
            class="flex items-center justify-center w-full md:w-auto p-1.5 text-gray-500 bg-white border border-gray-200 rounded hover:bg-gray-100 hover:text-red-500 transition-colors gap-2 cursor-pointer"
          >
            <IconLogout />
            <span class="md:hidden text-sm font-medium">Wyloguj się</span>
          </button>
        </div>
      </div>

      <div v-else class="md:flex md:items-center md:ml-auto">
        <button
          @click="goToLogin"
          class="flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-przyciskiNiebieski rounded-lg hover:opacity-90 transition-all duration-200 cursor-pointer"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="18"
            height="18"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <path d="M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4" />
            <polyline points="10 17 15 12 10 7" />
            <line x1="15" x2="3" y1="12" y2="12" />
          </svg>
          <span>Zaloguj się</span>
        </button>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, h, computed, watch, onMounted, onUnmounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useUserStore } from '@/stores/user'
import api from '@/services/api';

const router = useRouter()
const route = useRoute()
const userStore = useUserStore()

const IconDashboard = () =>
  h(
    'svg',
    {
      xmlns: 'http://www.w3.org/2000/svg',
      width: '18',
      height: '18',
      viewBox: '0 0 24 24',
      fill: 'none',
      stroke: 'currentColor',
      'stroke-width': '2',
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
    },
    [
      h('path', { d: 'M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z' }),
      h('polyline', { points: '9 22 9 12 15 12 15 22' }),
    ],
  )
const IconTicket = () =>
  h(
    'svg',
    {
      xmlns: 'http://www.w3.org/2000/svg',
      width: '18',
      height: '18',
      viewBox: '0 0 24 24',
      fill: 'none',
      stroke: 'currentColor',
      'stroke-width': '2',
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
    },
    [
      h('path', { d: 'M2 16l4 4 4-4' }),
      h('path', { d: 'M4 12V4h16v8' }),
      h('path', { d: 'M10 20h8v-8' }),
    ],
  )
const IconPending = () =>
  h(
    'svg',
    {
      xmlns: 'http://www.w3.org/2000/svg',
      width: '18',
      height: '18',
      viewBox: '0 0 24 24',
      fill: 'none',
      stroke: 'currentColor',
      'stroke-width': '2',
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
    },
    [
      h('circle', { cx: '12', cy: '12', r: '10' }),
      h('polyline', { points: '12 6 12 12 16 14' }),
    ],
  )
const IconAdmin = () =>
  h(
    'svg',
    {
      xmlns: 'http://www.w3.org/2000/svg',
      width: '18',
      height: '18',
      viewBox: '0 0 24 24',
      fill: 'none',
      stroke: 'currentColor',
      'stroke-width': '2',
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
    },
    [h('path', { d: 'M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z' })],
  )
const IconLogout = () =>
  h(
    'svg',
    {
      xmlns: 'http://www.w3.org/2000/svg',
      width: '20',
      height: '20',
      viewBox: '0 0 24 24',
      fill: 'none',
      stroke: 'currentColor',
      'stroke-width': '2',
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
    },
    [
      h('path', { d: 'M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4' }),
      h('polyline', { points: '16 17 21 12 16 7' }),
      h('line', { x1: '21', x2: '9', y1: '12', y2: '12' }),
    ],
  )
const IconUser = () =>
  h(
    'svg',
    {
      xmlns: 'http://www.w3.org/2000/svg',
      width: '20',
      height: '20',
      viewBox: '0 0 24 24',
      fill: 'none',
      stroke: 'currentColor',
      'stroke-width': '2',
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
    },
    [
      h('path', { d: 'M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2' }),
      h('circle', { cx: '12', cy: '7', r: '4' }),
    ],
  )

const IconBell = () =>
  h(
    'svg',
    {
      xmlns: 'http://www.w3.org/2000/svg',
      width: '20',
      height: '20',
      viewBox: '0 0 24 24',
      fill: 'none',
      stroke: 'currentColor',
      'stroke-width': '2',
      'stroke-linecap': 'round',
      'stroke-linejoin': 'round',
    },
    [
      h('path', { d: 'M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9' }),
      h('path', { d: 'M13.73 21a2 2 0 0 1-3.46 0' }),
    ]
  )

const isOpen = ref(false)
const activeTab = ref('dashboard')

const isAuthenticated = computed(() => userStore.isAuthenticated)

const isAdmin = computed(() => {
  return userStore.user?.roles?.includes('Admin') || false
})

const menuItems = computed(() => {
  const items = [
    { id: 'dashboard', label: 'Dashboard', icon: IconDashboard },
    { id: 'myTickets', label: 'Zgłoszenia', icon: IconTicket },
  ]
  const userRoles = userStore.user?.roles || []
  const hasPendingAccess = userRoles.includes('Admin') || userRoles.includes('Helpdesk')
  if (hasPendingAccess) {
    items.push({ id: 'pendingTickets', label: 'Oczekujące', icon: IconPending })
  }
  if (isAdmin.value) {
    items.push({ id: 'admin', label: 'Admin', icon: IconAdmin })
  }
  return items
})

watch(
  () => route.name,
  (newRouteName) => {
    if (newRouteName) activeTab.value = newRouteName
  },
  { immediate: true },
)

const toggleMenu = () => {
  isOpen.value = !isOpen.value
}

const handleNavigation = (id) => {
  activeTab.value = id
  isOpen.value = false
  router.push({ name: id })
}

const goToDashboard = () => {
  if (isAuthenticated.value) {
    router.push({ name: 'dashboard' })
  } else {
    router.push({ name: 'login' })
  }
}

const goToProfile = () => {
  isOpen.value = false
  router.push({ name: 'profile' })
}

const goToLogin = () => {
  isOpen.value = false
  router.push({ name: 'login' })
}

const handleLogout = async () => {
  try {
    await fetch('/api/Auth/logout', {
      method: 'POST',
      credentials: 'include',
    })
  } catch (error) {
  } finally {
    userStore.clearUser()
    isOpen.value = false
    router.push({ name: 'login' })
  }
}
const isCreateTicketActive = computed(() => {
  return route.name === 'problemReportClient' || route.name === 'problemReportHelpdesk'
})

const goToCreateTicket = () => {
  isOpen.value = false
  const user = userStore.user
  if (user?.roles?.includes('Helpdesk') || user?.roles?.includes('Admin') || user?.roles?.includes('Technik')) {
    router.push({ name: 'problemReportHelpdesk' })
  } else {
    router.push({ name: 'problemReportClient' })
  }
}

const notifications = ref([])
const unreadCount = computed(() => notifications.value.filter(n => !n.isRead).length)

const goToNotifications = () => {
  isOpen.value = false
  router.push({ name: 'notifications' })
}

const fetchNotifications = async () => {
  if (!isAuthenticated.value) return
  try {
    const response = await api.get('/api/notifications', { withCredentials: true })
    notifications.value = response.data
  } catch (error) {
  }
}

let pollInterval = null
onMounted(() => {
  if (isAuthenticated.value) fetchNotifications()
  pollInterval = setInterval(() => {
    if (isAuthenticated.value) fetchNotifications()
  }, 30000)
})

onUnmounted(() => {
  if (pollInterval) clearInterval(pollInterval)
})

watch(isAuthenticated, (newVal) => {
  if (newVal) {
    fetchNotifications()
  } else {
    notifications.value = []
  }
})


</script>
