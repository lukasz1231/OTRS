<template>
  <div class="min-h-screen bg-bialeTlo flex justify-center items-start p-4 md:pt-10">
    <div class="bg-white w-full max-w-4xl rounded-2xl shadow-sm p-6 md:p-10">
      <div class="mb-8">
        <h1 class="text-2xl font-bold text-tekstSzaryCiemny mb-2">Nowe zgłoszenie</h1>
        <p class="text-tekstSzary text-sm">
          Wypełnij formularz, aby utworzyć nowe zgłoszenie serwisowe
        </p>
      </div>

      <form @submit.prevent="submitTicket" class="space-y-6">
        <div>
          <label class="block text-sm font-semibold text-tekstSzaryCiemny mb-2">
            Tytuł zgłoszenia <span class="text-orange-500">*</span>
          </label>
          <input
            type="text"
            v-model="form.title"
            placeholder="Krótki opis problemu (5-50 znaków)"
            class="w-full px-4 py-3 border border-gray-200 rounded-xl text-sm text-tekstSzaryCiemny placeholder-placeholder focus:outline-none focus:border-przyciskiNiebieski focus:ring-1 focus:ring-przyciskiNiebieski transition-colors bg-transparent"
            minlength="5"
            maxlength="50"
            required
          />
        </div>

        <div>
          <label class="block text-sm font-semibold text-tekstSzaryCiemny mb-2">
            Opis zgłoszenia <span class="text-orange-500">*</span>
          </label>
          <textarea
            v-model="form.description"
            placeholder="Szczegółowy opis problemu (minimum 20 znaków)"
            class="w-full px-4 py-3 border border-gray-200 rounded-xl text-sm text-tekstSzaryCiemny placeholder-placeholder focus:outline-none focus:border-przyciskiNiebieski focus:ring-1 focus:ring-przyciskiNiebieski transition-colors resize-none bg-transparent"
            rows="5"
            minlength="20"
            required
          ></textarea>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <label class="block text-sm font-semibold text-tekstSzaryCiemny mb-2">
              Klient <span class="text-orange-500">*</span>
            </label>
            <select
              v-model="form.clientId"
              @change="handleClientChange"
              required
              class="w-full px-4 py-3 border border-gray-200 rounded-xl text-sm text-tekstSzaryCiemny focus:outline-none focus:border-przyciskiNiebieski focus:ring-1 focus:ring-przyciskiNiebieski bg-transparent appearance-none bg-no-repeat bg-[position:right_1rem_center] bg-[length:1.2em_1.2em]"
              style="background-image: url('data:image/svg+xml;charset=UTF-8,%3csvg xmlns=\'http://www.w3.org/2000/svg\' viewBox=\'0 0 24 24\' fill=\'none\' stroke=\'%237392A7\' stroke-width=\'2\' stroke-linecap=\'round\' stroke-linejoin=\'round\'%3e%3cpolyline points=\'6 9 12 15 18 9\'%3e%3c/polyline%3e%3c/svg%3e');"
            >
              <option value="" disabled>Wybierz klienta</option>
              <option v-for="client in clients" :key="client.id" :value="client.id">
                {{ client.name }}
              </option>
            </select>
          </div>

          <div>
            <label class="block text-sm font-semibold text-tekstSzaryCiemny mb-2">
              Typ <span class="text-orange-500">*</span>
            </label>
            <select
              v-model="form.typeId"
              required
              class="w-full px-4 py-3 border border-gray-200 rounded-xl text-sm text-tekstSzaryCiemny focus:outline-none focus:border-przyciskiNiebieski focus:ring-1 focus:ring-przyciskiNiebieski bg-transparent appearance-none bg-no-repeat bg-[position:right_1rem_center] bg-[length:1.2em_1.2em]"
              style="background-image: url('data:image/svg+xml;charset=UTF-8,%3csvg xmlns=\'http://www.w3.org/2000/svg\' viewBox=\'0 0 24 24\' fill=\'none\' stroke=\'%237392A7\' stroke-width=\'2\' stroke-linecap=\'round\' stroke-linejoin=\'round\'%3e%3cpolyline points=\'6 9 12 15 18 9\'%3e%3c/polyline%3e%3c/svg%3e');"
            >
              <option value="" disabled>Wybierz typ</option>
              <option v-for="t in types" :key="t.id" :value="t.id">
                {{ t.name }}
              </option>
            </select>
          </div>

          <div>
            <label class="block text-sm font-semibold text-tekstSzaryCiemny mb-2">
              Priorytet <span class="text-orange-500">*</span>
            </label>
            <select
              v-model="form.priorityId"
              required
              class="w-full px-4 py-3 border border-gray-200 rounded-xl text-sm text-tekstSzaryCiemny focus:outline-none focus:border-przyciskiNiebieski focus:ring-1 focus:ring-przyciskiNiebieski bg-transparent appearance-none bg-no-repeat bg-[position:right_1rem_center] bg-[length:1.2em_1.2em]"
              style="background-image: url('data:image/svg+xml;charset=UTF-8,%3csvg xmlns=\'http://www.w3.org/2000/svg\' viewBox=\'0 0 24 24\' fill=\'none\' stroke=\'%237392A7\' stroke-width=\'2\' stroke-linecap=\'round\' stroke-linejoin=\'round\'%3e%3cpolyline points=\'6 9 12 15 18 9\'%3e%3c/polyline%3e%3c/svg%3e');"
            >
              <option value="" disabled>Wybierz priorytet</option>
              <option v-for="priority in priorities" :key="priority.id" :value="priority.id">
                {{ priority.name }}
              </option>
            </select>
          </div>

          <div>
            <label class="block text-sm font-semibold text-tekstSzaryCiemny mb-2">
              Kategoria <span class="text-orange-500">*</span>
            </label>
            <select
              v-model="form.categoryId"
              :disabled="!form.clientId"
              required
              class="w-full px-4 py-3 border border-gray-200 rounded-xl text-sm text-tekstSzaryCiemny focus:outline-none focus:border-przyciskiNiebieski focus:ring-1 focus:ring-przyciskiNiebieski bg-transparent appearance-none bg-no-repeat bg-[position:right_1rem_center] bg-[length:1.2em_1.2em] disabled:opacity-50"
              style="background-image: url('data:image/svg+xml;charset=UTF-8,%3csvg xmlns=\'http://www.w3.org/2000/svg\' viewBox=\'0 0 24 24\' fill=\'none\' stroke=\'%237392A7\' stroke-width=\'2\' stroke-linecap=\'round\' stroke-linejoin=\'round\'%3e%3cpolyline points=\'6 9 12 15 18 9\'%3e%3c/polyline%3e%3c/svg%3e');"
            >
              <option value="" disabled>
                {{ form.clientId ? 'Wybierz kategorię' : 'Najpierw wybierz klienta' }}
              </option>
              <option v-for="category in filteredCategories" :key="category.id" :value="category.id">
                {{ category.name }}
              </option>
            </select>
          </div>
        </div>

        <div>
          <label class="block text-sm font-semibold text-tekstSzaryCiemny mb-2">
            Kolejka <span class="text-orange-500">*</span>
          </label>
          <select
            v-model="form.queueId"
            required
            class="w-full px-4 py-3 border border-gray-200 rounded-xl text-sm text-tekstSzaryCiemny focus:outline-none focus:border-przyciskiNiebieski focus:ring-1 focus:ring-przyciskiNiebieski bg-transparent appearance-none bg-no-repeat bg-[position:right_1rem_center] bg-[length:1.2em_1.2em]"
            style="background-image: url('data:image/svg+xml;charset=UTF-8,%3csvg xmlns=\'http://www.w3.org/2000/svg\' viewBox=\'0 0 24 24\' fill=\'none\' stroke=\'%237392A7\' stroke-width=\'2\' stroke-linecap=\'round\' stroke-linejoin=\'round\'%3e%3cpolyline points=\'6 9 12 15 18 9\'%3e%3c/polyline%3e%3c/svg%3e');"
          >
            <option value="" disabled>Wybierz kolejkę</option>
            <option v-for="queue in queues" :key="queue.id" :value="queue.id">
              {{ queue.name }}
            </option>
          </select>
        </div>

        <div v-if="errorMessage" class="p-4 bg-red-50 text-red-600 rounded-xl text-sm">
          {{ errorMessage }}
        </div>
        <div v-if="successMessage" class="p-4 bg-green-50 text-green-600 rounded-xl text-sm">
          {{ successMessage }}
        </div>

        <div class="flex flex-col sm:flex-row gap-4 pt-4">
          <button
            type="submit"
            :disabled="isSubmitting"
            class="flex-1 bg-przyciskiNiebieski hover:opacity-90 text-white font-semibold py-3 px-8 rounded-xl transition-opacity disabled:opacity-70 w-full sm:w-auto"
          >
            <span v-if="!isSubmitting">Utwórz zgłoszenie</span>
            <span v-else>Wysyłanie...</span>
          </button>

          <button
            type="button"
            @click="cancel"
            class="bg-white border border-gray-200 text-tekstSzaryCiemny hover:bg-gray-50 font-semibold py-3 px-8 rounded-xl transition-colors w-full sm:w-auto"
          >
            Anuluj
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import axios from 'axios'

