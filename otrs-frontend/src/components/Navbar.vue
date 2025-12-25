<template>
  <nav class="sticky top-0 z-50 w-full bg-bialeTlo shadow-md">
    <div class="max-w-6xl mx-auto px-6 py-4 md:flex md:items-center">
      
      <div class="flex items-center justify-between w-full md:w-auto">
        <div class="text-2xl font-bold text-przyciskiNiebieski tracking-tight cursor-pointer whitespace-nowrap">
          Hustletrack ITSM
        </div>

        <button 
          @click="toggleMenu" 
          class="md:hidden text-tekstSzary hover:text-tekstSzaryCiemny p-1 focus:outline-none"
        >
          <svg v-if="!isOpen" xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="4" x2="20" y1="12" y2="12"/><line x1="4" x2="20" y1="6" y2="6"/><line x1="4" x2="20" y1="18" y2="18"/></svg>
          <svg v-else xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M18 6 6 18"/><path d="m6 6 12 12"/></svg>
        </button>
      </div>

      <div 
        :class="[
          'md:flex md:flex-1 md:items-center md:justify-between md:ml-8 md:opacity-100 md:max-h-full md:mt-0',
          'transition-all duration-300 ease-in-out overflow-hidden',
          isOpen 
            ? 'max-h-[500px] opacity-100 mt-5'
            : 'max-h-0 opacity-0'
        ]"
      >
        
        <div class="flex flex-col gap-3 md:flex-row md:gap-2">
          <button 
            v-for="item in menuItems" 
            :key="item.id"
            @click="handleNavigation(item.id)"
            :class="[
              'flex items-center gap-2 px-4 py-2 text-sm font-medium rounded transition-all duration-200 w-full md:w-auto',
              activeTab === item.id 
                ? 'bg-przyciskiNiebieski text-white shadow-sm' 
                : 'text-tekstSzary hover:text-tekstSzaryCiemny'
            ]"
          >
            <component :is="item.icon" />
            {{ item.label }}
          </button>
        </div>

        <div class="border-t border-gray-300 my-4 md:hidden"></div>

        <button 
          @click="handleLogout"
          class="flex items-center justify-center w-full md:w-auto p-1.5 text-tekstSzary bg-white border border-placeholder rounded hover:bg-gray-200 hover:text-tekstSzaryCiemny transition-colors gap-2"
        >
          <IconLogout />
          <span class="md:hidden text-sm font-medium">Wyloguj się</span>
        </button>
      </div>

    </div>
  </nav>
</template>

<script setup>
import { ref, h } from 'vue';

const IconDashboard = () => h('svg', { xmlns:"http://www.w3.org/2000/svg", width:"18", height:"18", viewBox:"0 0 24 24", fill:"none", stroke:"currentColor", "stroke-width":"2", "stroke-linecap":"round", "stroke-linejoin":"round" }, [h('path', { d: "M2 9a3 3 0 0 1 0 6v2a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-2a3 3 0 0 1 0-6V7a2 2 0 0 0-2-2H4a2 2 0 0 0-2 2Z" }), h('path', { d: "M13 5v2" }), h('path', { d: "M13 17v2" }), h('path', { d: "M13 11v2" })]);
const IconLogout = () => h('svg', { xmlns:"http://www.w3.org/2000/svg", width:"20", height:"20", viewBox:"0 0 24 24", fill:"none", stroke:"currentColor", "stroke-width":"2", "stroke-linecap":"round", "stroke-linejoin":"round" }, [h('path', { d: "M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" }), h('polyline', { points: "16 17 21 12 16 7" }), h('line', { x1: "21", x2: "9", y1: "12", y2: "12" })]);

const menuItems = [
  { id: 'dashboard', label: 'Dashboard', icon: IconDashboard },
  { id: 'zgloszenia', label: 'Zgłoszenia', icon: IconDashboard }
];

const activeTab = ref('dashboard');
const isOpen = ref(false);

const toggleMenu = () => {
  isOpen.value = !isOpen.value;
};

const handleNavigation = (id) => {
  activeTab.value = id;
  isOpen.value = false;
  console.log(`Przełączono na zakładkę: ${id}`);
};

const handleLogout = () => {
  isOpen.value = false;
  console.log('Wylogowano użytkownika');
};
</script>