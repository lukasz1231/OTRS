<template>
  <div class="min-h-screen bg-bialeTlo p-6">
    <div class="max-w-6xl mx-auto">
      <button
        @click="$router.push({ name: 'admin' })"
        class="mb-6 text-gray-500 hover:text-blue-600 flex items-center gap-2 cursor-pointer transition-colors"
      >
        ← Wstecz
      </button>

      <div class="flex flex-col md:flex-row md:items-center justify-between mb-8 gap-4">
        <h1 class="text-3xl font-bold text-gray-800">Panel Administratora</h1>

        <div class="relative w-full md:w-96">
          <input
            v-model="searchQuery"
            @input="fetchUsers"
            type="text"
            placeholder="Szukaj po e-mailu lub imieniu..."
            class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-przyciskiNiebieski focus:border-transparent outline-none transition-all"
          />
          <svg
            class="absolute left-3 top-2.5 text-gray-400"
            xmlns="http://www.w3.org/2000/svg"
            width="20"
            height="20"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <circle cx="11" cy="11" r="8" />
            <path d="m21 21-4.3-4.3" />
          </svg>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
        <table class="w-full text-left">
          <thead class="bg-gray-50 border-b border-gray-200">
            <tr>
              <th class="px-6 py-4 text-sm font-semibold text-gray-600">Użytkownik</th>
              <th class="px-6 py-4 text-sm font-semibold text-gray-600">Email</th>
              <th class="px-6 py-4 text-sm font-semibold text-gray-600">Role</th>
              <th class="px-6 py-4 text-sm font-semibold text-gray-600 text-right">Opcje</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-100">
            <tr v-for="user in users" :key="user.id" class="hover:bg-blue-50/30 transition-colors">
              <td class="px-6 py-4 font-medium text-gray-900">
                {{ user.name }} {{ user.surname }}
              </td>
              <td class="px-6 py-4 text-gray-600">{{ user.email }}</td>
              <td class="px-6 py-4">
                <span
                  v-for="role in user.roles"
                  :key="role"
                  class="inline-block px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-700 mr-1 uppercase"
                >
                  {{ role }}
                </span>
              </td>
              <td class="px-6 py-4 text-right flex justify-end gap-3">
                <button
                  @click="openEditModal(user)"
                  class="text-przyciskiNiebieski hover:text-blue-800 font-medium text-sm transition-colors cursor-pointer"
                >
                  Edytuj
                </button>
                <button
                  @click="confirmDelete(user)"
                  class="text-red-500 hover:text-red-700 font-medium text-sm transition-colors cursor-pointer"
                >
                  Usuń
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div
      v-if="isModalOpen"
      class="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm"
    >
      <div
        class="bg-white rounded-2xl shadow-xl w-full max-w-md p-6 animate-in fade-in zoom-in duration-200"
      >
        <h2 class="text-xl font-bold text-gray-800 mb-6">Edycja użytkownika</h2>

        <div class="space-y-4">
          <div>
            <label class="block text-xs font-bold uppercase text-gray-500 mb-1">Imię i Nazwisko</label>
            <div class="grid grid-cols-2 gap-2">
              <input
                v-model="editForm.name"
                type="text"
                class="w-full p-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
                placeholder="Imię"
              />
              <input
                v-model="editForm.surname"
                type="text"
                class="w-full p-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
                placeholder="Nazwisko"
              />
            </div>
          </div>

          <div>
            <label class="block text-xs font-bold uppercase text-gray-500 mb-1">Adres Email</label>
            <input
              v-model="editForm.email"
              type="email"
              class="w-full p-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
            />
          </div>

          <div>
            <label class="block text-xs font-bold uppercase text-gray-500 mb-1">Rola</label>
            <select
              v-model="editForm.roles[0]"
              class="w-full p-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
            >
              <option v-for="role in allRoles" :key="role" :value="role">{{ role }}</option>
            </select>
          </div>

          <div>
            <label class="block text-xs font-bold uppercase text-gray-500 mb-1">Zmień hasło (opcjonalnie)</label>
            <input
              v-model="editForm.newPassword"
              type="password"
              placeholder="Wpisz nowe hasło..."
              class="w-full p-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none"
            />
          </div>
        </div>

        <div class="flex gap-3 mt-8">
          <button
            @click="isModalOpen = false"
            class="flex-1 px-4 py-2.5 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors cursor-pointer font-medium"
          >
            Anuluj
          </button>
          <button
            @click="saveChanges"
            class="flex-1 px-4 py-2.5 bg-przyciskiNiebieski text-white rounded-lg hover:bg-blue-700 transition-colors cursor-pointer font-medium"
          >
            Zapisz
          </button>
        </div>
      </div>
    </div>

    <div
      v-if="isDeleteModalOpen"
      class="fixed inset-0 z-[110] flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm"
    >
      <div
        class="bg-white rounded-2xl shadow-xl w-full max-w-sm p-8 animate-in zoom-in duration-200 text-center"
      >
        <div class="w-16 h-16 bg-red-100 text-red-600 rounded-full flex items-center justify-center mx-auto mb-4">
          <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M3 6h18"></path>
            <path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"></path>
            <path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"></path>
          </svg>
        </div>
        <h2 class="text-xl font-bold text-gray-800 mb-2">Usuń użytkownika</h2>
        <p class="text-gray-500 text-sm mb-8">
          Czy na pewno chcesz usunąć użytkownika <strong>{{ userToDelete?.email }}</strong>? Tej operacji nie można cofnąć.
        </p>

        <div class="flex gap-3">
          <button
            @click="isDeleteModalOpen = false"
            class="flex-1 px-4 py-2.5 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 font-medium cursor-pointer"
          >
            Anuluj
          </button>
          <button
            @click="deleteUser"
            class="flex-1 px-4 py-2.5 bg-red-600 text-white rounded-lg hover:bg-red-700 font-medium cursor-pointer transition-colors"
          >
            Usuń
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, inject } from 'vue'
import axios from 'axios'

