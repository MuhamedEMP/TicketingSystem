<template>
  <UserNavbar />
  <div class="overlay"></div>

  <div class="ticket-page">
    <h1>Received Tickets</h1>

    <div class="filter-container">
      <div class="filter-bar">
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

        <input v-model="filters.departmentName" placeholder="Department Name" />
        <input type="date" v-model="filters.fromDate" />
        <input type="date" v-model="filters.toDate" />

        <button @click="fetchTickets">Apply Filters</button>
        <button @click="resetFilters" class="reset-btn">Reset</button>
      </div>
    </div>

    <div v-if="noResults && tickets.length === 0" class="no-tickets">
      No department tickets found.
    </div>

    <div v-else>
      <div v-for="ticket in tickets" :key="ticket.id" class="ticket-card">
        <router-link :to="`/sharedtickets/${ticket.id}`" class="ticket-title-link tick">
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
            <div class="responses-text">
              {{ ticket.responsesCount > 0 ? `Number of Responses: ${ticket.responsesCount}` : 'No responses' }}
            </div>
          </span>
          <span>
            Updated:
            {{
              new Date(ticket.updatedAt).getFullYear() > 2000
                ? new Date(ticket.updatedAt).toLocaleString()
                : 'No Updates'
            }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import UserNavbar from '../../components/UserNavbar.vue';
import { filterDepartmentTickets } from '../../api/sharedApi';
import { useRoute } from 'vue-router';

const route = useRoute();

const tickets = ref([]);
const noResults = ref(false);
const filters = ref({
  status: '',
  urgency: '',
  assignedToName: '',
  assignedToId: '',
  categoryName: route.query.categoryName || '',
  departmentName: route.query.departmentName || '',
  fromDate: '',
  toDate: '',
  search: '',
  isAssigned: '',
  hasResponses: ''
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
    const filteredTickets = await filterDepartmentTickets(params);
    tickets.value = filteredTickets;
    noResults.value = filteredTickets.length === 0;
  } catch (error) {
    console.error('Error fetching department tickets:', error);
    tickets.value = [];
    noResults.value = true;
  }
};

const resetFilters = () => {
  filters.value = {
    status: '',
    urgency: '',
    assignedToName: '',
    assignedToId: '',
    categoryName: '',
    departmentName: '',
    fromDate: '',
    toDate: '',
    search: '',
    isAssigned: '',
    hasResponses: ''
  };

  fetchTickets();
};

onMounted(fetchTickets);
</script>

<style src="../../assets/css/tickets.css"></style>
