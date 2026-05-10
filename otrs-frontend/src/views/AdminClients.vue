<template>
  <div class="min-h-screen bg-bialeTlo p-4 md:p-8">
    <div class="max-w-4xl mx-auto">
      <button
        @click="$router.push({ name: 'admin' })"
        class="mb-6 text-gray-500 hover:text-blue-600 flex items-center gap-2 cursor-pointer transition-colors"
      >
        ← Wstecz
      </button>

      <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-8">
        <h1 class="text-3xl font-bold text-gray-800">Zarządzanie Klientami</h1>
        <button
          @click="openAddModal"
          class="bg-blue-600 text-white w-full sm:w-auto px-6 py-2.5 rounded-xl font-bold hover:bg-blue-700 shadow-sm transition-all active:scale-95"
        >
          + Nowy Klient
        </button>
      </div>

      <div class="grid gap-4">
        <div
          v-for="c in clients"
          :key="c.id"
          class="bg-white p-5 rounded-2xl shadow-sm flex flex-col sm:flex-row sm:justify-between sm:items-center gap-4 border border-gray-100 hover:shadow-md transition-shadow"
        >
          <div class="flex flex-col space-y-1">
            <h3 class="font-bold text-xl text-gray-800">{{ c.name }}</h3>
            <p class="text-sm text-gray-500 italic pb-2">{{ c.description || 'Brak opisu firmy' }}</p>
            
            <div class="flex flex-col text-sm text-gray-600 space-y-0.5">
              <span v-if="c.city || c.postalCode" class="flex items-center gap-2">
                📍 {{ c.postalCode }} {{ c.city }}
              </span>
              <span v-if="c.street" class="flex items-center gap-2 text-gray-500">
                🏠 ul. {{ c.street }} {{ c.streetNumber }}<template v-if="c.apartmentNumber">/{{ c.apartmentNumber }}</template>
              </span>
              <span v-if="c.phone" class="flex items-center gap-2 text-gray-500 mt-1 font-medium">
                📞 {{ c.phone }}
              </span>
              <span class="flex items-start gap-2 text-gray-500 mt-1">
                👤
                <span>
                  <span class="font-semibold">Konta: </span>
                  <span v-if="(c.users || []).length > 0">
                    {{ c.users.map(u => `${u.name} ${u.surname}`).join(', ') }}
                  </span>
                  <span v-else>brak przypisanych kont</span>
                </span>
              </span>
            </div>
          </div>

          <div class="flex gap-2 sm:flex-col md:flex-row w-full sm:w-auto shrink-0">
            <button @click="openEditModal(c)" class="flex-1 sm:flex-none px-4 py-2 text-blue-600 bg-blue-50 hover:bg-blue-100 rounded-lg font-medium transition-colors text-center">
              Edytuj
            </button>
            <button @click="confirmDelete(c)" class="flex-1 sm:flex-none px-4 py-2 text-red-600 bg-red-50 hover:bg-red-100 rounded-lg font-medium transition-colors text-center">
              Usuń
            </button>
          </div>
        </div>

        <div v-if="clients.length === 0" class="text-center py-12 bg-white rounded-2xl border-2 border-dashed border-gray-200">
           <p class="text-gray-400 font-medium">Brak zdefiniowanych klientów w bazie.</p>
        </div>
      </div>
    </div>

    <div v-if="showModal" class="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center p-4 z-50">
      <div class="bg-white rounded-3xl w-full max-w-lg shadow-2xl animate-in zoom-in duration-200 flex flex-col max-h-[90vh]">
        
        <div class="p-6 md:p-8 pb-4 border-b border-gray-100 shrink-0">
          <h2 class="text-2xl font-bold text-gray-800">{{ isEditing ? 'Edytuj Klienta' : 'Nowy Klient' }}</h2>
        </div>

        <div class="p-6 md:px-8 md:py-6 overflow-y-auto custom-scrollbar">
          <div class="space-y-4">
            
            <div>
              <label class="text-xs font-bold text-gray-500 uppercase ml-1 block mb-1">Nazwa Firmy / Klienta *</label>
              <input 
                v-model="currentClient.name" 
                type="text" 
                :class="{'border-red-500 focus:ring-red-500': getError('Name')}"
                class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500 transition-all bg-gray-50 focus:bg-white" 
                placeholder="np. Acme Corp" 
              />
              <span v-if="getError('Name')" class="text-red-500 text-xs mt-1 ml-1 block">{{ getError('Name')[0] }}</span>
            </div>

            <div>
              <label class="text-xs font-bold text-gray-500 uppercase ml-1 block mb-1">Opis (opcjonalnie)</label>
              <textarea 
                v-model="currentClient.description" 
                :class="{'border-red-500 focus:ring-red-500': getError('Description')}"
                class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500 transition-all bg-gray-50 focus:bg-white h-20 resize-none" 
                placeholder="Krótki opis..."
              ></textarea>
              <span v-if="getError('Description')" class="text-red-500 text-xs mt-1 ml-1 block">{{ getError('Description')[0] }}</span>
            </div>

            <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
              <div class="sm:col-span-1">
                <label class="text-xs font-bold text-gray-500 uppercase ml-1 block mb-1">Kod pocztowy</label>
                <input 
                  v-model="currentClient.postalCode" 
                  :class="{'border-red-500 focus:ring-red-500': getError('PostalCode')}"
                  class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500 transition-all bg-gray-50 focus:bg-white" 
                  placeholder="00-000" 
                />
                <span v-if="getError('PostalCode')" class="text-red-500 text-xs mt-1 ml-1 block">{{ getError('PostalCode')[0] }}</span>
              </div>
              <div class="sm:col-span-2">
                <label class="text-xs font-bold text-gray-500 uppercase ml-1 block mb-1">Miasto</label>
                <input 
                  v-model="currentClient.city" 
                  :class="{'border-red-500 focus:ring-red-500': getError('City')}"
                  class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500 transition-all bg-gray-50 focus:bg-white" 
                  placeholder="np. Warszawa" 
                />
                <span v-if="getError('City')" class="text-red-500 text-xs mt-1 ml-1 block">{{ getError('City')[0] }}</span>
              </div>
            </div>

            <div class="grid grid-cols-2 sm:grid-cols-4 gap-4">
              <div class="col-span-2">
                <label class="text-xs font-bold text-gray-500 uppercase ml-1 block mb-1">Ulica</label>
                <input 
                  v-model="currentClient.street" 
                  :class="{'border-red-500 focus:ring-red-500': getError('Street')}"
                  class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500 transition-all bg-gray-50 focus:bg-white" 
                  placeholder="np. Polna" 
                />
                <span v-if="getError('Street')" class="text-red-500 text-xs mt-1 ml-1 block">{{ getError('Street')[0] }}</span>
              </div>
              <div class="col-span-1">
                <label class="text-xs font-bold text-gray-500 uppercase ml-1 block mb-1">Nr bud.</label>
                <input 
                  v-model="currentClient.streetNumber" 
                  :class="{'border-red-500 focus:ring-red-500': getError('StreetNumber')}"
                  class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500 transition-all bg-gray-50 focus:bg-white" 
                  placeholder="10A" 
                />
                <span v-if="getError('StreetNumber')" class="text-red-500 text-xs mt-1 ml-1 block">{{ getError('StreetNumber')[0] }}</span>
              </div>
              <div class="col-span-1">
                <label class="text-xs font-bold text-gray-500 uppercase ml-1 block mb-1">Nr lok.</label>
                <input 
                  v-model="currentClient.apartmentNumber" 
                  :class="{'border-red-500 focus:ring-red-500': getError('ApartmentNumber')}"
                  class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500 transition-all bg-gray-50 focus:bg-white" 
                  placeholder="5" 
                />
                <span v-if="getError('ApartmentNumber')" class="text-red-500 text-xs mt-1 ml-1 block">{{ getError('ApartmentNumber')[0] }}</span>
              </div>
            </div>

            <div>
              <label class="text-xs font-bold text-gray-500 uppercase ml-1 block mb-1">Telefon kontaktowy</label>
              <input 
                v-model="currentClient.phone" 
                type="tel" 
                :class="{'border-red-500 focus:ring-red-500': getError('Phone')}"
                class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500 transition-all bg-gray-50 focus:bg-white" 
                placeholder="+48 123 456 789" 
              />
              <span v-if="getError('Phone')" class="text-red-500 text-xs mt-1 ml-1 block">{{ getError('Phone')[0] }}</span>
            </div>

            <div>
              <label class="text-xs font-bold text-gray-500 uppercase ml-1 block mb-2">Podpięte konta użytkowników</label>
              <input
                v-model="userSearch"
                type="text"
                class="w-full p-3 border border-gray-200 rounded-xl outline-none focus:ring-2 focus:ring-blue-500 transition-all bg-gray-50 focus:bg-white mb-2"
                placeholder="Szukaj po imieniu, nazwisku lub emailu"
              />

              <div class="max-h-44 overflow-y-auto border border-gray-200 rounded-xl bg-gray-50 p-2 space-y-1">
                <label
                  v-for="u in filteredUsers"
                  :key="u.id"
                  class="flex items-center gap-3 px-2 py-2 rounded-lg hover:bg-white cursor-pointer"
                >
                  <input
                    v-model="selectedUserIds"
                    :value="u.id"
                    type="checkbox"
                    class="w-4 h-4"
                  />
                  <span class="text-sm text-gray-700">
                    {{ u.name }} {{ u.surname }}
                    <span class="text-gray-500">({{ u.email }})</span>
                  </span>
                </label>

                <div v-if="filteredUsers.length === 0" class="text-xs text-gray-500 px-2 py-2">
                  Brak kont pasujących do wyszukiwania.
                </div>
              </div>
            </div>
            
          </div>
        </div>

        <div class="p-6 md:p-8 pt-4 border-t border-gray-100 flex gap-3 shrink-0 rounded-b-3xl bg-white">
          <button @click="showModal = false" class="flex-1 py-3 text-gray-600 font-bold hover:bg-gray-100 rounded-xl transition-colors">Anuluj</button>
          <button @click="saveClient" class="flex-1 py-3 bg-blue-600 text-white rounded-xl font-bold hover:bg-blue-700 shadow-md transition-all active:scale-95">Zapisz</button>
        </div>
        
      </div>
    </div>

    <div v-if="confirmModal.show" class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center p-4 z-[100]">
      <div class="bg-white rounded-3xl p-6 md:p-8 w-full max-w-sm shadow-2xl animate-in zoom-in duration-200 text-center">
        <div class="w-16 h-16 bg-red-100 text-red-500 rounded-full flex items-center justify-center mx-auto mb-4">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="w-8 h-8">
            <path stroke-linecap="round" stroke-linejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
        </div>
        <h2 class="text-2xl font-bold mb-2 text-gray-800">Czy na pewno?</h2>
        <p class="text-gray-500 mb-8 text-sm">{{ confirmModal.message }}</p>
        <div class="flex gap-3">
          <button @click="confirmModal.show = false" class="flex-1 py-3 text-gray-600 font-bold hover:bg-gray-100 rounded-xl transition-colors">Anuluj</button>
          <button @click="confirmModal.action" class="flex-1 py-3 bg-red-500 text-white rounded-xl font-bold hover:bg-red-600 shadow-md transition-all">Usuń</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, inject, reactive, computed } from 'vue'
