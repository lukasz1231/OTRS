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
        <h1 class="text-3xl font-bold text-gray-800">Kategorie Zgłoszeń</h1>
        <button
          @click="openAddModal"
          class="bg-blue-600 text-white px-6 py-2 rounded-lg font-bold hover:bg-blue-700 shadow-sm transition-all active:scale-95"
        >
          + Nowa Kategoria
        </button>
      </div>

      <div class="grid gap-4">
        <div
          v-for="c in categories"
          :key="c.id"
          class="bg-white p-5 rounded-xl shadow-sm flex justify-between items-center border border-gray-100 hover:shadow-md transition-shadow"
        >
          <div class="flex flex-col">
            <h3 class="font-bold text-xl text-gray-800">{{ c.name }}</h3>
            <p class="text-sm text-gray-500 italic">{{ c.description || 'Brak opisu' }}</p>
          </div>
          <div class="flex gap-3">
            <button @click="openEditModal(c)" class="px-4 py-2 text-blue-600 hover:bg-blue-50 rounded-lg font-medium">Edytuj</button>
            <button @click="confirmDelete(c)" class="px-4 py-2 text-red-500 hover:bg-red-50 rounded-lg font-medium">Usuń</button>
          </div>
        </div>
      </div>
    </div>

    <div v-if="showModal" class="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center p-4 z-50">
      <div class="bg-white rounded-3xl p-8 w-full max-w-sm shadow-2xl animate-in zoom-in duration-200">
        <h2 class="text-2xl font-bold mb-4 text-gray-800">{{ isEditing ? 'Edytuj Kategorię' : 'Nowa Kategoria' }}</h2>
        <div class="space-y-4">
          <div>
            <label class="text-xs font-bold text-gray-400 uppercase ml-1">Nazwa kategorii</label>
            <input v-model="currentCategory.name" type="text" class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500 shadow-sm" placeholder="np. Sprzęt IT" />
          </div>
          <div>
            <label class="text-xs font-bold text-gray-400 uppercase ml-1">Opis</label>
            <textarea v-model="currentCategory.description" class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500 shadow-sm h-24 resize-none" placeholder="Czego dotyczy ta usługa..."></textarea>
          </div>
        </div>
        <div class="flex gap-3 mt-8">
          <button @click="showModal = false" class="flex-1 py-3 text-gray-500 font-bold hover:bg-gray-50 rounded-xl">Anuluj</button>
          <button @click="saveCategory" class="flex-1 py-3 bg-blue-600 text-white rounded-xl font-bold hover:bg-blue-700 shadow-md">Zapisz</button>
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
import axios from 'axios'

const showNotification = inject('showNotification')
const API_URL = 'https://localhost:7054/api/Admin/categories'
const axiosConfig = { withCredentials: true }

const categories = ref([])
const showModal = ref(false)
const isEditing = ref(false)
const currentCategory = ref({ id: null, name: '', description: '' })
const confirmModal = reactive({ show: false, message: '', action: null })

const fetchCategories = async () => {
  try {
    const res = await axios.get(API_URL, axiosConfig)
    categories.value = res.data.map(c => ({ id: c.id || c.Id, name: c.name || c.Name, description: c.description || c.Description }))
  } catch (e) { console.error(e) }
}

const openAddModal = () => { isEditing.value = false; currentCategory.value = { id: null, name: '', description: '' }; showModal.value = true; }
const openEditModal = (cat) => { isEditing.value = true; currentCategory.value = { ...cat }; showModal.value = true; }

const saveCategory = async () => {
  if (!currentCategory.value.name) return
  try {
    if (isEditing.value) {
      // Przy edycji (PUT) ID jest wymagane w URL i w body
      await axios.put(`${API_URL}/${currentCategory.value.id}`, currentCategory.value, axiosConfig)
    } else {
      // Przy dodawaniu (POST) usuwamy ID, żeby nie wysyłać null
      const { id, ...payload } = currentCategory.value 
      await axios.post(API_URL, payload, axiosConfig)
    }
    showModal.value = false
    await fetchCategories()
    showNotification(isEditing.value ? 'Zaktualizowano kategorię!' : 'Dodano kategorię!', 'success')
  } catch (e) { 
    console.error(e.response?.data) // Sprawdź w konsoli co dokładnie boli serwer
    showNotification('Błąd zapisu kategorii.', 'error') 
  }
}

const confirmDelete = (cat) => {
  confirmModal.message = `Czy usunąć kategorię "${cat.name}"?`
  confirmModal.action = async () => {
    try {
      await axios.delete(`${API_URL}/${cat.id}`, axiosConfig)
      confirmModal.show = false
      await fetchCategories()
      showNotification('Usunięto.', 'success')
    } catch (e) { showNotification('Kategoria jest używana!', 'error'); confirmModal.show = false; }
  }
  confirmModal.show = true
}

onMounted(fetchCategories)
</script>