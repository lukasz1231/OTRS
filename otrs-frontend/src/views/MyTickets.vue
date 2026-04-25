<template>
  <div class="min-h-screen bg-bialeTlo">
    <div class="max-w-6xl mx-auto px-6 py-12">
      <div class="bg-white p-8 rounded-2xl shadow-sm border border-gray-100">

        <div class="flex justify-between items-center mb-8">
          <div>
            <h1 class="text-3xl font-bold text-przyciskiNiebieski">Moje zgłoszenia</h1>
            <p class="text-gray-500 mt-1">Lista wszystkich Twoich zgłoszeń</p>
          </div>

          <button @click="goToCreateTicket"
            class="flex items-center gap-2 bg-przyciskiNiebieski text-white px-5 py-2.5 rounded-lg hover:opacity-90 transition font-medium">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none"
              stroke="currentColor" stroke-width="2.5">
              <path d="M5 12h14" />
              <path d="M12 5v14" />
            </svg>
            Nowe zgłoszenie
          </button>
        </div>

        <div class="flex flex-col md:flex-row gap-4 mb-8">
          <div class="flex flex-wrap gap-4 mb-4">
            <div class="flex bg-gray-100 p-1 rounded-xl">
              <button v-for="filter in statusFilters" :key="filter" @click="activeStatusFilter = filter" :class="[
                'px-5 py-2 text-sm font-medium rounded-lg transition-all cursor-pointer',
                activeStatusFilter === filter
                  ? 'bg-white text-przyciskiNiebieski shadow-sm'
                  : 'text-gray-500 hover:text-gray-700'
              ]">
                {{ filter }}
              </button>
            </div>

            <div class="flex bg-gray-100 p-1 rounded-xl">
              <button v-for="priority in priorityFilters" :key="priority" @click="activePriorityFilter = priority"
                :class="[
                  'px-5 py-2 text-sm font-medium rounded-lg transition-all cursor-pointer',
                  activePriorityFilter === priority
                    ? 'bg-white text-przyciskiNiebieski shadow-sm'
                    : 'text-gray-500 hover:text-gray-700'
                ]">
                {{ priority }}
              </button>
            </div>
          </div>

          <div class="relative flex-grow">
            <input v-model="searchQuery" type="text" placeholder="Szukaj po ID lub tytule..."
              class="w-full pl-10 pr-4 py-2 rounded-xl border border-gray-200 focus:outline-none focus:ring-2 focus:ring-przyciskiNiebieski" />
            <span class="absolute left-3 top-2.5 text-gray-400">
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none"
                stroke="currentColor" stroke-width="2">
                <circle cx="11" cy="11" r="8" />
                <path d="m21 21-4.3-4.3" />
              </svg>
            </span>
          </div>
        </div>

        <div v-if="filteredTickets.length === 0" class="text-center py-20 bg-gray-50 rounded-xl">
          <p class="text-gray-500">Brak zgłoszeń</p>
        </div>

        <div v-else class="space-y-4">
          <div v-for="ticket in filteredTickets" :key="ticket.id"
            class="border border-gray-200 rounded-xl overflow-hidden hover:border-przyciskiNiebieski/50 transition-colors">
            <div class="bg-przyciskiNiebieski text-white px-6 py-3 flex justify-between items-center">
              <span class="font-medium">{{ ticket.title }}</span>
              <span class="text-xs font-mono opacity-80">ID: {{ ticket.publicId }}</span>
            </div>

            <div class="p-6 bg-white">
              <p class="text-gray-600 mb-4">{{ ticket.description }}</p>
              <div class="flex flex-wrap gap-4 text-sm">
                <span class="px-3 py-1 bg-gray-100 rounded-full text-gray-600">
                  {{ ticket.type }}
                </span>
                <span :class="getPriorityClass(ticket.priority)" class="px-3 py-1 rounded-full">
                  {{ ticket.priority }}
                </span>

                <StatusBadge :status="ticket.status" />

                <span :class="getSlaBadgeClass(ticket.slaState ?? ticket.SlaState)"
                  class="px-3 py-1 rounded-full font-medium">
                  {{ getSlaLabel(ticket.slaState ?? ticket.SlaState) }}
                </span>
                <span class="text-gray-400">
                  {{ formatDate(ticket.createdAt) }}
                </span>
              </div>
            </div>

            <div class="bg-gray-50 px-6 py-3 flex justify-end border-t border-gray-100">
              <button @click="router.push({ name: 'ticket-details', params: { id: ticket.publicId } })"
                class="flex items-center gap-2 px-5 py-2 bg-white text-[#3B71A3] border border-gray-200 rounded-lg text-xs font-bold uppercase tracking-wider hover:bg-[#3B71A3] hover:text-white hover:border-[#3B71A3] transition-all active:scale-95 cursor-pointer shadow-sm">
                <span>Zobacz szczegóły</span>
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none"
                  stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M5 12h14m-7-7 7 7-7 7" />
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import StatusBadge from '@/components/StatusBadge.vue'
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'
import axios from 'axios'

