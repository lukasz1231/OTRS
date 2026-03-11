<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import axios from 'axios'

const route = useRoute()
const ticket = ref(null)
const loading = ref(true)

const API_URL = `https://localhost:7054/api/ticket/${route.params.id}`
const axiosConfig = { withCredentials: true }

const fetchTicketDetails = async () => {
  try {
    const response = await axios.get(API_URL, axiosConfig)
    ticket.value = response.data
  } catch (error) {
    console.error('Błąd pobierania detali:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchTicketDetails()
})
</script>

<template>
  <div v-if="loading">Ładowanie...</div>
  <div v-else-if="ticket">
    <h1>Szczegóły ticketu {{ ticket.id }}</h1>
    <p>{{ ticket.message }}</p>
  </div>
</template>
