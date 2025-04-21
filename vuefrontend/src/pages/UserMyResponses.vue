<script setup>
import { ref, onMounted } from 'vue';
import { getResponsesToMyTickets } from '../api/userApi';
import UserNavbar from '../components/UserNavbar.vue';

const responses = ref([]);
const filters = ref({
  search: '',
  status: '',
  fromDate: '',
  toDate: '',
  hasAttachments: ''
});

const applyFilters = async () => {
  try {
    responses.value = await getResponsesToMyTickets(filters.value);
  } catch (err) {
    console.error('❌ Failed to fetch filtered responses:', err);
  }
};

const formatDate = (date) => new Date(date).toLocaleString();

const getFileName = (url) => {
  try {
    return decodeURIComponent(url.split('/').pop().split('?')[0]);
  } catch {
    return "Attachment";
  }
};

const fetchResponses = async () => {
  const params = new URLSearchParams();
  for (const key in filters.value) {
    if (filters.value[key] !== '') {
      params.append(key, filters.value[key]);
    }
  }

  try {
    responses.value = await getResponsesToMyTickets(params);
  } catch (err) {
    console.error('❌ Failed to fetch responses:', err);
  }
};

onMounted(fetchResponses);
</script>

<template>
  <UserNavbar />
  <div class="overlay"></div>

  <div class="ticket-view-page">
    <h1>📬 Responses to My Tickets</h1>

    <div class="filter-bar">
      <input v-model="filters.search" placeholder="Search message..." />

      <select v-model="filters.status">
        <option value="">All Statuses</option>
        <option value="Open">Open</option>
        <option value="Closed">Closed</option>
        <option value="InProgress">InProgress</option>
        <option value="Resolved">Resolved</option>
        <option value="Reopened">Reopened</option>
        <option value="Deleted">Deleted</option>
      </select>

      <label>From:</label>
      <input type="date" v-model="filters.fromDate" />

      <label>To:</label>
      <input type="date" v-model="filters.toDate" />

      <label>Has Attachments:</label>
      <select v-model="filters.hasAttachments">
        <option value="">All</option>
        <option value="true">With Attachments</option>
        <option value="false">No Attachments</option>
      </select>

      <button @click="applyFilters">Search</button>
    </div>

    <div v-if="responses.length > 0">
      <div
        v-for="response in responses"
        :key="response.id"
        class="ticket-response"
      >
        <div class="response-header">
          <strong>Ticket #{{ response.ticketId }}</strong>
          <span class="response-time">— {{ formatDate(response.createdAt) }}</span>
        </div>

        <p class="response-message">{{ response.message }}</p>

        <div v-if="response.attachmentUrls?.length" class="response-attachments">
          <h4>📎 Attachments</h4>
          <ul>
            <li v-for="(path, index) in response.attachmentUrls" :key="index">
              <a :href="path" target="_blank">{{ getFileName(path) }}</a>
            </li>
          </ul>
        </div>

        <p class="response-status">
          <em>Status: {{ response.status }}</em>
        </p>
        <router-link :to="`/sharedtickets/${response.ticketId}`" class="response-link">
          View Ticket
        </router-link>
        <hr class="white-line" />
      </div>
    </div>

    <p v-else class="no-responses">No responses to your tickets found.</p>
  </div>
</template>

<style scoped>
.response-attachments h4 {
  margin-bottom: 0.5rem;
  color: #aaa;
}

.response-attachments a {
  color: #42b983;
  text-decoration: underline;
}

.ticket-view-page {
  padding: 2rem;
  max-width: 900px;
  margin: auto;
  color: #eee;
}

.ticket-response {
  background-color: #2e2e2e;
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 1.5rem;
  border-left: 4px solid #42b983;
}

.response-header {
  font-weight: bold;
  color: #42b983;
  margin-bottom: 0.5rem;
}

.response-time {
  font-weight: normal;
  color: #aaa;
  margin-left: 0.5rem;
}

.response-message {
  color: #ccc;
  margin-bottom: 0.5rem;
}

.response-status {
  font-style: italic;
  color: #bbb;
}

.response-link {
  color: #42b983;
  text-decoration: underline;
}

.no-responses {
  color: red;
  font-style: italic;
}

.white-line {
  border: none;
  height: 1px;
  background-color: white;
  margin-top: 1rem;
  opacity: 0.1;
}
</style>