const showNotification = inject('showNotification')

const users = ref([])
const allRoles = ref([])
const searchQuery = ref('')

const isModalOpen = ref(false)
const editForm = ref({ id: null, name: '', surname: '', email: '', roles: [], newPassword: '' })

const isDeleteModalOpen = ref(false)
const userToDelete = ref(null)

const API_URL = 'https://localhost:7054/api/Admin'
const axiosConfig = { withCredentials: true }

const fetchUsers = async () => {
  try {
    const response = await axios.get(`${API_URL}/users?search=${searchQuery.value}`, axiosConfig)
    users.value = response.data
  } catch (err) {
    showNotification('Nie udało się pobrać listy użytkowników.', 'error')
  }
}

const fetchRoles = async () => {
  try {
    const response = await axios.get(`${API_URL}/roles`, axiosConfig)
    allRoles.value = response.data
  } catch {
  }
}

const openEditModal = (user) => {
  editForm.value = { ...user, newPassword: '' }
  isModalOpen.value = true
}

const saveChanges = async () => {
  try {
    await axios.put(`${API_URL}/users/${editForm.value.id}`, editForm.value, axiosConfig)
    isModalOpen.value = false
    await fetchUsers()
    showNotification('Dane użytkownika zostały zaktualizowane!', 'success')
  } catch (err) {
    showNotification('Wystąpił błąd podczas zapisywania zmian.', 'error')
  }
}

const confirmDelete = (user) => {
  userToDelete.value = user
  isDeleteModalOpen.value = true
}

const deleteUser = async () => {
  if (!userToDelete.value) return
  try {
    await axios.delete(`${API_URL}/users/${userToDelete.value.id}`, axiosConfig)
    isDeleteModalOpen.value = false
    userToDelete.value = null
    await fetchUsers()
    showNotification('Użytkownik został pomyślnie usunięty.', 'success')
  } catch (err) {
    const errorMsg = err.response?.data?.message || 'Wystąpił błąd podczas usuwania użytkownika.'
    showNotification(errorMsg, 'error')
    isDeleteModalOpen.value = false
  }
}

onMounted(() => {
  fetchUsers()
  fetchRoles()
})
</script>