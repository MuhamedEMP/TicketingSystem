<template>
  <UserNavbar @select-department="selectDepartment" />
  <div class="overlay"></div>
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
  import { filterMyTickets } from '../api/userApi';
  import UserNavbar from '../components/UserNavbar.vue';
  
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
  
    try {
    const { tickets: filteredTickets } = await filterMyTickets(params);
    tickets.value = filteredTickets;
    noResults.value = filteredTickets.length === 0;
  } catch (error) {
    console.error('Error fetching tickets:', error);
    tickets.value = [];
    noResults.value = true;
  }
  };
  
  onMounted(fetchTickets);
  </script>
  <style src="../assets/css/tickets.css"></style>