const router = useRouter()
const userStore = useUserStore()

const statusFilters = ['Wszystkie', 'W toku', 'Zakończone']
const priorityFilters = ['Wszystkie', 'Niski', 'Średni', 'Wysoki', 'Krytyczny']

const activeStatusFilter = ref('Wszystkie')
const activePriorityFilter = ref('Wszystkie')
const searchQuery = ref('')
const tickets = ref([])

const activeFilter = ref('Wszystkie')

const axiosConfig = { withCredentials: true }

const fetchMyTickets = async () => {
  try {
    const response = await axios.get('/api/ticket', axiosConfig)
    tickets.value = Array.isArray(response.data) ? response.data : []
  } catch (error) {
    console.error('Błąd podczas pobierania zgłoszeń:', error)
    tickets.value = []
  }
}

onMounted(() => {
  fetchMyTickets()
})

const filteredTickets = computed(() => {
  return tickets.value.filter(ticket => {
    // Normalizacja przeniesiona z zewnątrz, służy teraz tylko do logicznego sprawdzania statusów na potrzeby filtrów
    const rawStatus = (ticket.status || '').trim().toLowerCase()
    let normalizedStatusForFilter = 'W toku'

    if (rawStatus === 'nowy') {
      normalizedStatusForFilter = 'Nowy'
    } else if (rawStatus === 'rozwiązane' || rawStatus === 'rozwiazane') {
      normalizedStatusForFilter = 'Rozwiązane'
    }

    const userRoles = userStore.user?.roles || []
    const isResolver = userRoles.includes('Admin') || userRoles.includes('Helpdesk')

    // Jeśli to resolver, to "Nowe" powinny zniknąć z jego tablicy "Moje zgłoszenia", bo nowe lądują w Oczekujących
    if (isResolver && normalizedStatusForFilter === 'Nowy') return false

    // Filtry zakładek
    if (activeStatusFilter.value === 'W toku' && normalizedStatusForFilter !== 'W toku') return false
    if (activeStatusFilter.value === 'Zakończone' && normalizedStatusForFilter !== 'Rozwiązane') return false

    // Inne filtry
    if (activePriorityFilter.value !== 'Wszystkie' && ticket.priority !== activePriorityFilter.value) return false

    if (searchQuery.value) {
      const query = searchQuery.value.toLowerCase()
      const title = (ticket.title || '').toLowerCase()
      const publicId = (ticket.publicId || '').toLowerCase()
      return title.includes(query) || publicId.includes(query)
    }

    return true
  })
})

const getPriorityClass = (priority) => {
  const classes = {
    'Niski': 'bg-blue-100 text-blue-700',
    'Średni': 'bg-yellow-100 text-yellow-700',
    'Wysoki': 'bg-orange-100 text-orange-700',
    'Krytyczny': 'bg-red-100 text-red-700'
  }
  return classes[priority] || 'bg-gray-100 text-gray-700'
}

const parseUtcDate = (value) => {
  if (!value) return null
  if (value instanceof Date) return value
  if (typeof value !== 'string') return new Date(value)

  const hasTimezone = /[zZ]$|[+-]\d{2}:\d{2}$/.test(value)
  const normalized = hasTimezone ? value : `${value}Z`
  return new Date(normalized)
}

const formatDate = (date) => {
  const parsedDate = parseUtcDate(date)
  if (!parsedDate || Number.isNaN(parsedDate.getTime())) return '—'

  return parsedDate.toLocaleDateString('pl-PL', {
    timeZone: 'Europe/Warsaw',
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })
}

const getSlaLabel = (slaState) => {
  if (slaState === 'breached') return 'SLA przekroczone'
  if (slaState === 'critical') return 'SLA krytyczne (<= 2h)'
  if (slaState === 'warning') return 'SLA < 8h'
  if (slaState === 'paused') return 'SLA wstrzymane'
  return 'SLA OK'
}

const getSlaBadgeClass = (slaState) => {
  if (slaState === 'breached') return 'bg-red-100 text-red-700'
  if (slaState === 'critical') return 'bg-rose-100 text-rose-700'
  if (slaState === 'warning') return 'bg-amber-100 text-amber-700'
  if (slaState === 'paused') return 'bg-slate-100 text-slate-700'
  return 'bg-emerald-100 text-emerald-700'
}

const getSlaTextClass = (slaState) => {
  if (slaState === 'breached') return 'text-red-700'
  if (slaState === 'critical') return 'text-rose-700'
  if (slaState === 'warning') return 'text-amber-700'
  if (slaState === 'paused') return 'text-slate-700'
  return 'text-emerald-700'
}

const goToCreateTicket = () => {
  const user = userStore.user
  if (user?.roles?.includes('Helpdesk') || user?.roles?.includes('Admin') || user?.roles?.includes('Technik')) {
    router.push({ name: 'problemReportHelpdesk' })
  } else {
    router.push({ name: 'problemReportClient' })
  }
}
</script>