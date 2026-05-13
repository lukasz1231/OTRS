<template>
  <div class="min-h-screen bg-bialeTlo p-8">
    <div class="max-w-4xl mx-auto">
      <button
        @click="$router.push({ name: 'admin' })"
        class="mb-6 text-gray-500 hover:text-blue-600 flex items-center gap-2 cursor-pointer transition-colors"
      >
        ← Wstecz
      </button>
      <div class="flex justify-between items-center mb-8">
        <h1 class="text-3xl font-bold text-gray-800">Kolejki Zgłoszeń</h1>
        <button
          @click="showAddModal = true"
          class="bg-blue-600 text-white px-6 py-2 rounded-lg font-bold hover:bg-blue-700 cursor-pointer shadow-sm"
        >
          + Nowa Kolejka
        </button>
      </div>

      <div class="grid gap-4">
        <div
          v-for="q in queues"
          :key="q.id"
          class="bg-white p-5 rounded-xl shadow-sm flex justify-between items-center border border-gray-100 hover:shadow-md transition-shadow"
        >
          <div>
            <h3 class="font-bold text-xl text-gray-800">{{ q.name }}</h3>
            <p class="text-sm text-gray-500 italic">Przypisanych agentów: {{ q.userCount }}</p>
          </div>
          <div class="flex gap-3">
            <button
              @click="openManageUsers(q)"
              class="px-4 py-2 bg-blue-50 text-blue-600 rounded-lg hover:bg-blue-100 font-semibold cursor-pointer transition-colors"
            >
              Agenci
            </button>
            <button
              @click="confirmDeleteQueue(q)"
              class="px-4 py-2 text-red-500 hover:bg-red-50 rounded-lg cursor-pointer transition-colors"
            >
              Usuń
            </button>
          </div>
        </div>
      </div>
    </div>

    <div
      v-if="selectedQueue"
      class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center p-4 z-50"
    >
      <div class="bg-white rounded-2xl p-8 w-full max-w-lg shadow-2xl animate-in zoom-in duration-200">
        <h2 class="text-2xl font-bold mb-2 text-gray-800">{{ selectedQueue.name }}</h2>
        <p class="text-gray-500 text-sm mb-6">Lista osób widzących tę kolejkę:</p>
        
        <div class="mb-6 max-h-56 overflow-y-auto space-y-2 border-b pb-6 pr-2">
          <div
            v-for="u in queueUsers"
            :key="u.id"
            class="flex justify-between items-center bg-gray-50 p-3 rounded-xl border border-gray-100"
          >
            <div class="flex flex-col">
              <span class="font-bold text-gray-700">{{ u.name }} {{ u.surname }}</span>
              <div class="flex flex-wrap gap-1 mt-1">
                <span 
                  v-for="role in u.roles" 
                  :key="role" 
                  class="text-[10px] bg-blue-100 text-blue-700 px-1.5 py-0.5 rounded-md uppercase font-black tracking-wider"
                >
                  {{ role }}
                </span>
              </div>
            </div>
            <button
              @click="confirmRemoveUser(u)"
              class="text-red-500 text-xs font-black uppercase tracking-widest hover:bg-red-100 p-2 rounded-lg transition-colors"
            >
              Usuń
            </button>
          </div>
          <p v-if="queueUsers.length === 0" class="text-center text-gray-400 py-4 text-sm italic">
            Brak przypisanych agentów do tej kolejki.
          </p>
        </div>

        <div class="space-y-3 bg-gray-50 p-4 rounded-2xl border border-gray-100 relative">
          <label class="text-xs font-bold text-gray-400 uppercase ml-1">Wyszukaj i dodaj agenta</label>
          <div class="flex gap-2 relative">
            <div class="relative flex-grow">
              <input 
                v-model="userSearchQuery" 
                type="text"
                placeholder="Imię, nazwisko lub email..."
                class="w-full p-2.5 border border-gray-200 rounded-xl bg-white outline-none focus:ring-2 focus:ring-blue-500 shadow-sm text-sm"
                @focus="isUserListOpen = true"
              />
              
              <div v-if="isUserListOpen && filteredUsers.length > 0" 
                  class="absolute z-[60] w-full mt-1 bg-white border border-gray-200 rounded-xl shadow-xl max-h-48 overflow-y-auto overflow-x-hidden">
                <div 
                  v-for="user in filteredUsers" 
                  :key="user.id"
                  @click="selectUser(user)"
                  class="p-3 hover:bg-blue-50 cursor-pointer border-b last:border-0 border-gray-50 transition-colors text-left flex justify-between items-center gap-2"
                >
                  <div class="min-w-0">
                    <div class="font-bold text-gray-800 text-sm truncate">
                      {{ user.name }} {{ user.surname }}
                    </div>
                    <div class="text-[10px] text-gray-500 truncate">{{ user.email }}</div>
                  </div>
                  
                  <div class="flex flex-wrap gap-1 justify-end shrink-0">
                    <span 
                      v-for="role in user.roles" 
                      :key="role"
                      class="text-[9px] bg-gray-100 text-gray-600 px-1.5 py-0.5 rounded border border-gray-200 uppercase font-medium"
                    >
                      {{ role }}
                    </span>
                  </div>
                </div>
              </div>

              <div v-if="isUserListOpen && userSearchQuery && filteredUsers.length === 0" 
                   class="absolute z-[60] w-full mt-1 bg-white p-4 border border-gray-200 rounded-xl shadow-xl text-center text-sm text-gray-400 italic">
                Nie znaleziono pracownika...
              </div>
            </div>

            <button
              @click="addUserToQueue"
              :disabled="!userIdToAdd"
              class="bg-blue-600 text-white px-6 py-2 rounded-xl font-bold hover:bg-blue-700 transition-all shadow-md active:scale-95 disabled:opacity-50 disabled:scale-100"
            >
              DODAJ
            </button>
          </div>
          
          <div v-if="selectedUserLabel" class="mt-2 flex items-center gap-2">
            <span class="text-[10px] font-bold text-blue-600 bg-blue-50 px-2 py-1 rounded-md">Wybrano: {{ selectedUserLabel }}</span>
            <button @click="resetSelection" class="text-gray-400 hover:text-red-500 text-xs">✕</button>
          </div>
        </div>

        <button
          @click="closeManageUsers"
          class="w-full mt-8 py-3 bg-gray-100 rounded-xl font-bold text-gray-600 hover:bg-gray-200 transition-colors cursor-pointer"
        >
          Zamknij okno
        </button>
      </div>
    </div>

    <div
      v-if="showAddModal"
      class="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center p-4 z-50"
    >
      <div class="bg-white rounded-3xl p-8 w-full max-w-sm shadow-2xl">
        <h2 class="text-2xl font-bold mb-4 text-gray-800">Nowa Kolejka</h2>
        <input
          v-model="newQueueName"
          type="text"
          class="w-full p-3 border border-gray-200 rounded-xl mb-6 outline-none focus:ring-2 focus:ring-blue-500 shadow-sm"
          placeholder="Nazwa (np. Serwis IT)"
        />
        <div class="flex gap-3">
          <button @click="showAddModal = false" class="flex-1 py-3 text-gray-500 font-bold hover:bg-gray-50 rounded-xl transition-colors">
            Anuluj
          </button>
          <button
            @click="createQueue"
            class="flex-1 py-3 bg-blue-600 text-white rounded-xl font-bold hover:bg-blue-700 shadow-md transition-all"
          >
            Stwórz
          </button>
        </div>
      </div>
    </div>

    <div
      v-if="confirmModal.show"
      class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center p-4 z-[100]"
    >
      <div class="bg-white rounded-3xl p-8 w-full max-w-sm shadow-2xl text-center">
        <div class="w-16 h-16 bg-red-50 text-red-500 rounded-full flex items-center justify-center mx-auto mb-4">
          <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M3 6h18"/><path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"/><path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/><line x1="10" y1="11" x2="10" y2="17"/><line x1="14" y1="11" x2="14" y2="17"/></svg>
        </div>
        <h2 class="text-2xl font-bold mb-2 text-gray-800">Czy na pewno?</h2>
        <p class="text-gray-500 mb-8 text-sm">{{ confirmModal.message }}</p>
        <div class="flex gap-3">
          <button @click="confirmModal.show = false" class="flex-1 py-3 text-gray-500 font-bold hover:bg-gray-50 rounded-xl">Anuluj</button>
          <button @click="confirmModal.action" class="flex-1 py-3 bg-red-500 text-white rounded-xl font-bold hover:bg-red-600 shadow-md active:scale-95">Usuń</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, inject, reactive, computed } from 'vue'
