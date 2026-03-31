<template>
  <div class="min-h-screen bg-[#F8FAFC] font-sans antialiased text-gray-900 pb-12">
    <nav class="bg-white border-b border-gray-200 sticky top-0 z-10">
      <div class="max-w-5xl mx-auto px-4 md:px-6 py-4 flex items-center justify-between">
        <button
          @click="router.push({ name: 'profile' })"
          class="flex items-center gap-2 text-gray-500 hover:text-[#3B71A3] transition-colors font-bold text-sm group"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="20"
            height="20"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2.5"
            stroke-linecap="round"
            stroke-linejoin="round"
            class="group-hover:-translate-x-1 transition-transform"
          >
            <path d="m15 18-6-6 6-6" />
          </svg>
          <span class="hidden md:inline">Powrót do listy</span>
        </button>
        <div
          class="text-[10px] md:text-xs font-mono font-bold text-gray-400 bg-gray-100 px-3 py-1 rounded-full"
        >
          {{ ticket?.publicId || `Zgłoszenie #${route.params.id}` }}
        </div>
      </div>
    </nav>

    <main class="max-w-5xl mx-auto px-4 md:px-6 mt-8">
      <div v-if="loading" class="flex flex-col items-center justify-center py-20">
        <div class="animate-spin rounded-full h-10 w-10 border-b-2 border-[#3B71A3]"></div>
      </div>

      <div v-else-if="ticket" class="space-y-6">
        <div class="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
          <div class="p-6 md:p-8">
            <div class="flex flex-col md:flex-row md:items-center justify-between gap-4">
              <h1
                class="text-xl md:text-2xl font-semibold text-gray-800 tracking-tight leading-tight"
              >
                {{ ticket.title }}
              </h1>

              <div v-if="canChangeStatus" class="relative">
                <select
                  v-model="selectedStatusId"
                  @change="updateStatus"
                  :class="[
                    getStatusColor(ticket.status),
                    'appearance-none cursor-pointer px-4 py-1.5 pr-8 rounded-full text-[11px] font-bold uppercase tracking-wider border h-fit flex items-center justify-center min-w-[100px] self-start md:self-center focus:outline-none focus:ring-2 focus:ring-[#3B71A3]/50 transition-shadow',
                  ]"
                >
                  <option
                    v-for="statusOption in availableStatuses"
                    :key="statusOption.id"
                    :value="statusOption.id"
                    :disabled="isTechnicianOnly && statusOption.id !== 3 && statusOption.id !== 4"
                  >
                    {{ statusOption.name }}
                  </option>
                </select>
                <div
                  class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-2 text-current opacity-70"
                >
                  <svg
                    class="fill-current h-4 w-4"
                    xmlns="http://www.w3.org/2000/svg"
                    viewBox="0 0 20 20"
                  >
                    <path
                      d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z"
                    />
                  </svg>
                </div>
              </div>

              <div
                v-else
                :class="[
                  getStatusColor(ticket.status),
                  'px-4 py-1.5 rounded-full text-[11px] font-bold uppercase tracking-wider border h-fit flex items-center justify-center min-w-[100px] self-start md:self-center cursor-default',
                ]"
              >
                {{ ticket.status }}
              </div>
            </div>

            <div
              v-if="canChangeStatus"
              class="mt-4 grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-3"
            >
              <div class="flex flex-col gap-1">
                <label class="text-[10px] font-bold text-gray-400 uppercase">Klient</label>
                <select
                  v-model="selectedClientId"
                  @change="updateClient"
                  class="border border-gray-200 rounded-lg px-3 py-2 text-sm"
                >
                  <option :value="null">Brak klienta</option>
                  <option
                    v-for="clientOption in availableClients"
                    :key="clientOption.id"
                    :value="clientOption.id"
                  >
                    {{ clientOption.name }}
                  </option>
                </select>
              </div>
              <div class="flex flex-col gap-1">
                <label class="text-[10px] font-bold text-gray-400 uppercase">Priorytet</label>
                <select
                  v-model="selectedPriorityId"
                  @change="updatePriority"
                  class="border border-gray-200 rounded-lg px-3 py-2 text-sm"
                >
                  <option
                    v-for="priorityOption in availablePriorities"
                    :key="priorityOption.id"
                    :value="priorityOption.id"
                  >
                    {{ priorityOption.name }}
                  </option>
                </select>
              </div>
              <div class="flex flex-col gap-1">
                <label class="text-[10px] font-bold text-gray-400 uppercase">Kategoria</label>
                <select
                  v-model="selectedCategoryId"
                  @change="updateCategory"
                  class="border border-gray-200 rounded-lg px-3 py-2 text-sm"
                >
                  <option
                    v-for="categoryOption in filteredCategories"
                    :key="categoryOption.id"
                    :value="categoryOption.id"
                  >
                    {{ categoryOption.name }}
                  </option>
                </select>
              </div>
              <div v-if="canChangeQueue" class="flex flex-col gap-1">
                <label class="text-[10px] font-bold text-gray-400 uppercase">Kolejka</label>
                <select
                  v-model="selectedQueueId"
                  @change="updateQueue"
                  class="border border-gray-200 rounded-lg px-3 py-2 text-sm"
                >
                  <option
                    v-for="queueOption in availableQueues"
                    :key="queueOption.id"
                    :value="queueOption.id"
                  >
                    {{ queueOption.name }}
                  </option>
                </select>
              </div>
            </div>

            <div class="grid grid-cols-2 gap-4 md:hidden pt-4 mt-6 border-t border-gray-100">
              <div>
                <p class="text-[10px] font-bold text-gray-400 uppercase">Priorytet</p>
                <p class="text-sm font-bold text-gray-700">{{ ticket.priority }}</p>
              </div>
              <div>
                <p class="text-[10px] font-bold text-gray-400 uppercase">Kategoria</p>
                <p class="text-sm font-bold text-gray-700">{{ ticket.category }}</p>
              </div>
            </div>
          </div>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div class="lg:col-span-2 order-2 lg:order-1 space-y-6">
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6 md:p-8">
              <h3
                class="text-[11px] font-black uppercase text-[#3B71A3] tracking-[0.2em] mb-4 flex items-center gap-2"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="16"
                  height="16"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="3"
                >
                  <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" />
                </svg>
                Treść zgłoszenia
              </h3>
              <div
                class="text-gray-700 leading-relaxed whitespace-pre-wrap text-base md:text-lg italic font-medium break-all"
              >
                "{{ ticket.description }}"
              </div>
            </div>

            <div
              class="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden flex flex-col"
            >
              <div
                class="p-4 border-b border-gray-100 bg-gray-50/50 flex items-center justify-between"
              >
                <h3
                  class="text-[11px] font-black uppercase text-gray-500 tracking-[0.2em] flex items-center gap-2"
                >
                  Konwersacja
                </h3>
                <span
                  class="bg-gray-200 text-gray-600 text-[10px] px-2 py-0.5 rounded-full font-bold"
                >
                  {{ ticket.comments?.length || 0 }} wiadomości
                </span>
              </div>

              <div class="p-6 space-y-6 max-h-[500px] overflow-y-auto bg-white">
                <div
                  v-if="!ticket.comments || ticket.comments.length === 0"
                  class="text-center py-8 text-gray-400 italic text-sm"
                >
                  Brak komentarzy. Rozpocznij dyskusję poniżej.
                </div>

                <div
                  v-for="comment in ticket.comments"
                  :key="comment.id"
                  class="flex flex-col gap-1"
                >
                  <div class="flex items-center gap-2 px-1">
                    <span class="text-[11px] font-bold text-[#3B71A3] uppercase">
                      {{ comment.userName }}
                    </span>

                    <span
                      v-if="comment.userRole"
                      :class="[
                        'text-[9px] px-1.5 py-0.5 rounded font-bold uppercase tracking-wider',
                        getRoleBadgeColor(comment.userRole),
                      ]"
                    >
                      {{ comment.userRole }}
                    </span>

                    <span class="text-[10px] text-gray-400 ml-auto">
                      {{ formatDate(comment.createdAt) }}
                    </span>
                  </div>

                  <div
                    class="bg-gray-50 rounded-2xl rounded-tl-none p-4 text-sm text-gray-700 border border-gray-200 max-w-[90%] md:max-w-[85%] shadow-sm"
                  >
                    <div class="whitespace-pre-wrap break-words">{{ comment.content }}</div>

                    <div
                      v-if="comment.attachments && comment.attachments.length > 0"
                      class="mt-3 pt-3 border-t border-gray-200 space-y-2"
                    >
                      <div
                        v-for="file in comment.attachments"
                        :key="file.id"
                        class="flex items-center gap-2 bg-white p-2 rounded-lg border border-gray-200 text-xs"
                      >
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          width="14"
                          height="14"
                          viewBox="0 0 24 24"
                          fill="none"
                          stroke="#3B71A3"
                          stroke-width="2"
                        >
                          <path d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z" />
                          <polyline points="13 2 13 9 20 9" />
                        </svg>
                        <a
                          :href="`https://localhost:7054${file.filePath}`"
                          target="_blank"
                          class="text-[#3B71A3] font-bold hover:underline truncate max-w-[200px]"
                        >
                          {{ file.fileName }}
                        </a>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <div class="p-4 bg-gray-50 border-t border-gray-100">
                <div v-if="selectedFiles.length > 0" class="flex flex-wrap gap-2 mb-3">
                  <div
                    v-for="(file, index) in selectedFiles"
                    :key="index"
                    class="bg-white border border-gray-200 px-3 py-1.5 rounded-lg text-xs flex items-center gap-2 shadow-sm"
                  >
                    <span class="truncate max-w-[150px] font-medium text-[#3B71A3]">{{
                      file.name
                    }}</span>
                    <span class="text-gray-400">({{ (file.size / 1024).toFixed(1) }} KB)</span>
                    <button
                      @click="removeFile(index)"
                      class="text-gray-400 hover:text-red-500 font-bold ml-1 transition-colors"
                    >
                      ×
                    </button>
                  </div>
                </div>

                <div class="flex gap-2">
                  <input
                    type="file"
                    ref="fileInput"
                    multiple
                    class="hidden"
                    @change="handleFileChange"
                  />
                  <button
                    @click="$refs.fileInput.click()"
                    class="p-2.5 text-gray-400 hover:text-[#3B71A3] hover:bg-white rounded-xl transition-all border border-transparent hover:border-gray-200"
                    title="Dodaj załączniki"
                  >
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      width="20"
                      height="20"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      stroke-width="2.5"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                    >
                      <path
                        d="m21.44 11.05-9.19 9.19a6 6 0 0 1-8.49-8.49l8.57-8.57A4 4 0 1 1 18 8.84l-8.59 8.51a2 2 0 0 1-2.83-2.83l8.49-8.48"
                      />
                    </svg>
                  </button>

                  <input
                    v-model="newComment"
                    @keyup.enter="sendComment"
                    type="text"
                    placeholder="Wpisz treść komentarza..."
                    class="flex-1 bg-white border border-gray-200 rounded-xl px-4 py-2 text-sm focus:ring-2 focus:ring-[#3B71A3] focus:border-transparent outline-none transition-all"
                  />

                  <button
                    @click="sendComment"
                    :disabled="(!newComment.trim() && selectedFiles.length === 0) || sending"
                    class="bg-[#3B71A3] text-white p-2.5 rounded-xl hover:bg-[#2D567D] disabled:opacity-50 transition-all shadow-sm active:scale-95 flex items-center justify-center min-w-[44px]"
                  >
                    <svg
                      v-if="!sending"
                      xmlns="http://www.w3.org/2000/svg"
                      width="20"
                      height="20"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      stroke-width="2.5"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                    >
                      <line x1="22" y1="2" x2="11" y2="13" />
                      <polygon points="22 2 15 22 11 13 2 9 22 2" />
                    </svg>
                    <div
                      v-else
                      class="w-5 h-5 border-2 border-white border-t-transparent animate-spin rounded-full"
                    ></div>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <div class="order-1 lg:order-2">
            <div
              class="bg-white rounded-2xl shadow-sm border border-gray-200 divide-y divide-gray-100"
            >
              <div class="p-6">
                <h3 class="text-[11px] font-black uppercase text-gray-400 tracking-[0.2em] mb-6">
                  Informacje
                </h3>

                <div class="space-y-6">
                  <div class="flex items-start gap-4">
                    <div class="p-2.5 bg-blue-50 text-[#3B71A3] rounded-xl">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="20"
                        height="20"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                      >
                        <path d="M6 9H4.5a2.5 2.5 0 0 1 0-5H6" />
                        <path d="M18 9h1.5a2.5 2.5 0 0 0 0-5H18" />
                        <path d="M4 22h16" />
                        <path d="M10 14.66V17c0 .55-.47.98-.97 1.21C7.85 18.75 7 20.24 7 22" />
                        <path d="M14 14.66V17c0 .55.47.98.97 1.21C16.15 18.75 17 20.24 17 22" />
                        <path d="M18 2H6v7a6 6 0 0 0 12 0V2Z" />
                      </svg>
                    </div>
                    <div>
                      <p class="text-[10px] font-bold text-gray-400 uppercase">Priorytet</p>
                      <p class="font-bold text-gray-800">{{ ticket.priority }}</p>
                    </div>
                  </div>

                  <div class="flex items-start gap-4">
                    <div class="p-2.5 bg-purple-50 text-purple-600 rounded-xl">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="20"
                        height="20"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                      >
                        <path d="m20 7-8-4-8 4m16 0-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                      </svg>
                    </div>
                    <div>
                      <p class="text-[10px] font-bold text-gray-400 uppercase">Kategoria</p>
                      <p class="font-bold text-gray-800">{{ ticket.category }}</p>
                    </div>
                  </div>

                  <div class="flex items-start gap-4">
                    <div class="p-2.5 bg-emerald-50 text-emerald-600 rounded-xl">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="20"
                        height="20"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                      >
                        <circle cx="12" cy="12" r="10" />
                        <path d="M12 6v6l4 2" />
                      </svg>
                    </div>
                    <div>
                      <p class="text-[10px] font-bold text-gray-400 uppercase">Data zgłoszenia</p>
                      <p class="font-bold text-gray-800">{{ formatDate(ticket.createdAt) }}</p>
                    </div>
                  </div>

                  <div class="flex items-start gap-4">
                    <div class="p-2.5 bg-sky-50 text-sky-600 rounded-xl">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="20"
                        height="20"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                      >
                        <path d="M20 21a8 8 0 0 0-16 0" />
                        <circle cx="12" cy="7" r="4" />
                      </svg>
                    </div>
                    <div>
                      <p class="text-[10px] font-bold text-gray-400 uppercase">Zgłaszający</p>
                      <p class="font-bold text-gray-800">{{ ticket.creatorName || '—' }}</p>
                    </div>
                  </div>

                  <div class="flex items-start gap-4">
                    <div class="p-2.5 bg-sky-50 text-sky-600 rounded-xl">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="20"
                        height="20"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                      >
                        <rect x="2" y="4" width="20" height="16" rx="2" />
                        <path d="m22 7-10 7L2 7" />
                      </svg>
                    </div>
                    <div>
                      <p class="text-[10px] font-bold text-gray-400 uppercase">Email zgłaszającego</p>
                      <p class="font-bold text-gray-800">{{ ticket.creatorEmail || 'Brak danych' }}</p>
                    </div>
                  </div>

                  <div class="flex items-start gap-4">
                    <div class="p-2.5 bg-sky-50 text-sky-600 rounded-xl">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="20"
                        height="20"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                      >
                        <path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6A19.79 19.79 0 0 1 2.12 4.18 2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z" />
                      </svg>
                    </div>
                    <div>
                      <p class="text-[10px] font-bold text-gray-400 uppercase">Telefon zgłaszającego</p>
                      <p class="font-bold text-gray-800">{{ ticket.creatorPhone || 'Brak danych' }}</p>
                    </div>
                  </div>

                  <div class="flex items-start gap-4">
                    <div class="p-2.5 bg-indigo-50 text-indigo-600 rounded-xl">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="20"
                        height="20"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                      >
                        <path d="M3 21h18" />
                        <path d="M5 21V7l8-4 8 4v14" />
                        <path d="M9 9h.01" />
                        <path d="M9 13h.01" />
                        <path d="M9 17h.01" />
                      </svg>
                    </div>
                    <div>
                      <p class="text-[10px] font-bold text-gray-400 uppercase">Klient</p>
                      <p class="font-bold text-gray-800">{{ ticket.reporterClientName || 'Brak danych' }}</p>
                    </div>
                  </div>

                  <div class="flex items-start gap-4">
                    <div class="p-2.5 bg-indigo-50 text-indigo-600 rounded-xl">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="20"
                        height="20"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                      >
                        <path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6A19.79 19.79 0 0 1 2.12 4.18 2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z" />
                      </svg>
                    </div>
                    <div>
                      <p class="text-[10px] font-bold text-gray-400 uppercase">Telefon klienta</p>
                      <p class="font-bold text-gray-800">{{ ticket.reporterClientPhone || 'Brak danych' }}</p>
                    </div>
                  </div>

                  <div class="flex items-start gap-4">
                    <div class="p-2.5 bg-amber-50 text-amber-600 rounded-xl">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="20"
                        height="20"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                      >
                        <circle cx="12" cy="12" r="10" />
                        <path d="M12 6v6l4 2" />
                      </svg>
                    </div>
                    <div>
                      <p class="text-[10px] font-bold text-gray-400 uppercase">SLA do</p>
                      <p class="font-bold text-gray-800">
                        {{ formatDate(ticket.dueAtUtc ?? ticket.DueAtUtc) }}
                      </p>
                      <p
                        :class="getSlaTextClass(ticket.slaState ?? ticket.SlaState)"
                        class="text-xs font-semibold mt-1"
                      >
                        {{
                          formatSlaRemaining(
                            ticket.remainingMinutes ?? ticket.RemainingMinutes,
                            ticket.slaState ?? ticket.SlaState,
                          )
                        }}
                      </p>
                      <p
                        v-if="shouldShowSlaMessage(ticket.slaMessage ?? ticket.SlaMessage)"
                        :class="getSlaTextClass(ticket.slaState ?? ticket.SlaState)"
                        class="text-[11px] font-medium mt-1"
                      >
                        {{ ticket.slaMessage ?? ticket.SlaMessage }}
                      </p>
                    </div>
                  </div>
                </div>
              </div>

              <div class="p-6 bg-gray-50/50">
                <p class="text-[10px] font-bold text-gray-400 uppercase mb-3">Przypisana kolejka</p>
                <div class="flex items-center gap-2 font-mono text-sm font-bold text-[#3B71A3]">
                  <span class="w-2 h-2 rounded-full bg-[#3B71A3] animate-pulse"></span>
                  {{ ticket.queue }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'
import { useUserStore } from '@/stores/user'

const route = useRoute()
const router = useRouter()
const ticket = ref(null)
const loading = ref(true)

const newComment = ref('')
const selectedFiles = ref([])
const sending = ref(false)
const fileInput = ref(null)
const availableStatuses = ref([])
const availablePriorities = ref([])
const availableCategories = ref([])
const availableQueues = ref([])
const availableClients = ref([])

const selectedStatusId = ref(null)
const selectedPriorityId = ref(null)
const selectedCategoryId = ref(null)
const selectedQueueId = ref(null)
const selectedClientId = ref(null)

const filteredCategories = computed(() => {
  if (!selectedClientId.value) {
    return availableCategories.value.filter(c => c.clientId === null)
  }
  return availableCategories.value.filter(c => c.clientId === selectedClientId.value || c.clientId === null)
})

const userStore = useUserStore()

const isTechnicianOnly = computed(() => {
  const userRoles = userStore.user?.roles
  if (!userRoles) return false

  let rolesArray = []
  if (Array.isArray(userRoles)) {
    rolesArray = userRoles
  } else if (typeof userRoles === 'string') {
    rolesArray = userRoles.split(',').map((r) => r.trim())
  }

  const isAdminOrHelpdesk = rolesArray.some((role) => role === 'Admin' || role === 'Helpdesk')

  // Zwraca true, jeśli ma rolę Technik, ale NIE MA roli Admin lub Helpdesk
  return rolesArray.includes('Technik') && !isAdminOrHelpdesk
})

const canChangeStatus = computed(() => {
  if (!userStore.user || !userStore.user.roles) return false

  const allowedRoles = ['Admin', 'Helpdesk', 'Technik']

  if (Array.isArray(userStore.user.roles)) {
    return userStore.user.roles.some((role) => allowedRoles.includes(role))
  }

  if (typeof userStore.user.roles === 'string') {
    const userRolesArray = userStore.user.roles.split(',').map((r) => r.trim())
    return userRolesArray.some((role) => allowedRoles.includes(role))
  }

  return false
})

const canChangeQueue = computed(() => {
  if (!userStore.user || !userStore.user.roles) return false

  const allowedRoles = ['Admin', 'Helpdesk']

  if (Array.isArray(userStore.user.roles)) {
    return userStore.user.roles.some((role) => allowedRoles.includes(role))
  }

  if (typeof userStore.user.roles === 'string') {
    const userRolesArray = userStore.user.roles.split(',').map((r) => r.trim())
    return userRolesArray.some((role) => allowedRoles.includes(role))
  }

  return false
})

const fetchTicketDetails = async () => {
  try {
    const response = await axios.get(`https://localhost:7054/api/ticket/${route.params.id}`, {
      withCredentials: true,
    })
    ticket.value = response.data

    selectedStatusId.value = response.data.statusId
    selectedPriorityId.value = response.data.priorityId
    selectedCategoryId.value = response.data.categoryId
    selectedQueueId.value = response.data.queueId
    selectedClientId.value = response.data.clientId
  } catch (error) {
    console.error('Error fetching ticket details:', error)
  } finally {
    loading.value = false
  }
}

const fetchStatuses = async () => {
  try {
    const response = await axios.get('https://localhost:7054/api/ticket/statuses', {
      withCredentials: true,
    })
    availableStatuses.value = response.data
  } catch (error) {
    console.error('Błąd podczas pobierania statusów:', error)
  }
}

const fetchPriorities = async () => {
  try {
    const response = await axios.get('https://localhost:7054/api/admin/priorities', {
      withCredentials: true,
    })
    availablePriorities.value = response.data
  } catch (error) {
    console.error('Błąd podczas pobierania priorytetów:', error)
  }
}

const fetchCategories = async () => {
  try {
    const response = await axios.get('https://localhost:7054/api/admin/categories-all', {
      withCredentials: true,
    })
    availableCategories.value = response.data
  } catch (error) {
    console.error('Błąd podczas pobierania kategorii:', error)
  }
}

const fetchClients = async () => {
  try {
    const response = await axios.get('https://localhost:7054/api/admin/clients', {
      withCredentials: true,
    })
    availableClients.value = response.data
  } catch (error) {
    console.error('Błąd podczas pobierania klientów:', error)
  }
}

const fetchQueues = async () => {
  try {
    const response = await axios.get('https://localhost:7054/api/admin/queues', {
      withCredentials: true,
    })
    availableQueues.value = response.data
  } catch (error) {
    console.error('Błąd podczas pobierania kolejek:', error)
  }
}

const updateStatus = async () => {
  if (selectedStatusId.value == null) return
  try {
    await axios.patch(
      `https://localhost:7054/api/ticket/${route.params.id}/status`,
      { newStatusId: selectedStatusId.value },
      {
        withCredentials: true,
        headers: { 'Content-Type': 'application/json' },
      },
    )
    await fetchTicketDetails()
    console.log('Status zaktualizowany pomyślnie!')
  } catch (error) {
    console.error('Błąd podczas zmiany statusu:', error)
    alert('Nie udało się zmienić statusu.')
    await fetchTicketDetails()
  }
}

const updatePriority = async () => {
  if (selectedPriorityId.value == null) return
  try {
    await axios.patch(
      `https://localhost:7054/api/ticket/${route.params.id}/priority`,
      { newPriorityId: selectedPriorityId.value },
      {
        withCredentials: true,
        headers: { 'Content-Type': 'application/json' },
      },
    )
    await fetchTicketDetails()
    console.log('Priorytet zaktualizowany pomyślnie!')
  } catch (error) {
    console.error('Błąd podczas zmiany priorytetu:', error)
    alert('Nie udało się zmienić priorytetu.')
    await fetchTicketDetails()
  }
}

const updateCategory = async () => {
  try {
    await axios.patch(`https://localhost:7054/api/ticket/${route.params.id}/category`, 
      { newCategoryId: selectedCategoryId.value }, 
      {
        withCredentials: true,
        headers: {
          'Content-Type': 'application/json'
        }
      }
    )
    console.log('Kategoria zaktualizowana')
    await fetchTicketDetails()
  } catch (error) {
    console.error('Błąd podczas zmiany kategorii:', error)
    alert('Błąd uruchamiania backendu zmiany kategorii.')
    await fetchTicketDetails()
  }
}

const updateClient = async () => {
  try {
    await axios.patch(`https://localhost:7054/api/ticket/${route.params.id}/client`, 
      { newClientId: selectedClientId.value }, 
      {
        withCredentials: true,
        headers: {
          'Content-Type': 'application/json'
        }
      }
    )
    console.log('Klient zaktualizowany')
    await fetchTicketDetails()
  } catch (error) {
    console.error('Błąd podczas zmiany klienta:', error)
    alert('Błąd podczas aktualizacji klienta')
    await fetchTicketDetails()
  }
}

const updateQueue = async () => {
  if (!canChangeQueue.value) return
  if (selectedQueueId.value == null) return
  try {
    await axios.patch(
      `https://localhost:7054/api/ticket/${route.params.id}/queue`,
      { newQueueId: selectedQueueId.value },
      {
        withCredentials: true,
        headers: { 'Content-Type': 'application/json' },
      },
    )
    await fetchTicketDetails()
    console.log('Kolejka zaktualizowana pomyślnie!')
  } catch (error) {
    console.error('Błąd podczas zmiany kolejki:', error)
    alert('Nie udało się zmienić kolejki.')
    await fetchTicketDetails()
  }
}

const handleFileChange = (event) => {
  const files = Array.from(event.target.files)
  const validFiles = files.filter((file) => {
    if (file.size > 10 * 1024 * 1024) {
      alert(`Plik ${file.name} jest za duży. Maksymalnie 10MB.`)
      return false
    }
    return true
  })
  selectedFiles.value.push(...validFiles)
}

const removeFile = (index) => {
  selectedFiles.value.splice(index, 1)
  if (selectedFiles.value.length === 0 && fileInput.value) {
    fileInput.value.value = ''
  }
}

const sendComment = async () => {
  if ((!newComment.value.trim() && selectedFiles.value.length === 0) || sending.value) return

  sending.value = true
  const formData = new FormData()
  formData.append('content', newComment.value)
  selectedFiles.value.forEach((file) => {
    formData.append('files', file)
  })

  try {
    await axios.post(`https://localhost:7054/api/ticket/${route.params.id}/comment`, formData, {
      withCredentials: true,
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
    newComment.value = ''
    selectedFiles.value = []
    if (fileInput.value) fileInput.value.value = ''
    await fetchTicketDetails()
  } catch (error) {
    console.error('Error adding comment:', error)
    alert('Nie udało się dodać komentarza. Sprawdź konsolę.')
  } finally {
    sending.value = false
  }
}

const getRoleBadgeColor = (roleString) => {
  if (!roleString) return 'bg-gray-100 text-gray-500 border border-gray-200'
  const roles = roleString.toLowerCase()
  if (roles.includes('admin')) return 'bg-red-100 text-red-600 border border-red-200'
  if (roles.includes('helpdesk')) return 'bg-blue-100 text-blue-600 border border-blue-200'
  if (roles.includes('technik')) return 'bg-amber-100 text-amber-600 border border-amber-200'
  return 'bg-gray-100 text-gray-500 border border-gray-200'
}

const parseUtcDate = (value) => {
  if (!value) return null
  if (value instanceof Date) return value
  if (typeof value !== 'string') return new Date(value)

  const hasTimezone = /[zZ]$|[+-]\d{2}:\d{2}$/.test(value)
  const normalized = hasTimezone ? value : `${value}Z`
  return new Date(normalized)
}

const formatDate = (date) => {
  if (!date) return ''
  const parsedDate = parseUtcDate(date)
  if (!parsedDate || Number.isNaN(parsedDate.getTime())) return ''

  return parsedDate.toLocaleString('pl-PL', {
    timeZone: 'Europe/Warsaw',
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

const formatSlaRemaining = (minutes, slaState) => {
  if (slaState === 'breached') return 'SLA przekroczone'
  if (slaState === 'paused') return 'SLA wstrzymane'
  if (typeof minutes !== 'number') return 'Brak danych SLA'

  const hours = Math.floor(minutes / 60)
  const mins = Math.abs(minutes % 60)
  if (hours <= 0) return `Pozostało ${Math.max(minutes, 0)} min`
  return `Pozostało ${hours}h ${mins}m`
}

const getSlaTextClass = (slaState) => {
  if (slaState === 'breached') return 'text-red-600'
  if (slaState === 'critical') return 'text-red-500'
  if (slaState === 'warning') return 'text-amber-600'
  if (slaState === 'paused') return 'text-slate-600'
  return 'text-emerald-600'
}

const shouldShowSlaMessage = (message) => {
  if (!message) return false
  return message.trim() === 'Zgłoszenie rozwiązane po SLA'
}

const getStatusColor = (status) => {
  if (status === 'Nowy') return 'bg-green-50 text-green-600 border-green-100'
  if (status === 'W toku') return 'bg-blue-50 text-blue-600 border-blue-100'
  if (status === 'Rozwiązane' || status === 'Zamknięte' || status === 'Wykonane')
    return 'bg-gray-50 text-gray-600 border-gray-100'
  return 'bg-yellow-50 text-yellow-600 border-yellow-100'
}

onMounted(async () => {
  await fetchTicketDetails()

  if (!userStore.isSessionChecked) {
    await userStore.fetchCurrentUser()
  }

  if (canChangeStatus.value) {
    await Promise.all([fetchStatuses(), fetchPriorities(), fetchCategories(), fetchClients()])
  }

  if (canChangeQueue.value) {
    await fetchQueues()
  }
})
</script>
