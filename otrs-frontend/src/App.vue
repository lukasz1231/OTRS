<script setup>
import { RouterView } from 'vue-router'
import Navbar from './components/Navbar.vue'
import Footer from './components/Footer.vue'
import Notification from './components/Notification.vue'
import { ref, provide } from 'vue'

const notifications = ref([])
let nextId = 1

const showNotification = (message, type = 'success') => {
  const id = nextId++
  notifications.value.push({ id, message, type })
}

const removeNotification = (id) => {
  const index = notifications.value.findIndex((n) => n.id === id)
  if (index > -1) {
    notifications.value.splice(index, 1)
  }
}

provide('showNotification', showNotification)
</script>

<template>
  <div class="flex flex-col min-h-screen">
    <div class="fixed top-4 right-4 z-50 space-y-2">
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

<style scoped></style>
