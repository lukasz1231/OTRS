<template>
  <div class="min-h-screen bg-bialeTlo p-8">
    <div class="max-w-6xl mx-auto">
      <h1 class="text-3xl font-bold text-gray-800 mb-8">Centrum Administracji</h1>
      
      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        
        <div @click="$router.push({ name: 'admin-users' })" class="bg-white p-8 rounded-2xl shadow-sm border border-gray-100 hover:border-blue-500 hover:shadow-md transition-all cursor-pointer group">
          <div class="w-14 h-14 bg-blue-50 rounded-xl flex items-center justify-center mb-6 group-hover:bg-blue-600 transition-colors text-blue-600 group-hover:text-white">
            <svg 
              xmlns="http://www.w3.org/2000/svg" 
              width="28" 
              height="28" 
              viewBox="0 0 24 24" 
              fill="none" 
              stroke="currentColor" 
              stroke-width="2" 
              stroke-linecap="round" 
              stroke-linejoin="round"
            >
              <path d="M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2"></path>
              <circle cx="12" cy="7" r="4"></circle>
            </svg>
          </div>
          <h2 class="text-2xl font-bold text-gray-800 mb-2">Użytkownicy</h2>
          <p class="text-gray-500">Zarządzaj kontami, rolami i hasłami pracowników.</p>
        </div>

        <div @click="$router.push({ name: 'admin-queues' })" class="bg-white p-8 rounded-2xl shadow-sm border border-gray-100 hover:border-indigo-500 hover:shadow-md transition-all cursor-pointer group">
          <div class="w-14 h-14 bg-indigo-50 rounded-xl flex items-center justify-center mb-6 group-hover:bg-indigo-600 transition-colors text-indigo-600 group-hover:text-white">
            <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <rect x="3" y="3" width="18" height="18" rx="2" ry="2"/>
              <line x1="3" y1="9" x2="21" y2="9"/>
            </svg>
          </div>
          <h2 class="text-2xl font-bold text-gray-800 mb-2">Kolejki</h2>
          <p class="text-gray-500">Konfiguruj kolejki zgłoszeń i przypisuj do nich zespoły.</p>
        </div>

        <div @click="$router.push({ name: 'admin-statuses' })" class="bg-white p-8 rounded-2xl shadow-sm border border-gray-100 hover:border-emerald-500 hover:shadow-md transition-all cursor-pointer group">
          <div class="w-14 h-14 bg-emerald-50 rounded-xl flex items-center justify-center mb-6 group-hover:bg-emerald-600 transition-colors text-emerald-600 group-hover:text-white">
            <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path>
              <polyline points="22 4 12 14.01 9 11.01"></polyline>
            </svg>
          </div>
          <h2 class="text-2xl font-bold text-gray-800 mb-2">Statusy</h2>
          <p class="text-gray-500">Definiuj etapy zgłoszeń (np. Nowy, W realizacji, Zamknięty).</p>
        </div>

        <div @click="$router.push({ name: 'admin-categories' })" class="bg-white p-8 rounded-2xl shadow-sm border border-gray-100 hover:border-orange-500 hover:shadow-md transition-all cursor-pointer group">
          <div class="w-14 h-14 bg-orange-50 rounded-xl flex items-center justify-center mb-6 group-hover:bg-orange-600 transition-colors text-orange-600 group-hover:text-white">
            <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m3 9 9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/><polyline points="9 22 9 12 15 12 15 22"/></svg>
          </div>
          <h2 class="text-2xl font-bold text-gray-800 mb-2">Kategorie</h2>
          <p class="text-gray-500">Zdefiniuj rodzaje usług (np. Sprzęt, Software).</p>
        </div>

        <div @click="$router.push({ name: 'admin-priorities' })" class="bg-white p-8 rounded-2xl shadow-sm border border-gray-100 hover:border-red-500 hover:shadow-md transition-all cursor-pointer group">
          <div class="w-14 h-14 bg-red-50 rounded-xl flex items-center justify-center mb-6 group-hover:bg-red-600 transition-colors text-red-600 group-hover:text-white">
            <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 2v20M2 12h20"/></svg>
          </div>
          <h2 class="text-2xl font-bold text-gray-800 mb-2">Priorytety</h2>
          <p class="text-gray-500">Ustal stopnie ważności zgłoszeń i ich poziomy.</p>
        </div>

      </div>
    </div>
  </div>
</template>