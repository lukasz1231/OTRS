<template>
  <section id="auth" class="flex bg-bialeTlo items-center justify-center min-h-screen p-5 font-sans">
    
    <div class="container max-w-lg flex flex-col bg-white p-8 rounded-2xl shadow-xl transition-all duration-300 ease-in-out">
      
      <div class="flex flex-col items-center mb-8">
        <img src="../assets/HustleTrackLogo 1.png" alt="HustleTrack Logo" class="max-h-30 mb-4">
        
        <h2 class="text-xl font-bold text-tekstSzaryCiemny mb-2">
          {{ isSuccess ? 'Link wysłany' : 'Zapomniałeś hasła?' }}
        </h2>
        <p class="text-sm text-center" :class="isSuccess ? 'text-green-600 font-medium' : 'text-tekstSzary'">
          {{ isSuccess ? 'Jeżeli podany email istnieje w naszej bazie, wysłaliśmy na niego instrukcje resetowania hasła.' : 'Wprowadź swój adres email, a wyślemy Ci link do resetowania hasła' }}
        </p>
      </div>

      <div v-if="isSuccess" class="flex flex-col items-center justify-center">
        <button 
          @click="goBack" 
          class="w-full bg-przyciskiNiebieski hover:opacity-90 text-white font-semibold py-3 rounded-lg transition-colors shadow-sm flex justify-center items-center cursor-pointer"
        >
          Powrót do logowania
        </button>
      </div>

      <form v-else class="flex flex-col" @submit.prevent="handleSubmit">
        
        <div class="pb-1 flex flex-col space-y-1.5">
          <label for="email" class="text-sm font-semibold text-tekstSzaryCiemny">Adres email</label>
          <input 
            type="email" 
            id="email" 
            v-model="formData.email"
            placeholder="jankowalski@example.pl" 
            class="w-full border border-tekstSzary/20 rounded-lg px-4 py-2.5 text-tekstSzaryCiemny focus:outline-none focus:ring-2 focus:ring-przyciskiNiebieski focus:border-transparent placeholder-placeholder"
            required
          />
        </div>
        
        <div class="h-6 mb-2">
            <p v-if="errorMessage" class="text-red-500 text-sm">
              {{ errorMessage }}
            </p>
        </div>

        <button 
          type="submit" 
          :disabled="isLoading"
          class="w-full bg-przyciskiNiebieski hover:opacity-90 text-white font-semibold py-3 rounded-lg transition-colors shadow-sm flex justify-center items-center disabled:opacity-70 disabled:cursor-not-allowed cursor-pointer"
        >
          <svg v-if="isLoading" class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
          <span v-if="!isLoading">Wyślij link resetujący</span>
          <span v-else>Wysyłanie...</span>
        </button>

        <div class="mt-6 text-center">
            <button 
                type="button"
                @click="goBack"
                class="text-sm text-tekstSzaryCiemny hover:text-black font-medium transition-colors flex items-center justify-center gap-1 mx-auto cursor-pointer"
            >
                Powrót
            </button>
        </div>

      </form>

    </div>
  </section>
</template>

<script setup>
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const isLoading = ref(false);
const isSuccess = ref(false);
const errorMessage = ref('');

const formData = reactive({
  email: '',
});

const goBack = () => {
  router.push({ name: 'login' }); 
};

const handleSubmit = async () => {
  isLoading.value = true;
  errorMessage.value = '';
  
  try {
    const response = await fetch('/api/Auth/forgot-password', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ email: formData.email })
    });

    if (!response.ok) {
      const errorData = await response.text();
      throw new Error(errorData || "Wystąpił błąd podczas wysyłania linku.");
    }

    isSuccess.value = true;
  } catch (error) {
    console.error(error);
    errorMessage.value = error.message;
  } finally {
    isLoading.value = false;
  }
};
</script>