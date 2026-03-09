<template>
  <nav class="sticky top-0 z-50 w-full bg-bialeTlo shadow-md">
    <div class="max-w-6xl mx-auto px-6 py-4 md:flex md:items-center">
      
      <div class="flex items-center justify-between w-full md:w-auto">
        <div 
          @click="goToDashboard"
          class="text-2xl font-bold text-przyciskiNiebieski tracking-tight cursor-pointer whitespace-nowrap"
        >
          Hustletrack ITSM
        </div>

        <button 
          @click="toggleMenu" 
          class="md:hidden text-tekstSzary hover:text-tekstSzaryCiemny p-1 focus:outline-none cursor-pointer"
        >
          <svg v-if="!isOpen" xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="4" x2="20" y1="12" y2="12"/><line x1="4" x2="20" y1="6" y2="6"/><line x1="4" x2="20" y1="18" y2="18"/></svg>
          <svg v-else xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M18 6 6 18"/><path d="m6 6 12 12"/></svg>
        </button>
      </div>

      <div v-if="isAuthenticated"
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
              'flex items-center gap-2 px-4 py-2 text-sm font-medium rounded transition-all duration-200 w-full md:w-auto cursor-pointer',
              activeTab === item.id 
                ? 'bg-przyciskiNiebieski text-white shadow-sm' 
                : 'text-tekstSzary hover:text-tekstSzaryCiemny hover:bg-gray-100'
            ]"
          >
            <component :is="item.icon" />
            {{ item.label }}
          </button>
        </div>

        <div class="flex flex-col md:flex-row items-center gap-4 mt-4 md:mt-0">
          <div class="border-t border-gray-300 w-full md:hidden my-2"></div>
          
          <button 
            @click="goToProfile"
            class="flex items-center gap-2 px-4 py-2 text-sm font-medium text-tekstSzary hover:text-przyciskiNiebieski transition-all duration-200 w-full md:w-auto cursor-pointer"
          >
            <IconUser />
            <span>Moje konto</span>
          </button>

          <button 
            @click="handleLogout"
            class="flex items-center justify-center w-full md:w-auto p-1.5 text-tekstSzary bg-white border border-placeholder rounded hover:bg-gray-100 hover:text-red-500 transition-colors gap-2 cursor-pointer"
          >
            <IconLogout />
            <span class="md:hidden text-sm font-medium">Wyloguj się</span>
          </button>
        </div>
      </div>

      <div v-else class="md:flex md:items-center md:ml-auto">
        <button 
          @click="goToLogin"
          class="flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-przyciskiNiebieski rounded-lg hover:opacity-90 transition-all duration-200 cursor-pointer"
        >
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4"/><polyline points="10 17 15 12 10 7"/><line x1="15" x2="3" y1="12" y2="12"/></svg>
          <span>Zaloguj się</span>
        </button>
      </div>

    </div>
  </nav>
</template>

<script setup>
import { ref, h, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

// Ikony
const IconDashboard = () => h('svg', { xmlns:"http://www.w3.org/2000/svg", width:"18", height:"18", viewBox:"0 0 24 24", fill:"none", stroke:"currentColor", "stroke-width":"2", "stroke-linecap":"round", "stroke-linejoin":"round" }, [h('path', { d: "M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z" }), h('polyline', { points: "9 22 9 12 15 12 15 22" })]);
const IconTicket = () => h('svg', { xmlns:"http://www.w3.org/2000/svg", width:"18", height:"18", viewBox:"0 0 24 24", fill:"none", stroke:"currentColor", "stroke-width":"2", "stroke-linecap":"round", "stroke-linejoin":"round" }, [h('path', { d: "M2 16l4 4 4-4" }), h('path', { d: "M4 12V4h16v8" }), h('path', { d: "M10 20h8v-8" })]);
const IconLogout = () => h('svg', { xmlns:"http://www.w3.org/2000/svg", width:"20", height:"20", viewBox:"0 0 24 24", fill:"none", stroke:"currentColor", "stroke-width":"2", "stroke-linecap":"round", "stroke-linejoin":"round" }, [h('path', { d: "M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" }), h('polyline', { points: "16 17 21 12 16 7" }), h('line', { x1: "21", x2: "9", y1: "12", y2: "12" })]);
const IconUser = () => h('svg', { xmlns:"http://www.w3.org/2000/svg", width:"20", height:"20", viewBox:"0 0 24 24", fill:"none", stroke:"currentColor", "stroke-width":"2", "stroke-linecap":"round", "stroke-linejoin":"round" }, [h('path', { d: "M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2" }), h('circle', { cx: "12", cy: "7", r: "4" })]);

const menuItems = [
  { id: 'dashboard', label: 'Dashboard', icon: IconDashboard },
  { id: 'problemReportClient', label: 'Zgłoszenia', icon: IconTicket }
];

const activeTab = ref('dashboard');
const isOpen = ref(false);
const isAuthenticated = ref(!!localStorage.getItem('token'));

const updateAuthState = () => {
  isAuthenticated.value = !!localStorage.getItem('token');
};

const handleAuthChange = () => {
  updateAuthState();
};

onMounted(() => {
  const currentPath = router.currentRoute.value.name;
  if (currentPath && menuItems.some(item => item.id === currentPath)) {
    activeTab.value = currentPath;
  }
  
  window.addEventListener('auth-change', handleAuthChange);
  
  window.addEventListener('storage', (e) => {
    if (e.key === 'token') {
      updateAuthState();
    }
  });
});

onUnmounted(() => {
  window.removeEventListener('auth-change', handleAuthChange);
});

const toggleMenu = () => {
  isOpen.value = !isOpen.value;
};

const handleNavigation = (id) => {
  activeTab.value = id;
  isOpen.value = false;
  router.push({ name: id });
};

const goToDashboard = () => {
  if (isAuthenticated.value) {
    activeTab.value = 'dashboard';
    router.push({ name: 'dashboard' });
  } else {
    router.push({ name: 'login' });
  }
};

const goToProfile = () => {
  isOpen.value = false;
  activeTab.value = 'profile';
  router.push({ name: 'profile' }); 
};

const goToLogin = () => {
  router.push({ name: 'login' });
};

const handleLogout = () => {
  localStorage.removeItem('token');
  isAuthenticated.value = false;
  isOpen.value = false;
  router.push({ name: 'login' });
};
</script>