import api from '@/services/api';

const showNotification = inject('showNotification')
const API_URL = '/api/Admin'
const axiosConfig = { withCredentials: true }

// Podstawowe dane
const queues = ref([])
const showAddModal = ref(false)
const newQueueName = ref('')
const selectedQueue = ref(null)
const queueUsers = ref([])
const allUsers = ref([])

// Logika wyszukiwania agentów
const userSearchQuery = ref('')
const isUserListOpen = ref(false)
const userIdToAdd = ref(null)
const selectedUserLabel = ref('')

const confirmModal = reactive({
  show: false,
  message: '',
  action: null
})

// Filtrowanie użytkowników w locie
const filteredUsers = computed(() => {
  if (!userSearchQuery.value) return allUsers.value
  const query = userSearchQuery.value.toLowerCase()
  return allUsers.value.filter(u => 
    u.name.toLowerCase().includes(query) || 
    u.surname.toLowerCase().includes(query) || 
    u.email.toLowerCase().includes(query)
  )
})

const selectUser = (user) => {
  userIdToAdd.value = user.id
  const rolesBrackets = user.roles?.length ? ` [${user.roles.join(', ')}]` : ''
  selectedUserLabel.value = `${user.name} ${user.surname}${rolesBrackets}`
  userSearchQuery.value = `${user.name} ${user.surname}`
  isUserListOpen.value = false
}