const API_BASE_URL = 'https://localhost:7054/api/Admin'
const axiosConfig = { withCredentials: true }

const form = reactive({
  title: '',
  description: '',
  clientId: '', // Zmienione z 'client' na 'clientId' (int)
  typeId: '',
  priorityId: '',
  categoryId: '',
  queueId: '',
})

const isSubmitting = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

// REF-y NA DANE Z BAZY
const clients = ref([])
const types = ref([])
const priorities = ref([])
const allCategories = ref([])
const queues = ref([])

// POBIERANIE WSZYSTKIEGO Z BAZY PRZY STARCIE
onMounted(async () => {
  try {
    const [resTypes, resPrios, resCats, resQueues, resClients] = await Promise.all([
      axios.get(`${API_BASE_URL}/types`, axiosConfig),
      axios.get(`${API_BASE_URL}/priorities`, axiosConfig),
      axios.get(`${API_BASE_URL}/categories`, axiosConfig),
      axios.get(`${API_BASE_URL}/queues`, axiosConfig),
      axios.get(`${API_BASE_URL}/clients`, axiosConfig) // Dodany endpoint dla klientów
    ])

    // Mapowanie danych (obsługa PascalCase i camelCase)
    types.value = resTypes.data.map(t => ({ id: t.id || t.Id, name: t.name || t.Name }))
    priorities.value = resPrios.data.map(p => ({ id: p.id || p.Id, name: p.name || p.Name }))
    queues.value = resQueues.data.map(q => ({ id: q.id || q.Id, name: q.name || q.Name }))
    clients.value = resClients.data.map(cl => ({ id: cl.id || cl.Id, name: cl.name || cl.Name }))
    
    // Ważne: kategorie muszą mieć teraz clientId
    allCategories.value = resCats.data.map(c => ({ 
      id: c.id || c.Id, 
      name: c.name || c.Name, 
      clientId: c.clientId || c.ClientId 
    }))

  } catch (error) {
    console.error("Błąd ładowania danych słownikowych:", error)
    errorMessage.value = "Nie udało się pobrać opcji z serwera. Sprawdź połączenie z API."
  }
})

