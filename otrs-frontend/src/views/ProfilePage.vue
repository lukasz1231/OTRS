<template>
  <div class="min-h-screen bg-gray-50 font-sans antialiased text-gray-800 pb-12">
    
    <main class="max-w-6xl mx-auto px-6 py-12">
      <div class="bg-white p-8 md:p-10 rounded-2xl shadow-sm border border-gray-100">
        
        <section class="mb-12">
          <h1 class="text-3xl font-bold text-[#3B71A3] tracking-tight mb-8">Profil</h1>
          
          <div class="flex flex-col md:flex-row items-center gap-8 bg-gray-50 p-6 rounded-xl border border-gray-100 shadow-inner">
            <div class="flex-shrink-0 w-24 h-24 rounded-full bg-gray-200 flex items-center justify-center border-4 border-white shadow-sm">
              <IconUserLarge class="text-gray-400" />
            </div>
            
            <div class="flex-grow text-center md:text-left">
              <h2 class="text-2xl font-bold text-gray-700">{{ user.name }}</h2>
              <p class="text-gray-500 mb-1">{{ user.email }}</p>
              <span class="text-green-600 font-bold text-sm uppercase tracking-wider">{{ user.role }}</span>
            </div>
            
            <div class="flex-shrink-0">
              <button 
                @click="router.push({ name: 'reset-password' })"
                class="px-6 py-2 bg-[#3B71A3] text-white font-semibold rounded-lg hover:bg-blue-700 transition cursor-pointer shadow-md active:scale-95"
              >
                Zmień hasło
              </button>
            </div>
          </div>
        </section>

        <section>
          <div class="flex flex-col lg:flex-row justify-between items-start lg:items-center mb-8 gap-6">
            <div>
              <h2 class="text-2xl font-bold text-[#3B71A3] tracking-tight">Twoje tickety</h2>
              <p class="text-sm text-gray-500 mt-1">Zarządzaj swoimi zgłoszeniami i sprawdzaj ich status.</p>
            </div>

            <button 
              @click="router.push({ name: 'problemReportClient' })"
              class="flex items-center gap-2 px-5 py-2.5 bg-green-600 text-white font-bold rounded-lg hover:bg-green-700 transition shadow-sm cursor-pointer active:scale-95"
            >
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="M12 5v14"/></svg>
              Utwórz zgłoszenie
            </button>
          </div>

          <div class="flex flex-col md:flex-row gap-4 mb-8">
            <div class="flex bg-gray-100 p-1 rounded-xl w-fit">
              <button 
                v-for="filter in ['Wszystkie', 'W toku', 'Zakończone']" 
                :key="filter"
                @click="activeFilter = filter"
                :class="[
                  'px-5 py-2 text-sm font-bold rounded-lg transition-all cursor-pointer',
                  activeFilter === filter ? 'bg-white text-[#3B71A3] shadow-sm' : 'text-gray-500 hover:text-gray-700'
                ]"
              >
                {{ filter }}
              </button>
            </div>

            <div class="relative flex-grow">
              <input 
                v-model="searchQuery"
                type="text" 
                placeholder="Szukaj po ID lub tytule..." 
                class="w-full pl-10 pr-4 py-2.5 rounded-xl border border-gray-200 focus:outline-none focus:ring-2 focus:ring-[#3B71A3] transition text-sm bg-white"
              />
              <span class="absolute left-3 top-3 text-gray-400">
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/></svg>
              </span>
            </div>
          </div>

          <div class="space-y-4">
            <div 
              v-for="ticket in finalFilteredTickets" 
              :key="ticket.id"
              class="overflow-hidden rounded-xl border border-gray-200 shadow-sm hover:border-[#3B71A3]/50 transition-colors"
            >
              <div class="bg-[#3B71A3] text-white px-6 py-3 flex justify-between items-center text-sm font-bold">
                <span>Tytuł: {{ ticket.title }}</span>
                <span class="font-mono text-xs opacity-80 uppercase">ID: {{ ticket.publicId }}</span>
              </div>

              <div class="bg-[#7895A9] p-6 text-white text-sm">
                <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                  <div class="md:col-span-2">
                    <p class="font-bold mb-2 uppercase text-[10px] tracking-widest opacity-80">Opis zgłoszenia</p>
                    <p class="leading-relaxed text-gray-100 bg-black/10 p-3 rounded-lg">{{ ticket.description }}</p>
                  </div>
                  <div class="md:text-right flex flex-col justify-end space-y-3">
                    <p><span class="font-bold uppercase text-[10px] tracking-widest opacity-80 mr-2">Kolejka:</span> {{ ticket.queue }}</p>
                    <p>
                      <span class="font-bold uppercase text-[10px] tracking-widest opacity-80 mr-2">Status:</span>
                      <span :class="getStatusColor(ticket.status)">{{ ticket.status }}</span>
                    </p>
                  </div>
                </div>
              </div>
            </div>

            <div v-if="finalFilteredTickets.length === 0" class="text-center py-20 bg-gray-50 rounded-2xl border-2 border-dashed border-gray-200">
              <p class="text-gray-500 font-medium italic">Brak zgłoszeń pasujących do wybranych kryteriów.</p>
            </div>
          </div>
        </section>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, h, computed } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

