<template>
  <div class="min-h-screen bg-gray-50 p-8">
    <div class="max-w-4xl mx-auto">
      <button @click="$router.push({ name: 'admin' })" class="mb-6 text-gray-500 hover:text-blue-600 flex items-center gap-2 cursor-pointer">← Wstecz</button>
      <div class="flex justify-between items-center mb-8">
        <h1 class="text-3xl font-bold text-gray-800">Kolejki Zgłoszeń</h1>
        <button @click="showAddModal = true" class="bg-blue-600 text-white px-6 py-2 rounded-lg font-bold hover:bg-blue-700 cursor-pointer">+ Nowa Kolejka</button>
      </div>

      <div class="grid gap-4">
        <div v-for="q in queues" :key="q.id" class="bg-white p-5 rounded-xl shadow-sm flex justify-between items-center border border-gray-100">
          <div>
            <h3 class="font-bold text-xl text-gray-800">{{ q.name }}</h3>
            <p class="text-sm text-gray-500 italic">Przypisanych agentów: {{ q.userCount }}</p>
          </div>
          <div class="flex gap-3">
            <button @click="openManageUsers(q)" class="px-4 py-2 bg-blue-50 text-blue-600 rounded-lg hover:bg-blue-100 font-semibold cursor-pointer">Agenci</button>
            <button @click="deleteQueue(q.id)" class="px-4 py-2 text-red-500 hover:bg-red-50 rounded-lg cursor-pointer">Usuń</button>
          </div>
        </div>
      </div>
    </div>

    <div v-if="selectedQueue" class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center p-4 z-50">
      <div class="bg-white rounded-2xl p-8 w-full max-w-lg shadow-2xl">
        <h2 class="text-2xl font-bold mb-2">{{ selectedQueue.name }}</h2>
        <p class="text-gray-500 text-sm mb-6">Lista osób widzących tę kolejkę:</p>
        <div class="mb-6 max-h-48 overflow-y-auto space-y-2 border-b pb-4">
          <div v-for="u in queueUsers" :key="u.id" class="flex justify-between items-center bg-gray-50 p-3 rounded-lg">
            <span class="font-medium text-gray-700">{{ u.name }} {{ u.surname }}</span>
            <button @click="removeUserFromQueue(u.id)" class="text-red-500 text-xs font-black uppercase tracking-widest">Usuń</button>
          </div>
        </div>
        <div class="space-y-3">
          <label class="text-xs font-bold text-gray-400 uppercase">Dodaj agenta do kolejki</label>
          <div class="flex gap-2">
            <select v-model="userIdToAdd" class="flex-grow p-2 border rounded-lg bg-white">
              <option v-for="user in allUsers" :key="user.id" :value="user.id">{{ user.name }} {{ user.surname }} ({{ user.email }})</option>
            </select>
            <button @click="addUserToQueue" class="bg-blue-600 text-white px-6 py-2 rounded-lg font-bold">DODAJ</button>
          </div>
        </div>
        <button @click="selectedQueue = null" class="w-full mt-8 py-3 bg-gray-100 rounded-xl font-bold text-gray-600 hover:bg-gray-200 transition-colors cursor-pointer">Zamknij okno</button>
      </div>
    </div>

    <div v-if="showAddModal" class="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
      <div class="bg-white rounded-2xl p-6 w-full max-w-sm shadow-2xl">
        <h2 class="text-xl font-bold mb-4">Tworzenie Kolejki</h2>
        <input v-model="newQueueName" type="text" class="w-full p-3 border rounded-lg mb-6 outline-none focus:ring-2 focus:ring-blue-500" placeholder="Nazwa (np. Serwis IT)" />
        <div class="flex gap-2">
          <button @click="showAddModal = false" class="flex-1 py-2 text-gray-500 font-medium">Anuluj</button>
          <button @click="createQueue" class="flex-1 py-2 bg-blue-600 text-white rounded-lg font-bold">Stwórz</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';

const API_URL = 'http://localhost:5066/api/Admin';
const queues = ref([]);
const showAddModal = ref(false);
const newQueueName = ref('');
const selectedQueue = ref(null);
const queueUsers = ref([]);
const allUsers = ref([]);
const userIdToAdd = ref(null);

const getHeaders = () => ({ headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } });

const fetchQueues = async () => {
  const res = await axios.get(`${API_URL}/queues`, getHeaders());
  queues.value = res.data;
};

const createQueue = async () => {
  if (!newQueueName.value) return;
  await axios.post(`${API_URL}/queues`, { name: newQueueName.value }, getHeaders());
  newQueueName.value = ''; showAddModal.value = false; fetchQueues();
};

const deleteQueue = async (id) => {
  if (confirm("Usunąć kolejkę?")) {
    try { await axios.delete(`${API_URL}/queues/${id}`, getHeaders()); fetchQueues(); }
    catch(e) { alert("Nie można usunąć kolejki ze zgłoszeniami!"); }
  }
};

const openManageUsers = async (queue) => {
  selectedQueue.value = queue;
  const res = await axios.get(`${API_URL}/queues/${queue.id}/users`, getHeaders());
  queueUsers.value = res.data;
  const allU = await axios.get(`${API_URL}/users`, getHeaders());
  allUsers.value = allU.data;
};

const addUserToQueue = async () => {
  if (!userIdToAdd.value) return;
  await axios.post(`${API_URL}/queues/${selectedQueue.value.id}/users/${userIdToAdd.value}`, {}, getHeaders());
  openManageUsers(selectedQueue.value);
};

const removeUserFromQueue = async (uId) => {
  await axios.delete(`${API_URL}/queues/${selectedQueue.value.id}/users/${uId}`, getHeaders());
  openManageUsers(selectedQueue.value);
};

onMounted(fetchQueues);
</script>