const resetSelection = () => {
  userIdToAdd.value = null
  selectedUserLabel.value = ''
  userSearchQuery.value = ''
  isUserListOpen.value = false
}

const fetchQueues = async () => {
  try {
    const res = await api.get(`${API_URL}/queues`, axiosConfig)
    queues.value = res.data
  } catch (e) { }
}

const createQueue = async () => {
  if (!newQueueName.value) return
  try {
    await api.post(`${API_URL}/queues`, { name: newQueueName.value }, axiosConfig)
    newQueueName.value = ''
    showAddModal.value = false
    await fetchQueues()
    showNotification('Kolejka została utworzona!', 'success')
  } catch (e) { showNotification('Błąd tworzenia kolejki.', 'error') }
}

const executeDeleteQueue = async (id) => {
  try {
    await api.delete(`${API_URL}/queues/${id}`, axiosConfig)
    confirmModal.show = false
    await fetchQueues()
    showNotification('Kolejka usunięta.', 'success')
  } catch (e) {
    confirmModal.show = false
    showNotification('Nie można usunąć kolejki ze zgłoszeniami!', 'error')
  }
}

const confirmDeleteQueue = (queue) => {
  confirmModal.message = `Usunąć kolejkę "${queue.name}"?`
  confirmModal.action = () => executeDeleteQueue(queue.id)
  confirmModal.show = true
}

const openManageUsers = async (queue) => {
  selectedQueue.value = queue
  resetSelection()
  try {
    const res = await api.get(`${API_URL}/queues/${queue.id}/users`, axiosConfig)
    queueUsers.value = res.data
    const allU = await api.get(`${API_URL}/users`, axiosConfig)
    allUsers.value = allU.data
  } catch (e) { showNotification('Błąd ładowania danych.', 'error') }
}

const closeManageUsers = () => {
  selectedQueue.value = null
  resetSelection()
}

const addUserToQueue = async () => {
  if (!userIdToAdd.value) return
  try {
    await api.post(`${API_URL}/queues/${selectedQueue.value.id}/users/${userIdToAdd.value}`, {}, axiosConfig)
    await openManageUsers(selectedQueue.value)
    await fetchQueues()
    resetSelection()
    showNotification('Agent dodany!', 'success')
  } catch (e) { showNotification('Błąd dodawania agenta.', 'error') }
}

const executeRemoveUser = async (uId) => {
  try {
    await api.delete(`${API_URL}/queues/${selectedQueue.value.id}/users/${uId}`, axiosConfig)
    confirmModal.show = false
    await openManageUsers(selectedQueue.value)
    await fetchQueues()
    showNotification('Agent usunięty z kolejki.', 'success')
  } catch (e) { showNotification('Błąd usuwania.', 'error') }
}

const confirmRemoveUser = (user) => {
  confirmModal.message = `Usunąć ${user.name} z tej kolejki?`
  confirmModal.action = () => executeRemoveUser(user.id)
  confirmModal.show = true
}

onMounted(fetchQueues)
</script>