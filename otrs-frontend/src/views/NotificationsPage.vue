<template>
  <div class="max-w-4xl mx-auto px-4 py-8">
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-800">Powiadomienia</h1>
      <button 
        v-if="unreadCount > 0" 
        @click="markAllAsRead" 
        class="px-4 py-2 text-sm font-medium text-white bg-przyciskiNiebieski rounded shadow hover:bg-opacity-90 transition-all cursor-pointer"
      >
        Odczytaj wszystkie
      </button>
    </div>

    <div v-if="isLoading" class="text-center py-10 text-gray-500">
      Ładowanie powiadomień...
    </div>
    
    <div v-else-if="notifications.length === 0" class="text-center py-10 text-gray-500 bg-white rounded-lg shadow-sm border border-gray-100">
      Nie masz żadnych powiadomień.
    </div>

    <div v-else class="bg-white rounded-lg shadow-sm border border-gray-100 overflow-hidden">
      <div 
        v-for="notification in notifications" 
        :key="notification.id"
        @click="handleNotificationClick(notification)"
        :class="[
          'p-5 border-b border-gray-100 last:border-b-0 cursor-pointer transition-colors hover:bg-gray-50 flex flex-col sm:flex-row gap-4 justify-between items-start',
          !notification.isRead ? 'bg-blue-50/30 border-l-4 border-l-przyciskiNiebieski' : 'border-l-4 border-l-transparent'
        ]"
      >
        <div class="flex-1">
          <div class="flex items-center gap-2 mb-1">
            <span v-if="!notification.isRead" class="w-2 h-2 rounded-full bg-przyciskiNiebieski"></span>
            <h3 class="font-semibold" :class="!notification.isRead ? 'text-gray-900' : 'text-gray-700'">
              {{ notification.title }}
            </h3>
          </div>
          <p class="text-sm mt-1" :class="!notification.isRead ? 'text-gray-800' : 'text-gray-500'">
            {{ notification.message }}
          </p>
        </div>
        <div class="text-xs text-gray-400 whitespace-nowrap mt-1 sm:mt-0">
          {{ formatDate(notification.createdAt) }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'

const router = useRouter()
const notifications = ref([])
const isLoading = ref(true)

const unreadCount = computed(() => notifications.value.filter(n => !n.isRead).length)

const fetchNotifications = async () => {
  try {
    const response = await axios.get('https://localhost:7054/api/notifications', { withCredentials: true })
    notifications.value = response.data
  } catch (error) {
    console.error('Błąd pobierania powiadomień:', error)
  } finally {
    isLoading.value = false
  }
}

const handleNotificationClick = async (notification) => {
  if (!notification.isRead) {
    try {
      await axios.patch(`https://localhost:7054/api/notifications/${notification.id}/read`, {}, { withCredentials: true })
      notification.isRead = true
    } catch (error) {
      console.error('Błąd oznaczania powiadomienia jako przeczytane:', error)
    }
  }
  
  if (notification.ticketPublicId) {
    router.push({ name: 'ticket-details', params: { id: notification.ticketPublicId } })
  }
}

const markAllAsRead = async () => {
  try {
    await axios.patch('https://localhost:7054/api/notifications/read-all', {}, { withCredentials: true })
    notifications.value.forEach(n => n.isRead = true)
  } catch (error) {
    console.error('Błąd oznaczania powiadomień jako przeczytane:', error)
  }
}

const formatDate = (dateString) => {
  const date = new Date(dateString)
  return date.toLocaleString('pl-PL', { 
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
  })
}

onMounted(() => {
  fetchNotifications()
})
</script>
