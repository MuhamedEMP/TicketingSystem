<template>
    <UserNavbar />
    <div class="overlay"></div>
  
    <div class="ticket-page">
      <h1>Received Tickets</h1>
  
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

        
        <label for="">Responses</label>
            <select v-model="filters.hasResponses">
              <option value="">All</option>
            <option value="true">With Responses</option>
            <option value="false">No Responses</option>
        </select>

        <select v-model="filters.isAssigned">
        <option value="">Assignment Status</option>
        <option value="true">Assigned</option>
        <option value="false">Unassigned</option>
        </select>

  
        <input v-model="filters.assignedToName" placeholder="Assigned To" />
        <input v-model="filters.categoryName" placeholder="Category Name" />
        <input v-model="filters.departmentName" placeholder="Department Name" />
  
        <label>From:</label>
        <input type="date" v-model="filters.fromDate" />
        <label>To:</label>
        <input type="date" v-model="filters.toDate" />
  
        <button @click="fetchTickets">Apply Filters</button>
        <button @click="filterAssignedToMe" class="assigned-to-me-btn">
        Assigned to Me Only
        </button>
        <button @click="resetFilters" class="reset-filters-btn">
        Reset Filters
    </button>
      </div>



  
      <div v-if="noResults && tickets.length === 0" class="no-tickets">
        No department tickets found.
      </div>
  
      <div v-else>
        <div v-for="ticket in tickets" :key="ticket.id" class="ticket-card">
          <router-link :to="`/sharedtickets/${ticket.id}`" class="ticket-title-link">
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
  import { useRoute, useRouter } from 'vue-router';

  const route = useRoute();
  const router = useRouter();

  const filterAssignedToMe = () => {
  const userId = localStorage.getItem('userId'); // adjust if your key is different
  if (userId) {
    filters.value.assignedToId = userId;
    fetchTickets();
  }
};


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
  
  onMounted(fetchTickets);
  </script>
  
  <style src="../../assets/css/tickets.css">
.assigned-to-me-btn {
  background-color: #42b983;
  color: white;
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: bold;
  margin-top: 0.5rem;
}

.assigned-to-me-btn:hover {
  background-color: #36966e;
}

.reset-filters-btn {
  background-color: #555;
  color: white;
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: bold;
  margin-left: 0.5rem;
}

.reset-filters-btn:hover {
  background-color: #777;
}

</style>
  