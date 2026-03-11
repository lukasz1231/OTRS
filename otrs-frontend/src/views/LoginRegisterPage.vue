<template>
  <section
    id="auth"
    class="flex bg-bialeTlo items-center justify-center min-h-screen p-5 font-sans"
  >
    <div
      class="container max-w-lg flex flex-col bg-white p-5 rounded-2xl shadow-xl transition-all duration-300 ease-in-out"
    >
      <div class="flex flex-col items-center mb-8">
        <img src="../assets/HustleTrackLogo 1.png" alt="HustleTrack Logo" class="max-h-30 mb-2" />
        <p class="text-xs text-tekstSzary mt-1 text-center">
          System zarządzania zgłoszeniami serwisowymi
        </p>
      </div>

      <div class="flex bg-bialeTlo p-1 rounded-lg mb-6 relative">
        <button
          @click="switchTab('login')"
          class="flex-1 py-2 text-sm font-semibold rounded-md transition-all duration-300"
          :class="
            activeTab === 'login'
              ? 'bg-white shadow-sm text-tekstSzaryCiemny'
              : 'text-tekstSzary hover:text-tekstSzaryCiemny'
          "
        >
          Logowanie
        </button>
        <button
          @click="switchTab('register')"
          class="flex-1 py-2 text-sm font-semibold rounded-md transition-all duration-300"
          :class="
            activeTab === 'register'
              ? 'bg-white shadow-sm text-tekstSzaryCiemny'
              : 'text-tekstSzary hover:text-tekstSzaryCiemny'
          "
        >
          Rejestracja
        </button>
      </div>

      <form class="flex flex-col" @submit.prevent="handleSubmit">
        <transition name="expand">
          <div v-if="activeTab === 'register'" class="overflow-hidden px-1 -mx-1">
            <div class="pb-5 flex flex-col space-y-1.5">
              <label for="fullname" class="text-sm font-semibold text-tekstSzaryCiemny"
                >Imię i nazwisko</label
              >
              <input
                type="text"
                id="fullname"
                v-model="formData.fullname"
                @input="validateFullname"
                placeholder="Jan Kowalski"
                class="w-full border rounded-lg px-4 py-2.5 text-tekstSzaryCiemny focus:outline-none focus:ring-2 focus:ring-przyciskiNiebieski focus:border-transparent placeholder-placeholder"
                :class="errors.fullname ? 'border-red-500' : 'border-tekstSzary/20'"
                required
              />
              <p v-if="errors.fullname" class="text-red-500 text-xs mt-1">{{ errors.fullname }}</p>
            </div>
          </div>
        </transition>

        <div class="pb-5 flex flex-col space-y-1.5">
          <label for="email" class="text-sm font-semibold text-tekstSzaryCiemny">Adres email</label>
          <input
            type="email"
            id="email"
            v-model="formData.email"
            @input="validateEmail"
            placeholder="jankowalski@example.pl"
            class="w-full border rounded-lg px-4 py-2.5 text-tekstSzaryCiemny focus:outline-none focus:ring-2 focus:ring-przyciskiNiebieski focus:border-transparent placeholder-placeholder"
            :class="errors.email ? 'border-red-500' : 'border-tekstSzary/20'"
            required
          />
          <p v-if="errors.email" class="text-red-500 text-xs mt-1">{{ errors.email }}</p>
        </div>

        <div class="pb-5 flex flex-col space-y-1.5">
          <label for="password" class="text-sm font-semibold text-tekstSzaryCiemny">Hasło</label>
          <div class="relative">
            <input
              id="password"
              :type="showPassword ? 'text' : 'password'"
              v-model="formData.password"
              @input="validatePassword"
              placeholder="Hasło"
              class="w-full border rounded-lg pl-4 pr-10 py-2.5 text-tekstSzaryCiemny focus:outline-none focus:ring-2 focus:ring-przyciskiNiebieski focus:border-transparent placeholder-placeholder"
              :class="errors.password ? 'border-red-500' : 'border-tekstSzary/20'"
              required
            />
            <button
              type="button"
              @click="showPassword = !showPassword"
              class="absolute inset-y-0 right-0 flex items-center pr-3 text-tekstSzary hover:text-tekstSzaryCiemny focus:outline-none"
            >
              <svg
                v-if="!showPassword"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
                stroke-width="1.5"
                stroke="currentColor"
                class="w-5 h-5"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M2.036 12.322a1.012 1.012 0 010-.639C3.423 10.224 7.66 6.5 12 6.5s8.577 3.724 9.964 5.183c.375.375.375.983 0 1.358C20.577 14.776 16.34 18.5 12 18.5s-8.577-3.724-9.964-5.183a1.012 1.012 0 010-.639z"
                />
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
                />
              </svg>
              <svg
                v-else
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
                stroke-width="1.5"
                stroke="currentColor"
                class="w-5 h-5"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M3.98 8.223A10.477 10.477 0 001.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.45 10.45 0 0112 4.5c4.756 0 8.773 3.162 10.065 7.498a10.522 10.522 0 01-4.293 5.774M6.228 6.228L3 3m3.228 3.228l3.65 3.65m7.894 7.894L21 21m-3.228-3.228l-3.65-3.65m0 0a3 3 0 10-4.243-4.243m4.243 4.243l-4.243-4.243"
                />
              </svg>
            </button>
          </div>
          <p v-if="errors.password" class="text-red-500 text-xs mt-1">{{ errors.password }}</p>
        </div>

        <transition name="expand">
          <div v-if="activeTab === 'register'" class="overflow-hidden px-1 -mx-1">
            <div class="pb-5 flex flex-col space-y-1.5">
              <label for="confirmPassword" class="text-sm font-semibold text-tekstSzaryCiemny"
                >Powtórz hasło</label
              >
              <div class="relative">
                <input
                  :type="showConfirmPassword ? 'text' : 'password'"
                  id="confirmPassword"
                  v-model="formData.confirmPassword"
                  @input="validateConfirmPassword"
                  placeholder="Powtórz hasło"
                  class="w-full border rounded-lg pl-4 pr-10 py-2.5 text-tekstSzaryCiemny focus:outline-none focus:ring-2 focus:ring-przyciskiNiebieski focus:border-transparent placeholder-placeholder"
                  :class="errors.confirmPassword ? 'border-red-500' : 'border-tekstSzary/20'"
                  required
                />
                <button
                  type="button"
                  @click="showConfirmPassword = !showConfirmPassword"
                  class="absolute inset-y-0 right-0 flex items-center pr-3 text-tekstSzary hover:text-tekstSzaryCiemny focus:outline-none"
                >
                  <svg
                    v-if="!showConfirmPassword"
                    xmlns="http://www.w3.org/2000/svg"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke-width="1.5"
                    stroke="currentColor"
                    class="w-5 h-5"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      d="M2.036 12.322a1.012 1.012 0 010-.639C3.423 10.224 7.66 6.5 12 6.5s8.577 3.724 9.964 5.183c.375.375.375.983 0 1.358C20.577 14.776 16.34 18.5 12 18.5s-8.577-3.724-9.964-5.183a1.012 1.012 0 010-.639z"
                    />
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
                    />
                  </svg>
                  <svg
                    v-else
                    xmlns="http://www.w3.org/2000/svg"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke-width="1.5"
                    stroke="currentColor"
                    class="w-5 h-5"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      d="M3.98 8.223A10.477 10.477 0 001.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.45 10.45 0 0112 4.5c4.756 0 8.773 3.162 10.065 7.498a10.522 10.522 0 01-4.293 5.774M6.228 6.228L3 3m3.228 3.228l3.65 3.65m7.894 7.894L21 21m-3.228-3.228l-3.65-3.65m0 0a3 3 0 10-4.243-4.243m4.243 4.243l-4.243-4.243"
                    />
                  </svg>
                </button>
              </div>
              <p v-if="errors.confirmPassword" class="text-red-500 text-xs mt-1">
                {{ errors.confirmPassword }}
              </p>
            </div>
          </div>
        </transition>

        <div v-if="globalError" class="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg">
          <p class="text-red-600 text-sm text-center">{{ globalError }}</p>
        </div>

        <button
          type="submit"
          :disabled="isLoading || !isFormValid"
          class="w-full bg-przyciskiNiebieski hover:opacity-90 text-white font-semibold py-3 rounded-lg transition-colors mt-2 shadow-sm flex justify-center items-center disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <svg
            v-if="isLoading"
            class="animate-spin -ml-1 mr-3 h-5 w-5 text-white"
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
          >
            <circle
              class="opacity-25"
              cx="12"
              cy="12"
              r="10"
              stroke="currentColor"
              stroke-width="4"
            ></circle>
            <path
              class="opacity-75"
              fill="currentColor"
              d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
            ></path>
          </svg>
          <span v-if="!isLoading">{{
            activeTab === 'login' ? 'Zaloguj się' : 'Zarejestruj się'
          }}</span>
          <span v-else>{{ activeTab === 'login' ? 'Logowanie...' : 'Rejestracja...' }}</span>
        </button>

        <transition name="expand">
          <div v-if="activeTab === 'login'" class="mt-6 text-center">
            <button
              type="button"
              @click="goToResetPassword"
              class="text-sm text-tekstSzaryCiemny hover:text-black font-medium transition-colors flex items-center justify-center gap-1 mx-auto cursor-pointer"
            >
              Zapomniałeś hasła?
            </button>
          </div>
        </transition>
      </form>
    </div>
  </section>
