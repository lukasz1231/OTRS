<template>
  <div class="min-h-screen bg-gray-50 font-sans antialiased text-gray-800 pb-12">
    <main class="max-w-6xl mx-auto px-6 py-12">
      <div class="bg-white p-8 md:p-10 rounded-2xl shadow-sm border border-gray-100">
        <section class="mb-12">
          <h1 class="text-3xl font-bold text-[#3B71A3] tracking-tight mb-8">Profil</h1>

          <div
            class="flex flex-col md:flex-row items-center gap-8 bg-gray-50 p-6 rounded-xl border border-gray-100 shadow-inner"
          >
            <div
              class="flex-shrink-0 w-24 h-24 rounded-full bg-gray-200 flex items-center justify-center border-4 border-white shadow-sm"
            >
              <IconUserLarge class="text-gray-400" />
            </div>

            <div class="flex-grow text-center md:text-left">
              <h2 class="text-2xl font-bold text-gray-700">{{ user.name }}</h2>
              <p class="text-gray-500 mb-1">{{ user.email }}</p>
              <span class="text-green-600 font-bold text-sm uppercase tracking-wider">{{
                user.role
              }}</span>
            </div>

            <div class="flex-shrink-0">
              <button
                @click="router.push({ name: 'reset-password' })"
                class="px-6 py-2 bg-[#3B71A3] text-white font-semibold rounded-lg hover:bg-blue-700 transition cursor-pointer shadow-md active:scale-95"
              >
                Zmień hasło
              </button>
            </div>
          </div>
        </section>

        <section>
          <div
            class="flex flex-col lg:flex-row justify-between items-start lg:items-center mb-8 gap-6"
          >
            <div>
              <h2 class="text-2xl font-bold text-[#3B71A3] tracking-tight">Twoje tickety</h2>
              <p class="text-sm text-gray-500 mt-1">
                Zarządzaj swoimi zgłoszeniami i sprawdzaj ich status.
              </p>
            </div>

            <button
              @click="router.push({ name: 'problemReportClient' })"
              class="flex items-center gap-2 px-5 py-2.5 bg-green-600 text-white font-bold rounded-lg hover:bg-green-700 transition shadow-sm cursor-pointer active:scale-95"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="20"
                height="20"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2.5"
                stroke-linecap="round"
                stroke-linejoin="round"
              >
                <path d="M5 12h14" />
                <path d="M12 5v14" />
              </svg>
              Utwórz zgłoszenie
            </button>
          </div>

          <div class="flex flex-col md:flex-row gap-4 mb-8">
            <div class="flex bg-gray-100 p-1 rounded-xl w-fit">
              <button
                v-for="filter in ['Wszystkie', 'W toku', 'Zakończone']"
                :key="filter"
                @click="activeFilter = filter"
                :class="[
                  'px-5 py-2 text-sm font-bold rounded-lg transition-all cursor-pointer',
                  activeFilter === filter
                    ? 'bg-white text-[#3B71A3] shadow-sm'
                    : 'text-gray-500 hover:text-gray-700',
                ]"
              >
                {{ filter }}
              </button>
            </div>

            <div class="relative flex-grow">
              <input
                v-model="searchQuery"
                type="text"
                placeholder="Szukaj po ID lub tytule..."
                class="w-full pl-10 pr-4 py-2.5 rounded-xl border border-gray-200 focus:outline-none focus:ring-2 focus:ring-[#3B71A3] transition text-sm bg-white"
              />
              <span class="absolute left-3 top-3 text-gray-400">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="18"
                  height="18"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="2"
                >
                  <circle cx="11" cy="11" r="8" />
                  <path d="m21 21-4.3-4.3" />
                </svg>
              </span>
            </div>
          </div>

          <div class="space-y-4">
            <div
              v-for="ticket in finalFilteredTickets"
              :key="ticket.id"
              class="overflow-hidden rounded-xl border border-gray-200 shadow-sm hover:border-[#3B71A3]/50 transition-all hover:shadow-md bg-white"
            >
              <div
                class="bg-[#3B71A3] text-white px-6 py-3 flex justify-between items-center text-sm font-bold"
              >
                <div class="flex items-center gap-3">
                  <span
                    class="bg-white/20 px-2 py-0.5 rounded text-[10px] font-mono tracking-tighter uppercase"
                  >
                    ID: {{ ticket.publicId }}
                  </span>
                  <span class="truncate">Tytuł: {{ ticket.title }}</span>
                </div>
                <div
                  :class="[
                    getStatusColor(ticket.status),
                    'text-xs px-3 py-1 rounded-full bg-black/10 border border-white/10',
                  ]"
                >
                  {{ ticket.status }}
                </div>
              </div>

              <div class="bg-[#7895A9] p-6 text-white text-sm">
                <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
                  <div class="md:col-span-3">
                    <p class="font-bold mb-2 uppercase text-[10px] tracking-widest opacity-70">
                      Opis zgłoszenia
                    </p>
                    <p
                      class="leading-relaxed text-gray-100 bg-black/10 p-3 rounded-lg min-h-[60px]"
                    >
                      {{ ticket.description }}
                    </p>
                  </div>
                  <div
                    class="flex flex-col justify-start md:items-right space-y-4 border-l border-white/10 md:pl-6"
                  >
                    <div>
                      <p class="font-bold uppercase text-[10px] tracking-widest opacity-70 mb-1">
                        Kolejka
                      </p>
                      <p class="font-semibold">{{ ticket.queue }}</p>
                    </div>
                    <div>
                      <p class="font-bold uppercase text-[10px] tracking-widest opacity-70 mb-1">
                        Data
                      </p>
                      <p class="text-xs">{{ new Date(ticket.createdAt).toLocaleDateString() }}</p>
                    </div>
                  </div>
                </div>
              </div>

              <div class="bg-gray-50 px-6 py-3 flex justify-end border-t border-gray-100">
                <button
                  @click="router.push({ name: 'ticket-details', params: { id: ticket.id } })"
                  class="flex items-center gap-2 px-5 py-2 bg-white text-[#3B71A3] border border-gray-200 rounded-lg text-xs font-bold uppercase tracking-wider hover:bg-[#3B71A3] hover:text-white hover:border-[#3B71A3] transition-all active:scale-95 cursor-pointer shadow-sm"
                >
                  <span>Zobacz szczegóły</span>
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    width="14"
                    height="14"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    stroke-width="3"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  >
                    <path d="M5 12h14m-7-7 7 7-7 7" />
                  </svg>
                </button>
              </div>
            </div>
          </div>
        </section>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, h, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'