import axios from 'axios'

const showNotification = inject('showNotification')
const API_URL = 'https://localhost:7054/api/Admin/clients'
const USERS_API_URL = 'https://localhost:7054/api/Admin/users'
const axiosConfig = { withCredentials: true }

const clients = ref([])
const allUsers = ref([])
const userSearch = ref('')
const selectedUserIds = ref([])
const showModal = ref(false)
const isEditing = ref(false)

const validationErrors = ref({})

const currentClient = ref({
  id: null,
  name: '',
  description: '',
  city: '',
  postalCode: '',
  street: '',
  streetNumber: '',
  apartmentNumber: '',
  phone: ''
})

const filteredUsers = computed(() => {
  const query = userSearch.value.trim().toLowerCase()
  if (!query) return allUsers.value

  return allUsers.value.filter(u => {
    const fullName = `${u.name} ${u.surname}`.toLowerCase()
    return fullName.includes(query) || u.email.toLowerCase().includes(query)
  })
})

const confirmModal = reactive({ show: false, message: '', action: null })

const getError = (field) => {
  if (!validationErrors.value) return null;
  const lowerCaseField = field.charAt(0).toLowerCase() + field.slice(1);
  const upperCaseField = field.charAt(0).toUpperCase() + field.slice(1);
  return validationErrors.value[lowerCaseField] || validationErrors.value[upperCaseField] || null;
}

