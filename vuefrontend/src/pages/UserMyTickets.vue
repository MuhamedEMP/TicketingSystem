<template>
  <UserNavbar />
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

          <input v-model="filters.assignedToName" placeholder="Assigned To" />

          <input v-model="filters.categoryName" placeholder="Category Name" />
          <input v-model="filters.departmentName" placeholder="Department Name" />

          <label>From:</label>
          <input type="date" v-model="filters.fromDate" />
          <label>To:</label>
          <input type="date" v-model="filters.toDate" />

          <button @click="fetchTickets">Apply Filters</button>
    </div>

    <div v-if="noResults && tickets.length === 0" class="no-tickets">
      No tickets found.
    </div>

    <div v-else>
      <div v-for="ticket in tickets" :key="ticket.id" class="ticket-card">
        <router-link :to="`/user/tickets/${ticket.id}`" class="ticket-title-link">
          <h2>{{ ticket.title }}</h2>
        </router-link>
        <p>{{ ticket.description }}</p>
        <div class="ticket-meta">
          <span>Status: {{ ticket.status }}</span>
          <span>Urgency: {{ ticket.urgency }}</span>
          <span>Category: {{ ticket.categoryName }}</span>
          <span>Department: {{ ticket.departmentName }}</span>
          <span>Submitted: {{ new Date(ticket.createdAt).toLocaleString() }}</span>
          <span>
            Updated: 
            {{ new Date(ticket.updatedAt).getFullYear() > 2000 
                ? new Date(ticket.updatedAt).toLocaleString() 
                : 'No Updates' }}
          </span>

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
const noResults = ref(false);
const filters = ref({
  status: '',
  urgency: '',
  assignedToName: '',
  categoryName: '',
  departmentName: '',
  fromDate: '',
  toDate: '',
  search: '',
});

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
