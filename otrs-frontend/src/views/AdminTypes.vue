<template>
  <div class="min-h-screen bg-bialeTlo p-6">
    <div class="max-w-4xl mx-auto">
      <button 
        @click="$router.push({ name: 'admin' })" 
        class="mb-6 text-gray-500 hover:text-blue-600 flex items-center gap-2 transition-colors cursor-pointer"
      >
        ← Wstecz
      </button>

      <div class="flex justify-between items-center mb-8">
        <h1 class="text-3xl font-bold text-gray-800">Typy Zgłoszeń</h1>
        <button 
          @click="openEditModal()" 
          class="bg-blue-600 text-white px-6 py-2 rounded-lg font-bold hover:bg-blue-700 shadow-sm transition-all cursor-pointer"
        >
          + Nowy Typ
        </button>
      </div>

      <div class="grid gap-4">
        <div 
          v-for="t in types" 
          :key="t.id" 
          class="bg-white p-6 rounded-xl shadow-sm border border-gray-100 flex justify-between items-center hover:shadow-md transition-shadow"
        >
          <div>
            <h3 class="font-bold text-lg text-gray-800">{{ t.name }}</h3>
            <p class="text-gray-500 text-sm">{{ t.description || 'Brak opisu' }}</p>
          </div>
          <div class="flex gap-2">
            <button 
              @click="openEditModal(t)" 
              class="px-4 py-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors font-semibold cursor-pointer"
            >
              Edytuj
            </button>
            <button 
              @click="confirmDelete(t)" 
              class="px-4 py-2 text-red-500 hover:bg-red-50 rounded-lg transition-colors font-semibold cursor-pointer"
            >
              Usuń
            </button>
          </div>
        </div>
        
        <div v-if="types.length === 0" class="text-center py-12 text-gray-400 italic">
          Brak zdefiniowanych typów zgłoszeń.
        </div>
      </div>
    </div>

    <div 
      v-if="isEditModalOpen" 
      class="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center p-4 z-50"
    >
      <div class="bg-white rounded-2xl p-8 w-full max-w-md shadow-2xl animate-in zoom-in duration-200">
        <h2 class="text-2xl font-bold mb-6 text-gray-800">
          {{ editId ? 'Edytuj Typ' : 'Nowy Typ' }}
        </h2>
        
        <div class="space-y-4">
          <div>
            <label class="block text-xs font-bold uppercase text-gray-400 mb-1 ml-1">Nazwa typu</label>
            <input 
              v-model="form.name" 
              type="text" 
              placeholder="np. Incydent"
              class="w-full p-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none shadow-sm" 
            />
          </div>
          <div>
            <label class="block text-xs font-bold uppercase text-gray-400 mb-1 ml-1">Opis (opcjonalnie)</label>
            <textarea 
              v-model="form.description" 
              placeholder="Krótki opis przeznaczenia tego typu..."
              class="w-full p-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none h-24 resize-none shadow-sm"
            ></textarea>
          </div>
        </div>

        <div class="flex gap-3 mt-8">
          <button 
            @click="isEditModalOpen = false" 
            class="flex-1 py-3 text-gray-500 font-bold hover:bg-gray-50 rounded-xl transition-colors cursor-pointer"
          >
            Anuluj
          </button>
          <button 
            @click="saveType" 
            class="flex-1 py-3 bg-blue-600 text-white rounded-xl font-bold hover:bg-blue-700 shadow-md transition-all cursor-pointer"
          >
            Zapisz
          </button>
        </div>
      </div>
    </div>

    <div
      v-if="deleteModal.show"
      class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center p-4 z-[100]"
    >
      <div class="bg-white rounded-3xl p-8 w-full max-w-sm shadow-2xl animate-in zoom-in duration-200 text-center">
        <div class="w-16 h-16 bg-red-50 text-red-500 rounded-full flex items-center justify-center mx-auto mb-4">
          <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M3 6h18"/><path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"/><path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/><line x1="10" y1="11" x2="10" y2="17"/><line x1="14" y1="11" x2="14" y2="17"/>
          </svg>
        </div>
        <h2 class="text-2xl font-bold mb-2 text-gray-800">Usuń typ</h2>
        <p class="text-gray-500 mb-8 text-sm">
          Czy na pewno chcesz usunąć typ <strong>{{ deleteModal.typeName }}</strong>? Tej operacji nie można cofnąć.
        </p>
        <div class="flex gap-3">
          <button 
            @click="deleteModal.show = false" 
            class="flex-1 py-3 text-gray-500 font-bold hover:bg-gray-50 rounded-xl transition-colors cursor-pointer"
          >
            Anuluj
          </button>
          <button
            @click="executeDelete"
            class="flex-1 py-3 bg-red-500 text-white rounded-xl font-bold hover:bg-red-600 shadow-md transition-all active:scale-95 cursor-pointer"
          >
            Usuń
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, inject, reactive } from 'vue';
import api from '@/services/api';

const showNotification = inject('showNotification');
const API_URL = '/api/Admin/types';
const axiosConfig = { withCredentials: true };

const types = ref([]);
const isEditModalOpen = ref(false);
const editId = ref(null);
const form = ref({ name: '', description: '' });

const deleteModal = reactive({
  show: false,
  id: null,
  typeName: ''
});

const fetchTypes = async () => {
  try {
    const res = await api.get('/api/Admin/types-all', axiosConfig);
    types.value = res.data;
  } catch {
  }
};

const openEditModal = (item = null) => {
  if (item) {
    editId.value = item.id;
    form.value = { name: item.name, description: item.description };
  } else {
    editId.value = null;
    form.value = { name: '', description: '' };
  }
  isEditModalOpen.value = true;
};

const saveType = async () => {
  if (!form.value.name) {
    showNotification('Nazwa typu jest wymagana', 'error');
    return;
  }

  try {
    if (editId.value) {
      await api.put(`${API_URL}/${editId.value}`, form.value, axiosConfig);
      showNotification('Typ został zaktualizowany', 'success');
    } else {
      await api.post(API_URL, form.value, axiosConfig);
      showNotification('Nowy typ został utworzony', 'success');
    }
    isEditModalOpen.value = false;
    fetchTypes();
  } catch (e) {
    showNotification('Wystąpił błąd podczas zapisywania.', 'error');
  }
};

const confirmDelete = (type) => {
  deleteModal.id = type.id;
  deleteModal.typeName = type.name;
  deleteModal.show = true;
};

const executeDelete = async () => {
  try {
    await api.delete(`${API_URL}/${deleteModal.id}`, axiosConfig);
    showNotification('Typ został usunięty', 'success');
    deleteModal.show = false;
    fetchTypes();
  } catch (e) {
    deleteModal.show = false;
    const errorMsg = e.response?.data || 'Nie można usunąć typu, który jest w użyciu.';
    showNotification(errorMsg, 'error');
  }
};

onMounted(fetchTypes);
</script>