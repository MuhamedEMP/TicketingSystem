<template>
    <div class="ticket-page">
      <div class="filter-bar">
        <input v-model="filters.search" placeholder="Search..." />
        <select v-model="filters.status">
          <option value="">Status</option>
          <option value="Pending">Pending</option>
          <option value="Resolved">Resolved</option>
          <option value="Closed">Closed</option>
        </select>
        <select v-model="filters.urgency">
          <option value="">Urgency</option>
          <option value="Low">Low</option>
          <option value="Medium">Medium</option>
          <option value="High">High</option>
        </select>
        <button @click="fetchTickets">Apply Filters</button>
      </div>
  
      <!-- Show when no tickets are found -->
      <div v-if="noResults && tickets.length === 0" class="no-tickets">
        No tickets found.
      </div>
  
      <!-- Show all tickets -->
      <div v-else>
        <div
          v-for="ticket in tickets"
          :key="ticket.id"
          class="ticket-card"
        >
          <h2>{{ ticket.title }}</h2>
          <p>{{ ticket.description }}</p>
          <div class="ticket-meta">
            <span>Status: {{ ticket.status }}</span>
            <span>Urgency: {{ ticket.urgency }}</span>
            <span>Category: {{ ticket.categoryName }}</span>
            <span>Department: {{ ticket.departmentName }}</span>
            <span>Submitted: {{ new Date(ticket.createdAt).toLocaleString() }}</span>
            <span>Updated: {{ new Date(ticket.updatedAt).toLocaleString() }}</span>
          </div>
        </div>
      </div>
    </div>
  </template>
  
  
  <script setup>
  import { ref, onMounted } from 'vue';
  
  const tickets = ref([]);
  const filters = ref({
    status: '',
    urgency: '',
    assignedToId: '',
    categoryId: '',
    departmentId: '',
    fromDate: '',
    toDate: '',
    search: '',
  });
  
  const noResults = ref(false);
  
  const fetchTickets = async () => {
    noResults.value = false;
    const params = new URLSearchParams();
  
    for (const key in filters.value) {
      if (filters.value[key]) {
        params.append(key, filters.value[key]);
      }
    }
  
    const token = localStorage.getItem('accessToken');
  
    try {
      const response = await fetch(`http://localhost:5172/user/filter?${params.toString()}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
  
      if (response.ok) {
        tickets.value = await response.json();
      } else if (response.status === 400) {
        tickets.value = [];
        noResults.value = true;
      } else {
        console.error('Unexpected error:', response.status);
        console.log(response.json());
      }
    } catch (err) {
      console.error('Request failed:', err);
    }
  };
  
  onMounted(fetchTickets);
  </script>
  <style src="../assets/css/tickets.css"></style>