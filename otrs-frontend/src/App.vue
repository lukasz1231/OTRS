<script setup>
import { RouterView } from 'vue-router'
import Navbar from './components/Navbar.vue'
import Footer from './components/Footer.vue'
import Notification from './components/Notification.vue'
import { ref, provide, onMounted, onUnmounted } from 'vue'
import axios from 'axios'

// --- Notifications ---
const notifications = ref([])
let nextId = 1
const showNotification = (message, type = 'success') => {
  const id = nextId++
  notifications.value.push({ id, message, type })
}
const removeNotification = (id) => {
  const index = notifications.value.findIndex((n) => n.id === id)
  if (index > -1) notifications.value.splice(index, 1)
}
provide('showNotification', showNotification)

// --- HEARTBEAT & GUARD CONTROL ---
const updateUIStatus = (isOffline) => {
  const guard = document.getElementById('universal-guard');
  const loadingState = document.getElementById('guard-loading-state');
  const errorContent = document.getElementById('error-content');

  if (!guard) return;

  if (isOffline) {
    guard.classList.remove('hidden');
    guard.style.opacity = '1';
    if (loadingState) loadingState.style.display = 'none';
    if (errorContent) errorContent.style.display = 'flex'; 
  } else {
    guard.style.opacity = '0';
    setTimeout(() => guard.classList.add('hidden'), 400);
  }
}

const checkConnection = async () => {
  try {
    await axios.get('https://localhost:7054/api/Auth/me', { timeout: 2000 });
    updateUIStatus(false);
  } catch (error) {
    if (!error.response || error.response.status >= 500) {
      updateUIStatus(true);
    } else {
      updateUIStatus(false);
    }
  }
}

onMounted(() => {
  checkConnection();
  const timer = setInterval(checkConnection, 5000);
  onUnmounted(() => clearInterval(timer));
})
</script>

<template>
  <div class="flex flex-col min-h-screen">
    <div class="fixed top-4 right-4 z-100 space-y-2">
      <Notification
        v-for="notification in notifications"
        :key="notification.id"
        :message="notification.message"
        :type="notification.type"
        @closed="removeNotification(notification.id)"
      />
    </div>

    <Navbar />
    <main class="flex-grow">
      <RouterView />
    </main>
    <Footer />
  </div>
</template>