const fetchClients = async () => {
  try {
    const res = await axios.get(API_URL, axiosConfig)
    clients.value = res.data.map(cl => ({
      id: cl.id || cl.Id,
      name: cl.name || cl.Name,
      description: cl.description || cl.Description,
      city: cl.city || cl.City,
      postalCode: cl.postalCode || cl.PostalCode,
      street: cl.street || cl.Street,
      streetNumber: cl.streetNumber || cl.StreetNumber,
      apartmentNumber: cl.apartmentNumber || cl.ApartmentNumber,
      phone: cl.phone || cl.Phone,
      users: (cl.users || cl.Users || []).map(u => ({
        id: u.id || u.Id,
        name: u.name || u.Name,
        surname: u.surname || u.Surname,
        email: u.email || u.Email
      }))
    }))
  } catch (e) {
    showNotification('Nie udało się pobrać listy klientów.', 'error')
  }
}

const fetchUsers = async () => {
  try {
    const res = await axios.get(USERS_API_URL, axiosConfig)
    allUsers.value = res.data.map(u => ({
      id: u.id || u.Id,
      name: u.name || u.Name,
      surname: u.surname || u.Surname,
      email: u.email || u.Email
    }))
  } catch (e) {
    showNotification('Nie udało się pobrać listy kont użytkowników.', 'error')
  }
}

