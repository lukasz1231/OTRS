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

        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div>
            <label class="block text-sm font-semibold text-tekstSzaryCiemny mb-2">
              Typ <span class="text-orange-500">*</span>
            </label>
            <select
              v-model="form.typeId"
              required
              class="w-full px-4 py-3 border border-gray-200 rounded-xl text-sm text-tekstSzaryCiemny focus:outline-none focus:border-przyciskiNiebieski focus:ring-1 focus:ring-przyciskiNiebieski bg-transparent appearance-none bg-no-repeat bg-[position:right_1rem_center] bg-[length:1.2em_1.2em]"
              style="
                background-image: url(&quot;data:image/svg+xml;charset=UTF-8,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='%237392A7' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3e%3cpolyline points='6 9 12 15 18 9'%3e%3c/polyline%3e%3c/svg%3e&quot;);
              "
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
              style="
                background-image: url(&quot;data:image/svg+xml;charset=UTF-8,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='%237392A7' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3e%3cpolyline points='6 9 12 15 18 9'%3e%3c/polyline%3e%3c/svg%3e&quot;);
              "
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
              required
              class="w-full px-4 py-3 border border-gray-200 rounded-xl text-sm text-tekstSzaryCiemny focus:outline-none focus:border-przyciskiNiebieski focus:ring-1 focus:ring-przyciskiNiebieski bg-transparent appearance-none bg-no-repeat bg-[position:right_1rem_center] bg-[length:1.2em_1.2em]"
              style="
                background-image: url(&quot;data:image/svg+xml;charset=UTF-8,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='%237392A7' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3e%3cpolyline points='6 9 12 15 18 9'%3e%3c/polyline%3e%3c/svg%3e&quot;);
              "
            >
              <option value="" disabled>Wybierz kategorię</option>
              <option v-for="category in categories" :key="category.id" :value="category.id">
                {{ category.name }}
              </option>
            </select>
          </div>
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
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'
import axios from 'axios'

const router = useRouter()
const userStore = useUserStore()
const API_BASE_URL = 'https://localhost:7054/api/Admin'
const axiosConfig = { withCredentials: true }

const form = reactive({
  title: '',
  description: '',
  typeId: '',
  priorityId: '',
  categoryId: '',
})

const isSubmitting = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const isLoading = ref(true)

const types = ref([])
const priorities = ref([])
const categories = ref([])

onMounted(async () => {
    const user = userStore.user
  const hasHelpdeskAccess = user?.roles?.some(role => 
    ['Admin', 'Helpdesk', 'Technik'].includes(role)
  )
  if (hasHelpdeskAccess) {
    router.push({ name: 'problemReportHelpdesk' })
    return
  }
  try {
    const [resTypes, resPrios, resCats] = await Promise.all([
      axios.get(`${API_BASE_URL}/types`, axiosConfig),
      axios.get(`${API_BASE_URL}/priorities`, axiosConfig),
      axios.get(`${API_BASE_URL}/categories`, axiosConfig),
    ])

    types.value = resTypes.data.map((t) => ({ id: t.id || t.Id, name: t.name || t.Name }))
    priorities.value = resPrios.data.map((p) => ({ id: p.id || p.Id, name: p.name || p.Name }))
    categories.value = resCats.data.map((c) => ({ id: c.id || c.Id, name: c.name || c.Name }))
  } catch (error) {
    console.error('Błąd ładowania danych słownikowych:', error)
    errorMessage.value = 'Nie udało się pobrać opcji z serwera. Sprawdź połączenie z API.'
  }finally {
    isLoading.value = false
  }
})

const submitTicket = async () => {
  isSubmitting.value = true
  errorMessage.value = ''
  successMessage.value = ''

  try {
    const payload = {
      Title: form.title,
      Description: form.description,
      Client: 'Hustletrack ITSM', // Przykładowy klient
      TypeId: Number(form.typeId),
      PriorityId: Number(form.priorityId),
      CategoryId: Number(form.categoryId),
      QueueId: 1, // Domyślna kolejka
    }

    const response = await fetch('/api/ticket', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
      body: JSON.stringify(payload),
    })

    if (!response.ok) {
      const errorData = await response.json()
      throw new Error(errorData.message || 'Wystąpił błąd podczas tworzenia zgłoszenia.')
    }

    const createdTicket = await response.json()

    successMessage.value =
      'Zgłoszenie zostało pomyślnie utworzone! Za chwilę zostaniesz przekierowany.'

    setTimeout(() => {
      const pId = createdTicket.publicId || createdTicket.PublicId;
      router.push(`/ticket/${pId}`)
    }, 4000)
  } catch (error) {
    errorMessage.value = error.message
  } finally {
    isSubmitting.value = false
  }
}

const cancel = () => {
  router.back()
}
</script>
