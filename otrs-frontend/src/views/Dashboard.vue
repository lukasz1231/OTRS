<template>
  <div class="min-h-screen bg-bialeTlo">
    <div class="flex flex-col container max-w-7xl mx-auto p-6 space-y-8">

      <div class="flex flex-col">
        <h2 class="text-3xl font-bold text-tekstSzaryCiemny">Witaj, {{ userName }}!</h2>
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

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">

        <div class="lg:col-span-2 border border-gray-300 rounded-lg p-6 bg-white shadow-sm hover:shadow-md transition-shadow">
          <h3 class="text-xl font-bold mb-1 text-tekstSzaryCiemny">Zgłoszenia w ostatnich 7 dniach</h3>
          <p class="text-tekstSzary mb-4 text-sm">Liczba zgłoszeń według daty utworzenia</p>
          <div class="relative" style="height: 220px;">
            <canvas ref="chartCanvas"></canvas>
          </div>
        </div>

        <div class="border border-gray-300 rounded-lg p-6 bg-white shadow-sm hover:shadow-md transition-shadow flex flex-col">
          <h3 class="text-xl font-bold mb-1 text-tekstSzaryCiemny">Szybkie akcje</h3>
          <p class="text-tekstSzary mb-6 text-sm">Najczęściej używane funkcje systemu</p>

          <div class="flex flex-col gap-3 mt-auto">
            <button
              @click="createTicket"
              class="flex items-center gap-2 bg-przyciskiNiebieski text-white px-5 py-2.5 rounded-lg hover:opacity-90 transition font-medium justify-center"
            >
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M2 9a3 3 0 0 1 0 6v2a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-2a3 3 0 0 1 0-6V7a2 2 0 0 0-2-2H4a2 2 0 0 0-2 2Z"/>
                <path d="M13 5v2"/><path d="M13 17v2"/><path d="M13 11v2"/>
              </svg>
              Utwórz zgłoszenie
            </button>

            <button
              @click="viewTickets"
              class="flex items-center gap-2 border border-gray-300 text-tekstSzaryCiemny px-5 py-2.5 rounded-lg hover:bg-gray-50 transition font-medium justify-center"
            >
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M9 11l3 3L22 4"/><path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"/>
              </svg>
              Przeglądaj zgłoszenia
            </button>
          </div>
        </div>
      </div>

      <div class="border border-gray-300 rounded-lg bg-white shadow-sm hover:shadow-md transition-shadow overflow-hidden">
        <div class="p-6 pb-3">
          <h3 class="text-xl font-bold mb-1 text-tekstSzaryCiemny">Ostatnie zgłoszenia</h3>
          <p class="text-tekstSzary text-sm">5 ostatnio dodanych zgłoszeń</p>
        </div>

        <div v-if="recentTickets.length === 0" class="p-6 pt-2 text-tekstSzary text-sm">
          Brak zgłoszeń do wyświetlenia.
        </div>

        <table v-else class="w-full text-sm">
          <thead>
            <tr class="border-t border-gray-100 bg-gray-50 text-left">
              <th class="px-6 py-3 font-medium text-tekstSzary">ID</th>
              <th class="px-6 py-3 font-medium text-tekstSzary">Tytuł</th>
              <th class="px-6 py-3 font-medium text-tekstSzary">Status</th>
              <th class="px-6 py-3 font-medium text-tekstSzary">Data</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="ticket in recentTickets"
              :key="ticket.id ?? ticket.Id"
              @click="goToTicket(ticket.id ?? ticket.Id)"
              class="border-t border-gray-100 hover:bg-gray-50 cursor-pointer transition-colors"
            >
              <td class="px-6 py-3 text-tekstSzaryCiemny font-mono">#{{ ticket.id ?? ticket.Id }}</td>
              <td class="px-6 py-3 text-tekstSzaryCiemny max-w-xs truncate">{{ ticket.title ?? ticket.Title ?? '—' }}</td>
              <td class="px-6 py-3">
                <span :class="statusBadgeClass(ticket.status ?? ticket.Status)" class="px-2 py-0.5 rounded-full text-xs font-medium">
                  {{ ticket.status ?? ticket.Status ?? '—' }}
                </span>
              </td>
              <td class="px-6 py-3 text-tekstSzary">{{ formatDate(ticket.createdAt ?? ticket.CreatedAt) }}</td>
            </tr>
          </tbody>
        </table>
      </div>

    </div>
  </div>
</template>

