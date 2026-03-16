<template>
  <div class="min-h-screen bg-bialeTlo">
    <div class="flex flex-col container max-w-7xl mx-auto p-6 space-y-8">
      <div class="flex flex-col">
        <h2 class="text-3xl font-bold text-tekstSzaryCiemny">Witaj,</h2>
        <p class="text-tekstSzary">Przegląd systemu zgłoszeń</p>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div
          v-for="(stat, index) in stats"
          :key="index"
          class="flex flex-col border border-gray-300 rounded-lg p-6 bg-white shadow-sm hover:shadow-md transition-shadow"
        >
          <div class="flex justify-between items-start mb-4">
            <h2 class="font-medium text-tekstSzaryCiemny">{{ stat.title }}</h2>
            <div v-html="stat.icon" :class="stat.iconColor"></div>
          </div>

          <div class="mt-auto">
            <p class="text-3xl font-bold text-tekstSzaryCiemny mb-1">{{ stat.value }}</p>
            <p class="text-sm text-tekstSzary">{{ stat.desc }}</p>
          </div>
        </div>
      </div>

      <div
        class="border border-gray-300 rounded-lg p-6 bg-white shadow-sm hover:shadow-md transition-shadow"
      >
        <h3 class="text-xl font-bold mb-1 text-tekstSzaryCiemny">Szybkie akcje</h3>
        <p class="text-tekstSzary mb-6">Najczęściej używane funkcje systemu</p>

        <div class="flex flex-wrap gap-4">
          <button
            @click="createTicket"
            class="flex items-center gap-2 bg-przyciskiNiebieski text-white px-5 py-2.5 rounded-lg hover:opacity-90 transition font-medium"
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
              <path
                d="M2 9a3 3 0 0 1 0 6v2a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-2a3 3 0 0 1 0-6V7a2 2 0 0 0-2-2H4a2 2 0 0 0-2 2Z"
              />
              <path d="M13 5v2" />
              <path d="M13 17v2" />
              <path d="M13 11v2" />
            </svg>
            Utwórz zgłoszenie
          </button>

          <button
            @click="viewTickets"
            class="border border-gray-300 text-tekstSzaryCiemny px-5 py-2.5 rounded-lg hover:bg-gray-50 transition font-medium"
          >
            Przeglądaj zgłoszenia
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'

const router = useRouter()
const userStore = useUserStore()

const createTicket = () => {
  const user = userStore.user
  if (user?.roles?.includes('Helpdesk') || user?.roles?.includes('Admin')) {
    router.push({ name: 'problemReportHelpdesk' })
  } else {
    router.push({ name: 'problemReportClient' })
  }
}

const viewTickets = () => {
  router.push({ name: 'myTickets' })
}

const API_URL = 'https://localhost:7054/api/ticket'

const normalizeLabel = (value) => {
  return (value || '')
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .toLowerCase()
    .trim()
}

const loadDashboardStats = async () => {
  try {
    const ticketsResponse = await fetch(API_URL, {
      credentials: 'include',
      headers: { 'Content-Type': 'application/json' },
    })

    if (!ticketsResponse.ok) {
      throw new Error('Nie udało się pobrać zgłoszeń do dashboardu.')
    }

    const tickets = await ticketsResponse.json()
    const safeTickets = Array.isArray(tickets) ? tickets : []

    const now = Date.now()
    const SLA_LIMIT_MS = 48 * 60 * 60 * 1000 // 48 godzin

    const isNowy = (ticket) => normalizeLabel(ticket.status ?? ticket.Status) === 'nowy'
    const isRozwiazane = (ticket) => normalizeLabel(ticket.status ?? ticket.Status) === 'rozwiazane'

    const total = safeTickets.length
    const newCount = safeTickets.filter((t) => isNowy(t)).length
    const inProgressCount = safeTickets.filter((t) => !isNowy(t) && !isRozwiazane(t)).length
    const slaBreachCount = safeTickets.filter((t) => {
      const createdAtMs = new Date(t.createdAt ?? t.CreatedAt).getTime()
      if (Number.isNaN(createdAtMs)) return false
      return now - createdAtMs > SLA_LIMIT_MS
    }).length

    stats.value[0].value = total
    stats.value[1].value = newCount
    stats.value[2].value = inProgressCount
    stats.value[3].value = slaBreachCount
  } catch (error) {
    console.error('Błąd ładowania statystyk dashboardu:', error)
  }
}

onMounted(() => {
  loadDashboardStats()
})

const stats = ref([
  {
    title: 'Wszystkie zgłoszenia',
    value: 0,
    desc: 'Łączna liczba zgłoszeń',
    iconColor: 'text-placeholder',
    icon: `<svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 9a3 3 0 0 1 0 6v2a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-2a3 3 0 0 1 0-6V7a2 2 0 0 0-2-2H4a2 2 0 0 0-2 2Z"/><path d="M13 5v2"/><path d="M13 17v2"/><path d="M13 11v2"/></svg>`,
  },
  {
    title: 'Nowe',
    value: 0,
    desc: 'Oczekujące na przydzielenie',
    iconColor: 'text-blue-500',
    icon: `<svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><path d="M12 16v-4"/><path d="M12 8h.01"/></svg>`,
  },
  {
    title: 'W trakcie',
    value: 0,
    desc: 'Aktualnie realizowane',
    iconColor: 'text-teal-500',
    icon: `<svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><path d="M12 6v6l4 2"/></svg>`,
  },
  {
    title: 'Przekroczenie SLA',
    value: 0,
    desc: 'Wymagają eskalacji',
    iconColor: 'text-red-500',
    icon: `<svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>`,
  },
])
</script>

<style scoped></style>