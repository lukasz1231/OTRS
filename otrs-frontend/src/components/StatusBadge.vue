<template>
    <span :class="[
        'inline-flex items-center justify-center px-3 py-1 rounded-full text-[11px] font-bold uppercase tracking-wider border w-fit',
        colorClasses
    ]">
        {{ displayStatus }}
    </span>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
    status: {
        type: String,
        required: true,
        default: 'Brak'
    }
})

const displayStatus = computed(() => props.status || 'Brak')

const colorClasses = computed(() => {
    if (!props.status) return 'bg-gray-100 text-gray-500 border-gray-200'

    const normalizedStatus = props.status
        .normalize('NFD')
        .replace(/[\u0300-\u036f]/g, '')
        .toLowerCase()
        .trim()

    if (['nowy', 'nowa'].includes(normalizedStatus)) {
        return 'bg-green-100 text-green-700 border-green-200'
    }

    if (['w toku', 'w realizacji', 'w trakcie'].includes(normalizedStatus)) {
        return 'bg-blue-100 text-blue-700 border-blue-200'
    }

    if (['rozwiazane', 'zamkniete', 'zamkniety', 'wykonane'].includes(normalizedStatus)) {
        return 'bg-gray-100 text-gray-600 border-gray-200'
    }

    return 'bg-yellow-100 text-yellow-700 border-yellow-200'
})
</script>