<script setup>
import { onMounted, onBeforeUnmount, ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'
import { Chart, LineController, LineElement, PointElement, LinearScale, CategoryScale, Tooltip, Filler } from 'chart.js'

Chart.register(LineController, LineElement, PointElement, LinearScale, CategoryScale, Tooltip, Filler)

const router = useRouter()
const userStore = useUserStore()

const chartCanvas = ref(null)
let chartInstance = null

const userName = computed(() => {
  const u = userStore.user
  if (!u) return ''
  return u.name ?? u.Name ?? u.email ?? u.Email ?? ''
})

const createTicket = () => {
  const user = userStore.user
  if (user?.roles?.includes('Helpdesk') || user?.roles?.includes('Admin') || user?.roles?.includes('Technik')) {
    router.push({ name: 'problemReportHelpdesk' })
  } else {
    router.push({ name: 'problemReportClient' })
  }
}

const viewTickets = () => {
  router.push({ name: 'myTickets' })
}

const goToTicket = (id) => {
  router.push({ name: 'ticket-details', params: { id } })
}

const API_URL = 'https://localhost:7054/api/ticket'

const normalizeLabel = (value) => {
  return (value || '')
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .toLowerCase()
    .trim()
}

const parseUtcDate = (value) => {
  if (!value) return null
  if (value instanceof Date) return value
  if (typeof value !== 'string') return new Date(value)

  const hasTimezone = /[zZ]$|[+-]\d{2}:\d{2}$/.test(value)
  const normalized = hasTimezone ? value : `${value}Z`
  return new Date(normalized)
}

const formatDate = (val) => {
  if (!val) return '—'
  const d = parseUtcDate(val)
  if (isNaN(d)) return '—'
  return d.toLocaleDateString('pl-PL', {
    timeZone: 'Europe/Warsaw',
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })
}

const statusBadgeClass = (status) => {
  const s = normalizeLabel(status)
  if (s === 'nowy') return 'bg-blue-100 text-blue-700'
  if (s === 'rozwiazane' || s === 'zamkniety' || s === 'zamknięty') return 'bg-green-100 text-green-700'
  if (s === 'w realizacji' || s === 'w trakcie') return 'bg-teal-100 text-teal-700'
  return 'bg-gray-100 text-gray-600'
}

const recentTickets = ref([])

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

const buildChart = (tickets) => {
  if (!chartCanvas.value) return

  // Ostatnie 7 dni
  const days = []
  const counts = []
  for (let i = 6; i >= 0; i--) {
    const d = new Date()
    d.setDate(d.getDate() - i)
    const label = d.toLocaleDateString('pl-PL', {
      timeZone: 'Europe/Warsaw',
      day: '2-digit',
      month: '2-digit',
    })
    const dateStr = d.toISOString().slice(0, 10) // YYYY-MM-DD
    const count = tickets.filter((t) => {
      const created = t.createdAt ?? t.CreatedAt
      if (!created) return false
      const createdDate = parseUtcDate(created)
      if (!createdDate || Number.isNaN(createdDate.getTime())) return false
      return createdDate.toISOString().slice(0, 10) === dateStr
    }).length
    days.push(label)
    counts.push(count)
  }

  if (chartInstance) {
    chartInstance.destroy()
  }

  chartInstance = new Chart(chartCanvas.value, {
    type: 'line',
    data: {
      labels: days,
      datasets: [
        {
          label: 'Zgłoszenia',
          data: counts,
          borderColor: '#2563EB',
          backgroundColor: 'rgba(37, 99, 235, 0.08)',
          borderWidth: 2,
          pointBackgroundColor: '#2563EB',
          pointRadius: 4,
          pointHoverRadius: 6,
          fill: true,
          tension: 0.35,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { display: false },
        tooltip: {
          callbacks: {
            label: (ctx) => ` ${ctx.parsed.y} zgłoszenie${ctx.parsed.y === 1 ? '' : ctx.parsed.y < 5 ? 'a' : 'ń'}`,
          },
        },
      },
      scales: {
        x: {
          grid: { display: false },
          ticks: { color: '#6b7280', font: { size: 12 } },
        },
        y: {
          beginAtZero: true,
          ticks: {
            stepSize: 1,
            color: '#6b7280',
            font: { size: 12 },
            callback: (v) => (Number.isInteger(v) ? v : ''),
          },
          grid: { color: 'rgba(0,0,0,0.05)' },
        },
      },
    },
  })
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

    const isNowy = (ticket) => normalizeLabel(ticket.status ?? ticket.Status) === 'nowy'
    const isRozwiazane = (ticket) => normalizeLabel(ticket.status ?? ticket.Status) === 'rozwiazane'

    const total = safeTickets.length
    const newCount = safeTickets.filter((t) => isNowy(t)).length
    const inProgressCount = safeTickets.filter((t) => !isNowy(t) && !isRozwiazane(t)).length
    const slaBreachCount = safeTickets.filter((t) => {
      const breachFlag = t.isSlaBreached ?? t.IsSlaBreached
      return breachFlag === true
    }).length

    stats.value[0].value = total
    stats.value[1].value = newCount
    stats.value[2].value = inProgressCount
    stats.value[3].value = slaBreachCount

    recentTickets.value = [...safeTickets]
      .sort((a, b) => {
        const da = parseUtcDate(a.createdAt ?? a.CreatedAt)?.getTime() ?? 0
        const db = parseUtcDate(b.createdAt ?? b.CreatedAt)?.getTime() ?? 0
        return db - da
      })
      .slice(0, 5)

    buildChart(safeTickets)
  } catch (error) {
    console.error('Błąd ładowania statystyk dashboardu:', error)
  }
}

onMounted(() => {
  loadDashboardStats()
})

onBeforeUnmount(() => {
  if (chartInstance) {
    chartInstance.destroy()
    chartInstance = null
  }
})
</script>

<style scoped></style>