const openAddModal = () => {
  isEditing.value = false
  validationErrors.value = {}
  selectedUserIds.value = []
  userSearch.value = ''
  currentClient.value = {
    id: null,
    name: '',
    description: '',
    city: '',
    postalCode: '',
    street: '',
    streetNumber: '',
    apartmentNumber: '',
    phone: ''
  }
  showModal.value = true
}

const openEditModal = (client) => {
  isEditing.value = true
  validationErrors.value = {}
  selectedUserIds.value = (client.users || []).map(u => u.id)
  userSearch.value = ''
  currentClient.value = { ...client }
  showModal.value = true
}

const saveClient = async () => {
  validationErrors.value = {}
  let hasFrontendErrors = false

  if (!currentClient.value.name || currentClient.value.name.trim() === '') {
    validationErrors.value.Name = ['Nazwa klienta jest wymagana.']
    hasFrontendErrors = true
  }

  if (currentClient.value.postalCode) {
    const postalRegex = /^\d{2}-\d{3}$/
    if (!postalRegex.test(currentClient.value.postalCode)) {
      validationErrors.value.PostalCode = ['Kod pocztowy musi być w formacie XX-XXX (np. 00-000).']
      hasFrontendErrors = true
    }
  }

  if (currentClient.value.phone) {
    let cleanPhone = currentClient.value.phone.replace(/[\s\-()]/g, '');
    
    if (cleanPhone.startsWith('+48')) {
      cleanPhone = cleanPhone.slice(3);
    } else if (cleanPhone.startsWith('0048')) {
      cleanPhone = cleanPhone.slice(4);
    } else if (cleanPhone.startsWith('+')) {
      cleanPhone = cleanPhone.replace(/^\+\d{1,3}/, '');
    }

    const digitsOnlyRegex = /^\d{9,15}$/;

    if (!digitsOnlyRegex.test(cleanPhone)) {
      validationErrors.value.Phone = ['Podaj poprawny numer telefonu (minimum 9 cyfr właściwego numeru).']
      hasFrontendErrors = true
    }
  }

  if (hasFrontendErrors) {
    showNotification('Popraw błędy w formularzu przed zapisaniem.', 'error')
    return
  }

  try {
    const payload = {
      name: currentClient.value.name,
      description: currentClient.value.description,
      city: currentClient.value.city,
      postalCode: currentClient.value.postalCode,
      street: currentClient.value.street,
      streetNumber: currentClient.value.streetNumber,
      apartmentNumber: currentClient.value.apartmentNumber,
      phone: currentClient.value.phone
    }

    if (isEditing.value) {
      await axios.put(`${API_URL}/${currentClient.value.id}`, payload, axiosConfig)
      await axios.put(`${API_URL}/${currentClient.value.id}/users`, { userIds: selectedUserIds.value }, axiosConfig)
      showNotification('Zaktualizowano dane klienta.', 'success')
    } else {
      const createRes = await axios.post(API_URL, payload, axiosConfig)
      const createdClientId = createRes.data?.id ?? createRes.data?.Id

      if (createdClientId) {
        await axios.put(`${API_URL}/${createdClientId}/users`, { userIds: selectedUserIds.value }, axiosConfig)
      }

      showNotification('Dodano nowego klienta.', 'success')
    }

    showModal.value = false
    await fetchClients()
  } catch (e) {
    if (e.response && e.response.status === 400 && e.response.data.errors) {
      validationErrors.value = e.response.data.errors;
      showNotification('Serwer odrzucił dane - popraw błędy.', 'error')
    } else {
      showNotification('Błąd podczas zapisywania klienta.', 'error')
    }
  }
}

const confirmDelete = (client) => {
  confirmModal.message = `Czy na pewno chcesz usunąć klienta "${client.name}"? Spowoduje to problemy w przypisanych do niego kategoriach i zgłoszeniach.`
  confirmModal.action = async () => {
    try {
      await axios.delete(`${API_URL}/${client.id}`, axiosConfig)
      confirmModal.show = false
      await fetchClients()
      showNotification('Klient został usunięty.', 'success')
    } catch (e) {
      showNotification('Błąd: Ten klient jest prawdopodobnie powiązany z innymi danymi.', 'error')
      confirmModal.show = false
    }
  }
  confirmModal.show = true
}

onMounted(async () => {
  await Promise.all([fetchClients(), fetchUsers()])
})
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent; 
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background-color: transparent; 
  border-radius: 10px;
}
.custom-scrollbar:hover::-webkit-scrollbar-thumb,
.custom-scrollbar:active::-webkit-scrollbar-thumb {
  background-color: #d1d5db; 
}
</style>