// Reaktywne stany
const searchQuery = ref('');
const activeFilter = ref('Wszystkie');

const user = ref({
  name: 'Jan Kowalski',
  email: 'jan.kowalski@hustletrack.pl',
  role: 'Użytkownik',
});

const tickets = ref([
  { id: 1, publicId: 'PL38271321', title: 'Problem z drukarką', description: 'Drukarka w biurze nie przyjmuje żółtego tuszu.', queue: 'IT Support', status: 'Przyjęte' },
  { id: 2, publicId: 'PL99283100', title: 'Dostęp VPN', description: 'Nie mogę połączyć się z domowego biura.', queue: 'Sieci', status: 'W toku' },
  { id: 3, publicId: 'PL10293847', title: 'Wymiana myszki', description: 'Myszka przestała działać poprawnie.', queue: 'Magazyn', status: 'Rozwiązane' },
  { id: 4, publicId: 'PL55443322', title: 'Nowy Laptop', description: 'Zgłoszenie o wymianę sprzętu na nowszy model.', queue: 'Hardware', status: 'Nowe' }
]);

// LOGIKA FILTROWANIA (Filtry + Wyszukiwarka)
const finalFilteredTickets = computed(() => {
  return tickets.value.filter(t => {
    // 1. Filtr kategorii
    let matchesFilter = true;
    if (activeFilter.value === 'W toku') {
      matchesFilter = ['Przyjęte', 'W toku', 'Nowe'].includes(t.status);
    } else if (activeFilter.value === 'Zakończone') {
      matchesFilter = ['Rozwiązane', 'Zamknięte'].includes(t.status);
    }

    // 2. Wyszukiwarka tekstowa
    const term = searchQuery.value.toLowerCase();
    const matchesSearch = t.title.toLowerCase().includes(term) || t.publicId.toLowerCase().includes(term);

    return matchesFilter && matchesSearch;
  });
});

// Helper do kolorowania statusów
const getStatusColor = (status) => {
  if (['Rozwiązane', 'Zamknięte'].includes(status)) return 'text-gray-300 underline';
  if (status === 'W toku') return 'text-yellow-300 font-bold';
  if (status === 'Przyjęte') return 'text-green-300 font-bold';
  return 'text-blue-200';
};

const IconUserLarge = () => h('svg', { xmlns:"http://www.w3.org/2000/svg", width:"48", height:"48", viewBox:"0 0 24 24", fill:"none", stroke:"currentColor", "stroke-width":"2", "stroke-linecap":"round", "stroke-linejoin":"round" }, [h('path', { d: "M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2" }), h('circle', { cx: "12", cy: "7", r: "4" })]);
</script>