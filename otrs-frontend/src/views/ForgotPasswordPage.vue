<template>
  <section id="auth" class="flex bg-bialeTlo items-center justify-center min-h-screen p-5 font-sans">
    
    <div class="container max-w-lg flex flex-col bg-white p-8 rounded-2xl shadow-xl transition-all duration-300 ease-in-out">
      
      <div class="flex flex-col items-center mb-8">
        <img src="../assets/HustleTrackLogo 1.png" alt="HustleTrack Logo" class="max-h-30 mb-4">
        
        <h2 class="text-xl font-bold text-tekstSzaryCiemny mb-2">Zapomniałeś hasła?</h2>
        <p class="text-sm text-tekstSzary text-center">
          Wprowadź swój adres email, a wyślemy Ci link do resetowania hasła
        </p>
      </div>

      <form class="flex flex-col" @submit.prevent="handleSubmit">
        
        <div class="pb-6 flex flex-col space-y-1.5">
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

const formData = reactive({
  email: '',
});

const goBack = () => {
  router.push({ name: 'login' }); 
};

const handleSubmit = async () => {
  isLoading.value = true;
  
  console.log(`Wysyłanie linku resetującego na adres: ${email.value}`);
  
  await new Promise(resolve => setTimeout(resolve, 2000));
  
  isLoading.value = false;
  
};
</script>