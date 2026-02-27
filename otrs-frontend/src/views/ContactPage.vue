<template>
  <div class="min-h-screen bg-gray-50 font-sans antialiased text-gray-800 pb-20">
    <main class="max-w-4xl mx-auto px-6 py-12">
      <div class="bg-white p-10 md:p-16 rounded-3xl shadow-sm border border-gray-100">
        <h1 class="text-3xl font-bold text-[#3B71A3] tracking-tight mb-8">Kontakt</h1>
        
        <div class="grid grid-cols-1 md:grid-cols-2 gap-12">
          <div class="space-y-8">
            <div>
              <h2 class="text-sm font-bold text-gray-400 uppercase tracking-widest mb-4">Dane firmy</h2>
              <p class="text-lg font-bold text-gray-800">Hustletrack Sp. z o.o.</p>
              <p class="text-gray-600">ul. Technologiczna 12</p>
              <p class="text-gray-600">15-001 Białystok</p>
            </div>

            <div class="space-y-4">
              <div class="flex items-center gap-3 text-gray-600">
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="#3B71A3" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z"/></svg>
                <span>+48 123 456 789</span>
              </div>
              <div class="flex items-center gap-3 text-gray-600">
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="#3B71A3" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"/><polyline points="22,6 12,13 2,6"/></svg>
                <span>kontakt@hustletrack.pl</span>
              </div>
            </div>

            <div class="p-6 bg-gray-50 rounded-2xl border border-gray-100">
              <p class="text-xs font-bold text-gray-500 uppercase mb-2">Wsparcie techniczne</p>
              <p class="text-sm text-gray-600">Dla zarejestrowanych użytkowników wsparcie dostępne jest całodobowo przez system zgłoszeń.</p>
            </div>
          </div>

          <form @submit.prevent class="space-y-4">
            <div>
              <label class="text-xs font-bold text-gray-500 uppercase ml-1">Temat</label>
              <input type="text" placeholder="W czym możemy pomóc?" class="w-full mt-1 p-3 rounded-xl border border-gray-200 focus:outline-none focus:ring-2 focus:ring-[#3B71A3] transition text-sm shadow-sm" />
            </div>
            <div>
              <label class="text-xs font-bold text-gray-500 uppercase ml-1">Wiadomość</label>
              <textarea rows="4" placeholder="Opisz swoje pytanie..." class="w-full mt-1 p-3 rounded-xl border border-gray-200 focus:outline-none focus:ring-2 focus:ring-[#3B71A3] transition text-sm shadow-sm resize-none"></textarea>
            </div>
            <button class="w-full py-3 bg-[#3B71A3] text-white font-bold rounded-xl hover:bg-blue-700 transition shadow-md active:scale-95 cursor-pointer">
              Wyślij wiadomość
            </button>
          </form>
        </div>
      </div>
    </main>
  </div>
</template>