</template>

<script setup>
import { reactive, ref, computed, inject, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'

const route = useRoute()
const router = useRouter()
const showNotification = inject('showNotification')
const userStore = useUserStore()

const activeTab = computed(() => {
  return route.name === 'register' ? 'register' : 'login'
})

watch(activeTab, () => {
  globalError.value = ''
})

const showPassword = ref(false)
const showConfirmPassword = ref(false)
const isLoading = ref(false)
const globalError = ref('')

const formData = reactive({
  fullname: '',
  email: '',
  password: '',
  confirmPassword: '',
})

const errors = reactive({
  fullname: '',
  email: '',
  password: '',
  confirmPassword: '',
})

const validateFullname = () => {
  if (activeTab.value === 'register') {
    if (!formData.fullname.trim()) {
      errors.fullname = 'Imię i nazwisko jest wymagane'
    } else if (formData.fullname.trim().length < 3) {
      errors.fullname = 'Imię i nazwisko musi mieć co najmniej 3 znaki'
    } else {
      errors.fullname = ''
    }
  }
}

const validateEmail = () => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!formData.email) {
    errors.email = 'Email jest wymagany'
  } else if (!emailRegex.test(formData.email)) {
    errors.email = 'Podaj poprawny adres email'
  } else {
    errors.email = ''
  }
}

