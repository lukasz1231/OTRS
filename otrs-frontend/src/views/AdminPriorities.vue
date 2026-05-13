<template>
  <div class="min-h-screen bg-bialeTlo p-8">
    <div class="max-w-4xl mx-auto">
      <button @click="$router.push({ name: 'admin' })" class="mb-6 text-gray-500 hover:text-blue-600 flex items-center gap-2 cursor-pointer transition-colors">
        ← Wstecz
      </button>

      <div class="flex justify-between items-center mb-8">
        <h1 class="text-3xl font-bold text-gray-800">Priorytety Zgłoszeń</h1>
        <button @click="openAddModal" class="bg-blue-600 text-white px-6 py-2 rounded-lg font-bold hover:bg-blue-700 shadow-sm transition-all active:scale-95">
          + Nowy Priorytet
        </button>
      </div>

      <div class="grid gap-4">
        <div v-for="p in priorities" :key="p.id" class="bg-white p-5 rounded-xl shadow-sm flex justify-between items-center border border-gray-100">
          <div class="flex items-center gap-6">
            <div class="w-12 h-12 rounded-full bg-blue-50 text-blue-600 flex items-center justify-center font-black text-lg border border-blue-100 shadow-inner">
              {{ p.level }}
            </div>
            <div>
              <h3 class="font-bold text-xl text-gray-800">{{ p.name }}</h3>
              <p class="text-sm text-gray-500 italic">{{ p.description || 'Brak opisu' }}</p>
              <p class="text-xs text-blue-700 font-semibold mt-1">SLA: {{ p.slaHours }}h</p>
            </div>
          </div>
          <div class="flex gap-3">
            <button @click="openEditModal(p)" class="px-4 py-2 text-blue-600 hover:bg-blue-50 rounded-lg font-medium">Edytuj</button>
            <button @click="confirmDelete(p)" class="px-4 py-2 text-red-500 hover:bg-red-50 rounded-lg font-medium">Usuń</button>
          </div>
        </div>
      </div>
    </div>

    <div v-if="showModal" class="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center p-4 z-50">
      <div class="bg-white rounded-3xl p-8 w-full max-w-sm shadow-2xl animate-in zoom-in duration-200">
        <h2 class="text-2xl font-bold mb-4 text-gray-800">{{ isEditing ? 'Edytuj Priorytet' : 'Nowy Priorytet' }}</h2>
        <div class="space-y-4">
          <div>
            <label class="text-xs font-bold text-gray-400 uppercase ml-1">Nazwa priorytetu</label>
            <input v-model="currentPriority.name" type="text" class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500" placeholder="np. Wysoki" />
          </div>
          <div>
            <label class="text-xs font-bold text-gray-400 uppercase ml-1">Waga / Poziom (np. 1-10)</label>
            <input v-model.number="currentPriority.level" type="number" class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500" />
          </div>
          <div>
            <label class="text-xs font-bold text-gray-400 uppercase ml-1">Opis</label>
            <textarea v-model="currentPriority.description" class="w-full p-3 border border-gray-200 rounded-xl h-20 resize-none outline-none focus:ring-2 focus:ring-blue-500" placeholder="Kiedy stosować ten priorytet..."></textarea>
          </div>
          <div>
            <label class="text-xs font-bold text-gray-400 uppercase ml-1">SLA (godziny)</label>
            <input v-model.number="currentPriority.slaHours" type="number" min="1" class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500" placeholder="np. 24" />
          </div>
        </div>
        <div class="flex gap-3 mt-8">
          <button @click="showModal = false" class="flex-1 py-3 text-gray-500 font-bold hover:bg-gray-50 rounded-xl">Anuluj</button>
          <button @click="savePriority" class="flex-1 py-3 bg-blue-600 text-white rounded-xl font-bold">Zapisz</button>
        </div>
      </div>
    </div>

    <div v-if="confirmModal.show" class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center p-4 z-[100]">
      <div class="bg-white rounded-3xl p-8 w-full max-w-sm shadow-2xl text-center">
        <h2 class="text-2xl font-bold mb-2 text-gray-800">Czy na pewno?</h2>
        <p class="text-gray-500 mb-8 text-sm">{{ confirmModal.message }}</p>
        <div class="flex gap-3">
          <button @click="confirmModal.show = false" class="flex-1 py-3 text-gray-500 font-bold hover:bg-gray-50 rounded-xl">Anuluj</button>
          <button @click="confirmModal.action" class="flex-1 py-3 bg-red-500 text-white rounded-xl font-bold">Usuń</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, inject, reactive } from 'vue'
import api from '@/services/api';

const showNotification = inject('showNotification')
const API_URL = '/api/Admin/priorities'
const axiosConfig = { withCredentials: true }

const priorities = ref([])
const showModal = ref(false)
const isEditing = ref(false)
const currentPriority = ref({ id: null, name: '', description: '', level: 1, slaHours: 24 })
const confirmModal = reactive({ show: false, message: '', action: null })

const fetchPriorities = async () => {
  try {
    const res = await api.get(API_URL, axiosConfig)
    priorities.value = res.data.map(p => ({
      id: p.id ?? p.Id,
      name: p.name ?? p.Name,
      description: p.description ?? p.Description,
      level: p.level ?? p.Level,
      slaHours: p.slaHours ?? p.SlaHours ?? 24,
    }))
  } catch {
  }
}

const openAddModal = () => { isEditing.value = false; currentPriority.value = { id: null, name: '', description: '', level: 1, slaHours: 24 }; showModal.value = true; }
const openEditModal = (prio) => { isEditing.value = true; currentPriority.value = { ...prio }; showModal.value = true; }

const savePriority = async () => {
  if (!currentPriority.value.name || !currentPriority.value.slaHours || currentPriority.value.slaHours < 1) return
  try {
    if (isEditing.value) {
      await api.put(`${API_URL}/${currentPriority.value.id}`, currentPriority.value, axiosConfig)
    } else {
      const { id, ...payload } = currentPriority.value
      await api.post(API_URL, payload, axiosConfig)
    }
    showModal.value = false
    await fetchPriorities()
    showNotification(isEditing.value ? 'Zaktualizowano priorytet!' : 'Dodano priorytet!', 'success')
  } catch { 
    showNotification('Błąd zapisu priorytetu.', 'error') 
  }
}

const confirmDelete = (prio) => {
  confirmModal.message = `Czy usunąć priorytet "${prio.name}"?`
  confirmModal.action = async () => {
    try {
      await api.delete(`${API_URL}/${prio.id}`, axiosConfig)
      confirmModal.show = false
      await fetchPriorities()
      showNotification('Usunięto.', 'success')
    } catch (e) { showNotification('Priorytet jest używany!', 'error'); confirmModal.show = false; }
  }
  confirmModal.show = true
}

onMounted(fetchPriorities)
</script>