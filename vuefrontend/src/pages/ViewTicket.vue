<!-- src/pages/ViewTicket.vue -->
<template>
    <div class="ticket-view-page">
      <div v-if="ticket" class="ticket-card">
        <h1>{{ ticket.title }}</h1>
        <p class="ticket-description">{{ ticket.description }}</p>
        <div class="ticket-meta">
          <span><strong>Status:</strong> {{ ticket.status }}</span>
          <span><strong>Urgency:</strong> {{ ticket.urgency }}</span>
          <span><strong>Category:</strong> {{ ticket.categoryName }}</span>
          <span><strong>Department:</strong> {{ ticket.departmentName }}</span>
          <span><strong>Submitted:</strong> {{ new Date(ticket.createdAt).toLocaleString() }}</span>
          <span><strong>Updated:</strong> {{ new Date(ticket.updatedAt).toLocaleString() }}</span>
        </div>
      </div>
      <div v-else-if="error" class="error-message">{{ error }}</div>
      <div v-else class="ticket-meta">Loading ticket...</div>
    </div>
  </template>
  
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import { useRoute } from 'vue-router';
  import { getMyTicketById } from '../api/userApi';
  
  const route = useRoute();
  const ticket = ref(null);
  const error = ref(null);
  
  const fetchTicket = async () => {
    try {
      const id = route.params.id;
      ticket.value = await getMyTicketById(id);
    } catch (err) {
      error.value = err.message || 'Failed to load ticket.';
    }
  };
  
  onMounted(fetchTicket);
  </script>
  
  <style src="../assets/css/viewticket.css"></style>
  