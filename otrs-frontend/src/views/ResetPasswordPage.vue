<template>
  <section id="auth" class="flex bg-bialeTlo items-center justify-center min-h-screen p-5 font-sans">
    
    <div class="container max-w-lg flex flex-col bg-white p-8 rounded-2xl shadow-xl transition-all duration-300 ease-in-out">
      
      <div class="flex flex-col items-center mb-8">
        <img src="../assets/HustleTrackLogo 1.png" alt="HustleTrack Logo" class="max-h-30 mb-4">
        
        <h2 class="text-xl font-bold text-tekstSzaryCiemny mb-2">Ustaw nowe hasło</h2>
        <p class="text-sm text-tekstSzary text-center">
          Wprowadź nowe hasło do swojego konta
        </p>
      </div>

      <form class="flex flex-col" @submit.prevent="handleSubmit">
        
        <div class="pb-5 flex flex-col space-y-1.5">
          <label for="password" class="text-sm font-semibold text-tekstSzaryCiemny">Hasło</label>
          <div class="relative">
            <input 
              id="password" 
              :type="showPassword ? 'text' : 'password'"
              v-model="formData.password"
              placeholder="Hasło" 
              class="w-full border border-tekstSzary/20 rounded-lg pl-4 pr-10 py-2.5 text-tekstSzaryCiemny focus:outline-none focus:ring-2 focus:ring-przyciskiNiebieski focus:border-transparent placeholder-placeholder"
              required
            />
            <button type="button" @click="showPassword = !showPassword" class="absolute inset-y-0 right-0 flex items-center pr-3 text-tekstSzary hover:text-tekstSzaryCiemny focus:outline-none">
               <svg v-if="!showPassword" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-5 h-5"><path stroke-linecap="round" stroke-linejoin="round" d="M2.036 12.322a1.012 1.012 0 010-.639C3.423 10.224 7.66 6.5 12 6.5s8.577 3.724 9.964 5.183c.375.375.375.983 0 1.358C20.577 14.776 16.34 18.5 12 18.5s-8.577-3.724-9.964-5.183a1.012 1.012 0 010-.639z" /><path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" /></svg>
               <svg v-else xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-5 h-5"><path stroke-linecap="round" stroke-linejoin="round" d="M3.98 8.223A10.477 10.477 0 001.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.45 10.45 0 0112 4.5c4.756 0 8.773 3.162 10.065 7.498a10.522 10.522 0 01-4.293 5.774M6.228 6.228L3 3m3.228 3.228l3.65 3.65m7.894 7.894L21 21m-3.228-3.228l-3.65-3.65m0 0a3 3 0 10-4.243-4.243m4.243 4.243l-4.243-4.243" /></svg>
            </button>
          </div>
        </div>

        <div class="pb-5 flex flex-col space-y-1.5">
              <label for="confirmPassword" class="text-sm font-semibold text-tekstSzaryCiemny">Powtórz hasło</label>
              <div class="relative">
                <input 
                  :type="showConfirmPassword ? 'text' : 'password'"
                  id="confirmPassword" 
                  v-model="formData.confirmPassword"
                  placeholder="Powtórz hasło" 
                  class="w-full border border-tekstSzary/20 rounded-lg pl-4 pr-10 py-2.5 text-tekstSzaryCiemny focus:outline-none focus:ring-2 focus:ring-przyciskiNiebieski focus:border-transparent placeholder-placeholder"
                  required
                />
                <button type="button" @click="showConfirmPassword = !showConfirmPassword" class="absolute inset-y-0 right-0 flex items-center pr-3 text-tekstSzary hover:text-tekstSzaryCiemny focus:outline-none">
                  <svg v-if="!showConfirmPassword" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-5 h-5"><path stroke-linecap="round" stroke-linejoin="round" d="M2.036 12.322a1.012 1.012 0 010-.639C3.423 10.224 7.66 6.5 12 6.5s8.577 3.724 9.964 5.183c.375.375.375.983 0 1.358C20.577 14.776 16.34 18.5 12 18.5s-8.577-3.724-9.964-5.183a1.012 1.012 0 010-.639z" /><path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" /></svg>
                  <svg v-else xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-5 h-5"><path stroke-linecap="round" stroke-linejoin="round" d="M3.98 8.223A10.477 10.477 0 001.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.45 10.45 0 0112 4.5c4.756 0 8.773 3.162 10.065 7.498a10.522 10.522 0 01-4.293 5.774M6.228 6.228L3 3m3.228 3.228l3.65 3.65m7.894 7.894L21 21m-3.228-3.228l-3.65-3.65m0 0a3 3 0 10-4.243-4.243m4.243 4.243l-4.243-4.243" /></svg>
                </button>
              </div>

              <p v-if="errorMessage" class="text-red-500 text-sm mt-1">
                {{ errorMessage }}
              </p>
            </div>

        <button 
          type="submit" 
          :disabled="isLoading"
          class="w-full bg-przyciskiNiebieski hover:opacity-90 text-white font-semibold py-3 rounded-lg transition-colors shadow-sm flex justify-center items-center disabled:opacity-70 disabled:cursor-not-allowed cursor-pointer"
        >
          <svg v-if="isLoading" class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
          <span v-if="!isLoading">Zmień hasło</span>
          <span v-else>Wysyłanie...</span>
        </button>

      </form>

    </div>
  </section>
</template>

<script setup>
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const showPassword = ref(false);
const showConfirmPassword = ref(false);
const isLoading = ref(false);
const errorMessage = ref('');

const formData = reactive({
  password: '',
  confirmPassword: ''
});

const handleSubmit = async () => {
  errorMessage.value = '';
  if (formData.password !== formData.confirmPassword) {
    errorMessage.value = 'Hasła nie są identyczne.';
    return;
  }
  if (formData.password.length < 8) {
    errorMessage.value = 'Hasło musi mieć co najmniej 8 znaków.';
    return;
  }

  isLoading.value = true;
  
  console.log(`Ustawianie nowego hasła`);
  
  await new Promise(resolve => setTimeout(resolve, 2000));
  
  isLoading.value = false;
  
};
</script>