// LOGIKA FILTROWANIA KATEGORII NA PODSTAWIE WYBRANEGO ID KLIENTA
const filteredCategories = computed(() => {
  if (!form.clientId) return []
  return allCategories.value.filter(cat => cat.clientId === form.clientId)
})

// RESET KATEGORII PRZY ZMIANIE KLIENTA
const handleClientChange = () => {
  form.categoryId = ''
}

const submitTicket = async () => {
  if (isSubmitting.value) return
  
  isSubmitting.value = true
  errorMessage.value = ''
  successMessage.value = ''

  try {
    const payload = {
      Title: form.title,
      Description: form.description,
      ClientId: Number(form.clientId),
      TypeId: Number(form.typeId),
      PriorityId: Number(form.priorityId),
      CategoryId: Number(form.categoryId),
      QueueId: Number(form.queueId),
    }

    // Wysyłka do TicketController
    await axios.post('https://localhost:7054/api/Ticket', payload, axiosConfig)

    successMessage.value = 'Zgłoszenie zostało pomyślnie utworzone!'
    resetForm()
  } catch (error) {
    console.error(error)
    errorMessage.value = error.response?.data?.message || 'Wystąpił błąd podczas tworzenia zgłoszenia.'
  } finally {
    isSubmitting.value = false
  }
}

const resetForm = () => {
  form.title = ''
  form.description = ''
  form.clientId = ''
  form.typeId = ''
  form.priorityId = ''
  form.categoryId = ''
  form.queueId = ''
}

const cancel = () => {
  resetForm()
  // opcjonalnie: $router.back()
}
</script>