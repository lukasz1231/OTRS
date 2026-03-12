<template>
  <div class="min-h-screen bg-bialeTlo">
    <div class="max-w-6xl mx-auto px-6 py-12">
      <div class="bg-white p-8 rounded-2xl shadow-sm border border-gray-100">
        
        <div class="flex justify-between items-center mb-8">
          <div>
            <h1 class="text-3xl font-bold text-przyciskiNiebieski">Moje zgłoszenia</h1>
            <p class="text-gray-500 mt-1">Lista wszystkich Twoich zgłoszeń</p>
          </div>
          
          <button 
            @click="goToCreateTicket"
            class="flex items-center gap-2 bg-przyciskiNiebieski text-white px-5 py-2.5 rounded-lg hover:opacity-90 transition font-medium"
          >
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
              <path d="M5 12h14"/>
              <path d="M12 5v14"/>
            </svg>
            Nowe zgłoszenie
          </button>
        </div>

        <div class="flex flex-col md:flex-row gap-4 mb-8">
          <div class="flex bg-gray-100 p-1 rounded-xl">
            <button 
              v-for="filter in filters" 
              :key="filter.value"
              @click="activeFilter = filter.value"
              :class="[
                'px-5 py-2 text-sm font-medium rounded-lg transition-all cursor-pointer',
                activeFilter === filter.value 
                  ? 'bg-white text-przyciskiNiebieski shadow-sm' 
                  : 'text-gray-500 hover:text-gray-700'
              ]"
            >
              {{ filter.label }}
            </button>
          </div>

          <div class="relative flex-grow">
            <input 
              v-model="searchQuery"
              type="text" 
              placeholder="Szukaj po tytule..." 
              class="w-full pl-10 pr-4 py-2 rounded-xl border border-gray-200 focus:outline-none focus:ring-2 focus:ring-przyciskiNiebieski"
            />
            <span class="absolute left-3 top-2.5 text-gray-400">
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="11" cy="11" r="8"/>
                <path d="m21 21-4.3-4.3"/>
              </svg>
            </span>
          </div>
        </div>

        <div v-if="filteredTickets.length === 0" class="text-center py-20 bg-gray-50 rounded-xl">
          <p class="text-gray-500">Brak zgłoszeń</p>
        </div>

        <div v-else class="space-y-4">
          <div 
            v-for="ticket in filteredTickets" 
            :key="ticket.id"
            class="border border-gray-200 rounded-xl overflow-hidden hover:border-przyciskiNiebieski/50 transition-colors"
          >
            <div class="bg-przyciskiNiebieski text-white px-6 py-3 flex justify-between items-center">
              <span class="font-medium">{{ ticket.title }}</span>
              <span class="text-xs font-mono opacity-80">ID: {{ ticket.id }}</span>
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
                <span :class="getStatusClass(ticket.status)" class="px-3 py-1 rounded-full">
                  {{ ticket.status }}
                </span>
                <span class="text-gray-400">
                  {{ formatDate(ticket.createdAt) }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'

const router = useRouter()
const userStore = useUserStore()

const filters = [
  { label: 'Wszystkie', value: 'all' },
  { label: 'Otwarte', value: 'open' },
  { label: 'W trakcie', value: 'in_progress' },
  { label: 'Zakończone', value: 'closed' }
]

const activeFilter = ref('all')
const searchQuery = ref('')

// Przykładowe dane - do zastąpienia z backendu
const tickets = ref([
  { 
    id: 'TK-001', 
    title: 'Problem z drukarką', 
    description: 'Drukarka nie drukuje w biurze',
    type: 'Incydent',
    priority: 'Wysoki',
    status: 'Otwarte',
    createdAt: '2024-01-15'
  },
  { 
    id: 'TK-002', 
    title: 'Nowy laptop', 
    description: 'Prośba o nowy sprzęt',
    type: 'Wniosek',
    priority: 'Średni',
    status: 'W trakcie',
    createdAt: '2024-01-20'
  }
])

const filteredTickets = computed(() => {
  return tickets.value.filter(ticket => {
    if (activeFilter.value === 'open' && ticket.status !== 'Otwarte') return false
    if (activeFilter.value === 'in_progress' && ticket.status !== 'W trakcie') return false
    if (activeFilter.value === 'closed' && !['Zakończone', 'Rozwiązane'].includes(ticket.status)) return false
    
    if (searchQuery.value) {
      const query = searchQuery.value.toLowerCase()
      return ticket.title.toLowerCase().includes(query) || 
             ticket.id.toLowerCase().includes(query)
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

const getStatusClass = (status) => {
  const classes = {
    'Otwarte': 'bg-green-100 text-green-700',
    'W trakcie': 'bg-blue-100 text-blue-700',
    'Rozwiązane': 'bg-gray-100 text-gray-700',
    'Zakończone': 'bg-gray-100 text-gray-700'
  }
  return classes[status] || 'bg-gray-100 text-gray-700'
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('pl-PL')
}

const goToCreateTicket = () => {
  const user = userStore.user
  if (user?.roles?.includes('Helpdesk') || user?.roles?.includes('Admin')) {
    router.push({ name: 'problemReportHelpdesk' })
  } else {
    router.push({ name: 'problemReportClient' })
  }
}
</script>