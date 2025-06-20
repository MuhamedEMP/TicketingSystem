<template>
  <UserNavbar></UserNavbar>
  <div class="ticket-view-page">
    <div v-if="ticket" class="ticket-card">
      <h1>{{ ticket.title }}</h1>
      <p class="ticket-description">{{ ticket.description }}</p>

      <div class="ticket-meta">
        <span><strong>Status:</strong> {{ ticket.status }}</span>
        <span><strong>Urgency:</strong> {{ ticket.urgency }}</span>
        <span><strong>Category:</strong> {{ ticket.categoryName }}</span>
        <span><strong>Department:</strong> {{ ticket.departmentName }}</span>
        <span><strong>Submitted:</strong> {{ formatDate(ticket.createdAt) }}</span>
        <span><strong>Updated:</strong> {{ formatDate(ticket.updatedAt) }}</span>
      </div>

      <div v-if="ticket.attachmentPaths?.length" class="ticket-attachments">
        <h3>📎 Attachments</h3>
        <ul>
          <li v-for="(file, index) in ticket.attachmentPaths" :key="index">
            <a :href="file" target="_blank">{{ getFileName(file) }}</a>
          </li>
        </ul>
      </div>
    </div>

    <div v-if="ticket?.responses?.length" class="response-section">
      <h2>Responses</h2>
      <div v-for="response in ticket.responses" :key="response.id" class="ticket-response">
        <div class="response-header">
          <strong>{{ response.userFullName }}</strong>
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
        <div v-else class="response-attachments">
          <p>No Attachments</p>
        </div>
        

        <p v-if="response.status" class="response-status">
          <em>Ticket Status: {{ response.status }}</em>
        </p>
        <hr class="white-line" />
      </div>
    </div>
    <div v-else>
      <p>No Responses yet</p>
    </div>

    <div v-if="error" class="error-message">{{ error }}</div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { getMyTicketById } from '../api/userApi';
import UserNavbar from '../components/UserNavbar.vue';

const route = useRoute();
const ticket = ref(null);
const error = ref(null);

const fetchTicket = async () => {
  try {
    const id = route.params.id;
    ticket.value = await getMyTicketById(id);
    ticket.value.responses = ticket.value.viewResponses;
    console.log(ticket.value);
  } catch (err) {
    error.value = err.message || 'Failed to load ticket.';
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


onMounted(fetchTicket);
</script>

<style scoped>
.ticket-view-page {
  padding: 2rem;
  max-width: 900px;
  margin: auto;
  color: #eee;
}

.ticket-card {
  background-color: white;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.3);
  margin-bottom: 2rem;
}

.ticket-card h1 {
  color: black;
  margin-bottom: 1rem;
}

.ticket-description {
  font-size: 1.1rem;
  margin-bottom: 1.5rem;
  color: black;
}

.ticket-meta span {
  display: block;
  margin-bottom: 0.5rem;
  color: black;
}

.ticket-attachments ul,
.response-attachments {
  list-style: none;
  color: black;
  padding-left: 1rem;
}

.ticket-attachments a,
.response-attachments a {
  color: #42b983;
  text-decoration: underline;
}

.response-section {
  background-color: white;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 0 5px rgba(0, 0, 0, 0.2);
}

.response-section h2 {
  color: black;
  margin-bottom: 1.2rem;
}

.ticket-response {
  background-color: white;
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 1.2rem;
}

.response-header {
  font-weight: bold;
  margin-bottom: 0.5rem;
  color: black;
}

.response-time {
  font-weight: normal;
  color: black;
  margin-left: 0.5rem;
}

.response-message {
  color: black;
  margin-bottom: 0.5rem;
}

.response-status {
  color: black;
  font-style: italic;
}

.error-message {
  color: #ff4d4f;
  font-weight: bold;
  margin-top: 2rem;
  text-align: center;
}

.white-line {
  border: none;
  height: 2px;
  background-color: white;
  margin: 1.5rem 0;
  opacity: 0.2;
}
</style>