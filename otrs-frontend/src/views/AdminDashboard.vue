<template>
  <div class="min-h-screen bg-gray-50 p-8">
    <div class="max-w-5xl mx-auto">
      <h1 class="text-3xl font-bold text-gray-800 mb-8">Centrum Administracji</h1>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
        <div @click="$router.push({ name: 'admin-users' })" class="bg-white p-8 rounded-2xl shadow-sm border border-gray-100 hover:border-blue-500 hover:shadow-md transition-all cursor-pointer group">
          <div class="w-14 h-14 bg-blue-50 rounded-xl flex items-center justify-center mb-6 group-hover:bg-blue-600 transition-colors text-blue-600 group-hover:text-white">
            <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/></svg>
          </div>
          <h2 class="text-2xl font-bold text-gray-800 mb-2">Użytkownicy</h2>
          <p class="text-gray-500">Zarządzaj kontami, rolami i hasłami pracowników.</p>
        </div>
        <div @click="$router.push({ name: 'admin-queues' })" class="bg-white p-8 rounded-2xl shadow-sm border border-gray-100 hover:border-indigo-500 hover:shadow-md transition-all cursor-pointer group">
          <div class="w-14 h-14 bg-indigo-50 rounded-xl flex items-center justify-center mb-6 group-hover:bg-indigo-600 transition-colors text-indigo-600 group-hover:text-white">
            <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="3" width="18" height="18" rx="2" ry="2"/><line x1="3" y1="9" x2="21" y2="9"/></svg>
          </div>
          <h2 class="text-2xl font-bold text-gray-800 mb-2">Kolejki</h2>
          <p class="text-gray-500">Konfiguruj kolejki zgłoszeń i przypisuj do nich zespoły.</p>
        </div>
      </div>
    </div>
  </div>
</template>