const validatePassword = () => {
  if (!formData.password) {
    errors.password = 'Hasło jest wymagane'
  } else if (formData.password.length < 8) {
    errors.password = 'Hasło musi mieć co najmniej 8 znaków'
  } else {
    errors.password = ''
  }

  if (activeTab.value === 'register' && formData.confirmPassword) {
    validateConfirmPassword()
  }
}

const validateConfirmPassword = () => {
  if (activeTab.value === 'register') {
    if (!formData.confirmPassword) {
      errors.confirmPassword = 'Potwierdzenie hasła jest wymagane'
    } else if (formData.password !== formData.confirmPassword) {
      errors.confirmPassword = 'Hasła nie są identyczne'
    } else {
      errors.confirmPassword = ''
    }
  }
}

const isFormValid = computed(() => {
  validateEmail()

  if (!formData.email || errors.email) return false
  if (!formData.password || errors.password) return false

  if (activeTab.value === 'register') {
    validateFullname()
    validateConfirmPassword()
    return !errors.fullname && !errors.confirmPassword
  }

  return true
})

const switchTab = (tabName) => {
  Object.keys(errors).forEach((key) => (errors[key] = ''))
  globalError.value = ''
  router.push({ name: tabName })
}

const goToResetPassword = () => {
  router.push({ name: 'forgot-password' })
}

const handleSubmit = async () => {
  validateEmail()
  validatePassword()

  if (activeTab.value === 'register') {
    validateFullname()
    validateConfirmPassword()
  }

  const hasErrors = Object.values(errors).some((error) => error)
  if (hasErrors) {
    return
  }

  isLoading.value = true
  globalError.value = ''

  try {
    let backendUrl = '/api/Auth/login'
    let body = {
      email: formData.email,
      password: formData.password,
    }

    if (activeTab.value === 'register') {
      backendUrl = '/api/Auth/register'
      body = {
        fullname: formData.fullname,
        email: formData.email,
        password: formData.password,
      }
    }

    const response = await fetch(backendUrl, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
      body: JSON.stringify(body),
    })

    let data
    const contentType = response.headers.get('content-type') || ''

    if (contentType.includes('application/json')) {
      data = await response.json()
    } else {
      data = await response.text()
    }

    // 1. Ulepszona obsługa błędów (z main)
    if (!response.ok) {
      let errorMessage = 'Wystąpił błąd. Spróbuj ponownie.'

      if (typeof data === 'string') {
        errorMessage = data
      } else if (data.title) {
        errorMessage = data.title
      } else if (data.message) {
        errorMessage = data.message
      } else if (data.errors) {
        errorMessage = Object.values(data.errors).flat()[0]
      }

      throw new Error(errorMessage)
    }

    // 2. Bezpieczny zapis do Pinia (z HEAD)
    userStore.setUser(data.user)

    // 3. Powiadomienia (z main)
    showNotification(
      activeTab.value === 'login' ? 'Zalogowano pomyślnie!' : 'Konto utworzone pomyślnie!',
      'success',
    )

    // 4. Przekierowanie na podstawie ról, ale wyciąganych bezpiecznie z Pinii, a nie z tokena!
    const userRoles = data.user.roles || []

    if (userRoles.includes('Helpdesk') || userRoles.includes('Admin')) {
      router.push({ name: 'problemReportHelpdesk' })
    } else {
      router.push({ name: 'problemReportClient' })
    }

    router.push({ name: 'dashboard' })
  } catch (error) {
    console.error('Błąd:', error)
    globalError.value = error.message
    showNotification(error.message, 'error')
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
.expand-enter-active,
.expand-leave-active {
  transition: all 0.4s ease-in-out;
  max-height: 150px;
  opacity: 1;
}

.expand-enter-from,
.expand-leave-to {
  max-height: 0;
  opacity: 0;
  transform: translateY(-10px);
}
</style>