import axios from 'axios'

const router = useRouter()
const userStore = useUserStore()

const searchQuery = ref('')
const activeFilter = ref('Wszystkie')
const tickets = ref([])

// Uzupełnienie HTTPS portu - upewnij się, że wpiszesz dobry!
const API_URL = 'https://localhost:7054/api/ticket'
const axiosConfig = { withCredentials: true }

// Dynamiczny użytkownik ze store'a
const user = computed(() => {
  const u = userStore.user
  if (!u) return { name: '', email: '', role: '' }

  return {
    name: `${u.name} ${u.surname}`,
    email: u.email,
    role: u.roles && u.roles.length > 0 ? u.roles[0] : 'Użytkownik',
  }
})

// Pobieranie ticketów z .NET
const fetchMyTickets = async () => {
  try {
    const response = await axios.get(API_URL, axiosConfig)
    tickets.value = response.data
  } catch (error) {
    console.error('Błąd podczas pobierania ticketów:', error)
  }
}

onMounted(() => {
  fetchMyTickets()
})

// Filtrowanie z wyszukiwarką
const finalFilteredTickets = computed(() => {
  return tickets.value.filter((t) => {
    let matchesFilter = true
    if (activeFilter.value === 'W toku') {
      matchesFilter = ['Przyjęte', 'W toku', 'Nowy'].includes(t.status)
    } else if (activeFilter.value === 'Zakończone') {
      matchesFilter = ['Rozwiązane', 'Zamknięte'].includes(t.status)
    }

    const term = searchQuery.value.toLowerCase()
    const titleMatch = t.title ? t.title.toLowerCase().includes(term) : false
    const publicIdMatch = t.publicId ? t.publicId.toLowerCase().includes(term) : false

    const matchesSearch = titleMatch || publicIdMatch

    return matchesFilter && matchesSearch
  })
})

const getStatusColor = (status) => {
  if (['Rozwiązane', 'Zamknięte'].includes(status)) return 'text-gray-300 underline'
  if (['W toku'].includes(status)) return 'text-yellow-300 font-bold'
  if (['Przyjęte', 'Nowy'].includes(status)) return 'text-green-300 font-bold'
  return 'text-blue-200'
}

const IconUserLarge = () =>
  h(
    'svg',
    {
      xmlns: 'http://www.w3.org/2000/svg',
      width: '48',
      height: '48',
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
</script>
