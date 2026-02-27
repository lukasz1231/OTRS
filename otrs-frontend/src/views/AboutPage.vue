<template>
  <div class="min-h-screen bg-gray-50 font-sans antialiased text-gray-800 pb-24">
    <main class="max-w-6xl mx-auto px-6 py-12 md:py-16">
      
      <section class="text-center mb-16 md:mb-20">
        <h1 class="text-4xl md:text-5xl font-bold text-[#3B71A3] tracking-tighter mb-6">
          Hustletrack ITSM
        </h1>
        <p class="text-lg md:text-xl text-gray-600 max-w-3xl mx-auto leading-relaxed">
          Platforma nowej generacji, która redefiniuje zarządzanie usługami IT. Łączymy stabilność systemu OTRS z nowoczesnym, intuicyjnym interfejsem użytkownika, dostarczając narzędzia gotowe na wyzwania współczesnego biznesu.
        </p>
      </section>

      <section class="grid grid-cols-1 md:grid-cols-3 gap-8 mb-16 md:mb-20">
        <div class="bg-white p-8 rounded-2xl shadow-sm border border-gray-100 hover:border-blue-100 transition duration-300">
          <div class="w-12 h-12 rounded-lg bg-blue-50 flex items-center justify-center text-[#3B71A3] mb-5 border border-blue-100">
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><path d="M12 8v4l3 3"/></svg>
          </div>
          <h3 class="text-xl font-bold text-gray-800 mb-3">Szybkość</h3>
          <p class="text-sm text-gray-600 leading-relaxed">Dashboard pozwala na błyskawiczny podgląd statusu spraw, co minimalizuje czas przestojów w Twojej firmie.</p>
        </div>

        <div class="bg-white p-8 rounded-2xl shadow-sm border border-gray-100 hover:border-blue-100 transition duration-300">
          <div class="w-12 h-12 rounded-lg bg-blue-50 flex items-center justify-center text-[#3B71A3] mb-5 border border-blue-100">
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>
          </div>
          <h3 class="text-xl font-bold text-gray-800 mb-3">Ludzie</h3>
          <p class="text-sm text-gray-600 leading-relaxed">Upraszczamy komunikację między użytkownikami a działami technicznymi poprzez intuicyjny Dashboard i szybki system zgłoszeń.</p>
        </div>

        <div class="bg-white p-8 rounded-2xl shadow-sm border border-gray-100 hover:border-blue-100 transition duration-300">
          <div class="w-12 h-12 rounded-lg bg-blue-50 flex items-center justify-center text-[#3B71A3] mb-5 border border-blue-100">
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z"/></svg>
          </div>
          <h3 class="text-xl font-bold text-gray-800 mb-3">Jakość</h3>
          <p class="text-sm text-gray-600 leading-relaxed">Wykorzystujemy sprawdzone rozwiązania Open Source, optymalizując je pod kątem wydajności i bezpieczeństwa danych naszych użytkowników.</p>
        </div>
      </section>

     <section class="bg-white p-10 md:p-16 rounded-3xl shadow-sm border border-gray-100 mb-16 md:mb-20 grid grid-cols-1 md:grid-cols-2 gap-12 items-center">
        <div>
          <h2 class="text-3xl font-bold text-[#3B71A3] tracking-tighter mb-6">Nasza Misja</h2>
          <div class="space-y-4 text-gray-600 leading-relaxed text-sm">
            <p>
              Wierzymy, że system ITSM nie musi być skomplikowany. Naszym celem jest dostarczenie platformy, która staje się naturalnym elementem codziennej pracy, a nie kolejnym trudnym narzędziem do opanowania.
            </p>
            <p>
              Hustletrack ITSM dąży do pełnej automatyzacji rutynowych zadań, aby technicy mogli skupić się na tym, co naprawdę ważne - rozwiązywaniu problemów i wspieraniu rozwoju biznesu naszych klientów.
            </p>
          </div>
        </div>
        
        <div class="p-8 bg-gray-50 rounded-2xl border border-gray-100 flex flex-col items-center justify-center text-center">
          <img 
            :src="logoUrl" 
            alt="Hustletrack Logo" 
            class="max-w-[200px] h-auto mb-6 drop-shadow-md"
          />
          <p class="text-xl font-bold text-gray-700 tracking-tight">
            Skup się na wsparciu, <br/> my zajmiemy się systemem.
          </p>
        </div>
      </section>

      <section class="text-center p-8 bg-white rounded-3xl border border-gray-100 shadow-sm">
        <h3 class="text-xs font-bold text-gray-400 uppercase tracking-widest mb-3">Napędzany przez silnik OTRS</h3>
        <p class="text-gray-600 max-w-xl mx-auto text-sm">
          Korzystamy ze stabilności i dojrzałości frameworku OTRS, dostarczając go w zupełnie nowej, przejrzystej formie. To sprawdzone rozwiązanie, które znasz, w wydaniu, które pokochasz.
        </p>
      </section>

    </main>
  </div>
</template>

<script setup>
import logoUrl from '@/assets/HustleTrackLogo 1.png';
</script>