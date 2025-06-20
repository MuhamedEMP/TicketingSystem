<template>
  <UserNavbar />
  <div class="overlay"></div>

  <div class="ticket-page">
    <h1 class="main-title">My Tickets</h1>

    <div class="content-container">
      <div class="filter-row">
        <select v-model="filters.departmentName" class="filter-select">
          <option value="">Department</option>
          <option v-for="dept in uniqueDepartments" :key="dept">{{ dept }}</option>
        </select>

        <select v-model="filters.categoryName" class="filter-select">
          <option value="">Issue type</option>
          <option v-for="cat in uniqueCategories" :key="cat">{{ cat }}</option>
        </select>

        <select v-model="filters.assignedToName" class="filter-select">
          <option value="">Assigned person</option>
          <option v-for="name in uniqueAssignees" :key="name">{{ name }}</option>
        </select>

        <select v-model="filters.status" class="filter-select">
          <option value="">Status</option>
          <option value="Pending">Pending</option>
          <option value="Resolved">Resolved</option>
          <option value="Closed">Closed</option>
        </select>

        <button class="filter-button" @click="fetchTickets">Filter tickets</button>
      </div>

      <div v-if="noResults && tickets.length === 0" class="no-tickets">
        No tickets found.
      </div>

      <div v-else class="ticket-grid">
        <div v-for="ticket in tickets" :key="ticket.id" class="ticket-card">
          <router-link :to="`/user/tickets/${ticket.id}`" class="ticket-link">
            <div class="ticket-top">
              <div>
                <h3 class="ticket-title">{{ ticket.title }}</h3>
                <span class="ticket-tag">{{ ticket.categoryName }}</span>
              </div>
              <span class="ticket-time">{{ new Date(ticket.createdAt).toLocaleDateString() }}</span>
            </div>

            <div class="ticket-meta aligned-bottom">
              <div class="meta-row">
                <img class="avatar" src="../assets/user-icon.png" alt="avatar" />
                <span>{{ ticket.assignedToName || 'Unassigned' }}</span>
              </div>
              <div class="ticket-status">{{ ticket.status }}</div>
            </div>
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { filterMyTickets } from '../api/userApi'
import UserNavbar from '../components/UserNavbar.vue'

const tickets = ref([])
const noResults = ref(false)
const filters = ref({
  status: '',
  urgency: '',
  assignedToName: '',
  categoryName: '',
  departmentName: '',
  fromDate: '',
  toDate: '',
  search: '',
  hasResponses: ''
})

const fetchTickets = async () => {
  noResults.value = false
  const params = new URLSearchParams()

  for (const key in filters.value) {
    if (filters.value[key]) {
      params.append(key, filters.value[key])
    }
  }

  try {
    const { tickets: filteredTickets } = await filterMyTickets(params)
    tickets.value = filteredTickets
    noResults.value = filteredTickets.length === 0
  } catch (error) {
    console.error('Error fetching tickets:', error)
    tickets.value = []
    noResults.value = true
  }
}

const uniqueDepartments = computed(() =>
  [...new Set(tickets.value.map(t => t.departmentName))]
)
const uniqueCategories = computed(() =>
  [...new Set(tickets.value.map(t => t.categoryName))]
)
const uniqueAssignees = computed(() =>
  [...new Set(tickets.value.map(t => t.assignedToName).filter(Boolean))]
)

onMounted(fetchTickets)
</script>

<style scoped>
.ticket-page {
  padding: 2rem 1rem;
  min-height: 100vh;
  font-family: 'Poppins', sans-serif;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.main-title {
  font-size: 2rem;
  font-weight: bold;
  color: white;
  margin-bottom: 2rem;
}

/* Shared container for filters and cards */
.content-container {
  max-width: 1140px;
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding-left: 1.2rem; /* subtle left shift */
}

/* Filter bar */
.filter-row {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.filter-select {
  width: 220px;
  padding: 10px 14px;
  background-color: white;
  color: black;
  border-radius: 20px;
  border: none;
  font-size: 14px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

.filter-button {
  padding: 10px 18px;
  background-color: #ca0176;
  color: white;
  border: none;
  border-radius: 20px;
  font-size: 14px;
  font-weight: bold;
  cursor: pointer;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.2);
  transition: background-color 0.2s ease;
}

.filter-button:hover {
  background-color: white;
  color: #ca0176;
}

/* Tickets grid */
.ticket-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 2rem;
  width: 110%;
  box-sizing: border-box;
  justify-items: center;
}

.ticket-card {
  width: 240px;
  height: 260px;
  background-color: white;
  border-radius: 16px;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
  transition: transform 0.2s ease;
}

.ticket-card:hover {
  transform: translateY(-5px);
}

.ticket-link {
  text-decoration: none;
  color: black;
  display: flex;
  flex-direction: column;
  height: 100%;
  justify-content: space-between;
}

.ticket-top {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
}

.ticket-title {
  font-weight: bold;
  font-size: 1rem;
  margin: 0 0 0.25rem 0;
}

.ticket-tag {
  display: inline-block;
  background-color: #d9f3ed;
  color: #2c7a7b;
  font-size: 0.75rem;
  font-weight: 500;
  padding: 4px 12px;
  border-radius: 20px;
  width: fit-content;
}

.ticket-time {
  font-size: 0.8rem;
  color: #888;
}

.ticket-meta.aligned-bottom {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 0.4rem;
}

.meta-row {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.avatar {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  object-fit: cover;
}

.ticket-status {
  font-weight: 600;
  margin-left: 2rem;
}

.no-tickets {
  text-align: center;
  color: #f88;
  font-weight: bold;
  margin-top: 2rem;
}

/* Responsive grid */
@media (max-width: 1200px) {
  .ticket-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}

@media (max-width: 900px) {
  .ticket-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .filter-row {
    flex-direction: column;
    align-items: center;
  }
}

@media (max-width: 600px) {
  .ticket-grid {
    grid-template-columns: 1fr;
  }
}
</style>
