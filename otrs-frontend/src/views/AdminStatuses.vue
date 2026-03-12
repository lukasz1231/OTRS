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
        <h1 class="text-3xl font-bold text-gray-800">Konfiguracja Statusów</h1>
        <button
          @click="openAddModal"
          class="bg-blue-600 text-white px-6 py-2 rounded-lg font-bold hover:bg-blue-700 shadow-sm transition-all active:scale-95"
        >
          + Nowy Status
        </button>
      </div>

      <div class="grid gap-4">
        <div
          v-for="s in statuses"
          :key="s.id"
          class="bg-white p-5 rounded-xl shadow-sm flex justify-between items-center border border-gray-100 hover:shadow-md transition-shadow"
        >
          <div class="flex flex-col">
            <h3 class="font-bold text-xl text-gray-800">{{ s.name }}</h3>
            <p class="text-sm text-gray-500 italic">{{ s.description || 'Brak opisu' }}</p>
          </div>
          <div class="flex gap-3">
            <button
              @click="openEditModal(s)"
              class="px-4 py-2 text-blue-600 hover:bg-blue-50 rounded-lg cursor-pointer transition-colors font-medium"
            >
              Edytuj
            </button>
            <button
              @click="confirmDeleteStatus(s)"
              class="px-4 py-2 text-red-500 hover:bg-red-50 rounded-lg cursor-pointer transition-colors font-medium"
            >
              Usuń
            </button>
          </div>
        </div>
        
        <div v-if="statuses.length === 0" class="text-center py-12 bg-white rounded-2xl border-2 border-dashed border-gray-100">
           <p class="text-gray-400 italic">Ładowanie statusów lub brak danych w bazie...</p>
        </div>
      </div>
    </div>

    <div v-if="showModal" class="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center p-4 z-50">
      <div class="bg-white rounded-3xl p-8 w-full max-w-sm shadow-2xl animate-in zoom-in duration-200">
        <h2 class="text-2xl font-bold mb-4 text-gray-800">
          {{ isEditing ? 'Edytuj Status' : 'Nowy Status' }}
        </h2>
        <div class="space-y-4">
          <div>
            <label class="text-xs font-bold text-gray-400 uppercase ml-1">Nazwa</label>
            <input
              v-model="currentStatus.name"
              type="text"
              class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500 shadow-sm"
              placeholder="np. W realizacji"
            />
          </div>
          <div>
            <label class="text-xs font-bold text-gray-400 uppercase ml-1">Opis</label>
            <textarea
              v-model="currentStatus.description"
              class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500 shadow-sm h-24 resize-none"
              placeholder="Opis..."
            ></textarea>
          </div>
        </div>
        <div class="flex gap-3 mt-8">
          <button @click="showModal = false" class="flex-1 py-3 text-gray-500 font-bold hover:bg-gray-50 rounded-xl">
            Anuluj
          </button>
          <button
            @click="saveStatus"
            class="flex-1 py-3 bg-blue-600 text-white rounded-xl font-bold hover:bg-blue-700 shadow-md transition-all active:scale-95"
          >
            {{ isEditing ? 'Zapisz' : 'Stwórz' }}
          </button>
        </div>
      </div>
    </div>

    <div v-if="confirmModal.show" class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center p-4 z-[100]">
      <div class="bg-white rounded-3xl p-8 w-full max-w-sm shadow-2xl animate-in zoom-in duration-200 text-center">
        <h2 class="text-2xl font-bold mb-2 text-gray-800">Czy na pewno?</h2>
        <p class="text-gray-500 mb-8 text-sm">{{ confirmModal.message }}</p>
        <div class="flex gap-3">
          <button @click="confirmModal.show = false" class="flex-1 py-3 text-gray-500 font-bold hover:bg-gray-50 rounded-xl">
            Anuluj
          </button>
          <button @click="confirmModal.action" class="flex-1 py-3 bg-red-500 text-white rounded-xl font-bold hover:bg-red-600 shadow-md">
            Usuń
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, inject, reactive } from 'vue'
import axios from 'axios'

const showNotification = inject('showNotification')
const API_URL = 'https://localhost:7054/api/Admin/statuses'
const axiosConfig = { withCredentials: true }

const statuses = ref([])
const showModal = ref(false)
const isEditing = ref(false)
const currentStatus = ref({ id: null, name: '', description: '' })
const confirmModal = reactive({ show: false, message: '', action: null })

const fetchStatuses = async () => {
  try {
    const res = await axios.get(API_URL, axiosConfig)
    // MAPOWANIE: Gwarantujemy, że pola będą miały małe litery niezależnie od ustawień JSON na serwerze
    statuses.value = res.data.map(s => ({
      id: s.id || s.Id,
      name: s.name || s.Name,
      description: s.description || s.Description
    }))
  } catch (e) { 
    console.error('Błąd fetch:', e)
    showNotification('Błąd połączenia z serwerem.', 'error')
  }
}

const openAddModal = () => {
  isEditing.value = false
  currentStatus.value = { id: null, name: '', description: '' }
  showModal.value = true
}

const openEditModal = (status) => {
  isEditing.value = true
  currentStatus.value = { ...status }
  showModal.value = true
}

const saveStatus = async () => {
  if (!currentStatus.value.name) return
  try {
    if (isEditing.value) {
      await axios.put(`${API_URL}/${currentStatus.value.id}`, currentStatus.value, axiosConfig)
      showNotification('Zaktualizowano status.', 'success')
    } else {
      // TWORZYMY KOPIĘ BEZ POLA ID
      const payload = {
        name: currentStatus.value.name,
        description: currentStatus.value.description
      }
      await axios.post(API_URL, payload, axiosConfig)
      showNotification('Dodano nowy status.', 'success')
    }
    showModal.value = false
    await fetchStatuses()
  } catch (e) {
    // PODGLĄD BŁĘDU W KONSOLI - pomoże nam jeśli to nie ID był problemem
    console.error('Szczegóły błędu 400:', e.response?.data)
    showNotification('Błąd zapisu. Sprawdź dane.', 'error')
  }
}

const confirmDeleteStatus = (status) => {
  confirmModal.message = `Czy na pewno chcesz usunąć "${status.name}"?`
  confirmModal.action = () => executeDeleteStatus(status.id)
  confirmModal.show = true
}

const executeDeleteStatus = async (id) => {
  try {
    await axios.delete(`${API_URL}/${id}`, axiosConfig)
    confirmModal.show = false
    await fetchStatuses()
    showNotification('Status usunięty.', 'success')
  } catch (e) {
    confirmModal.show = false
    showNotification('Status jest używany!', 'error')
  }
}

onMounted(fetchStatuses)
</script>