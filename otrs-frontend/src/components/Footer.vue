<template>
  <footer class="bg-[#2D373A] text-white pt-16 pb-8 px-6 font-sans">
    
    <div class="max-w-4xl mx-auto grid grid-cols-1 md:grid-cols-3 gap-4 mb-16">
      
      <div class="flex flex-col items-center md:items-start">
        <h3 class="font-bold text-[13px] mb-6 tracking-[0.2em] uppercase opacity-90">
          Aplikacja
        </h3>
        <ul class="space-y-4 text-gray-400 text-sm font-medium text-center md:text-left whitespace-nowrap">
          <li><router-link :to="{ name: 'dashboard' }" class="hover:text-white transition">Dashboard</router-link></li>
          <li><router-link :to="{ name: 'problemReportClient' }" class="hover:text-white transition">Załóż zgłoszenie</router-link></li>
          <li><router-link :to="{ name: 'problemReportHelpdesk' }" class="hover:text-white transition">Widok zgłoszeń</router-link></li>
        </ul>
      </div>

      <div class="flex flex-col items-center text-center">
        <h3 class="font-bold text-[13px] mb-6 tracking-[0.2em] uppercase opacity-90">
          Użytkownik
        </h3>
        <ul class="space-y-4 text-gray-400 text-sm font-medium md:text-left whitespace-nowrap">
          <li><router-link :to="{ name: 'profile' }" class="hover:text-white transition">Moje konto</router-link></li>
          <li><a href="#" class="hover:text-white transition">Centrum pomocy</a></li>
          <li><a href="#" class="hover:text-white transition">Dokumentacja API</a></li>
        </ul>
      </div>

      <div class="flex flex-col items-center md:items-end">
        <div class="md:text-left min-w-[120px]">
          <h3 class="font-bold text-[13px] mb-6 tracking-[0.2em] uppercase opacity-90 text-center md:text-left">
            Firma
          </h3>
          <ul class="space-y-4 text-gray-400 text-sm font-medium text-center md:text-left whitespace-nowrap">
            <li><router-link :to="{ name: 'about' }" class="hover:text-white transition">O nas</router-link></li>
            <li><router-link :to="{ name: 'contact' }" class="hover:text-white transition">Kontakt</router-link></li>
            <li><a href="#" class="hover:text-white transition">Partnerzy</a></li>
          </ul>
        </div>
      </div>

    </div>

    <div class="max-w-7xl mx-auto">
      <div class="bg-[#E9ECEF] text-[#495057] py-4 px-8 rounded-xl flex flex-col md:flex-row justify-between items-center text-[13px] font-semibold">
        <div class="flex flex-wrap justify-center gap-x-8 gap-y-2 mb-4 md:mb-0">
          <router-link :to="{ name: 'privacy'}" class="hover:text-black transition cursor-pointer">Ochrona prywatności</router-link>
          <router-link :to="{ name: 'terms'}" class="hover:text-black transition cursor-pointer">Zasady uzytkowania</router-link>
          <router-link :to="{ name: 'trademark'}" class="hover:text-black transition cursor-pointer">Znaki Towarowe</router-link>
        </div>
        <div class="whitespace-nowrap opacity-90">
          © Hustletrack ITSM 2025
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup>
</script>