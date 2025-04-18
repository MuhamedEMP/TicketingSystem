<template>
    <UserNavbar />
    <div class="overlay"></div>
  
    <div class="ticket-view-page">
      <!-- Ticket Details Card -->
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
          <h3>Attachments</h3>
          <ul>
            <li v-for="(file, index) in ticket.attachmentPaths" :key="index">
              <a :href="file" target="_blank">{{ file }}</a>
            </li>
          </ul>
        </div>
      </div>
  
      <!-- Ticket Responses Section -->
      <div v-if="ticket && ticket.responses" class="response-section">
        <h2>Responses</h2>
  
        <div v-if="ticket.responses.length > 0">
          <div
            v-for="response in ticket.responses"
            :key="response.id"
            class="ticket-response"
          >
            <div class="response-header">
              <strong>{{ response.userFullName }}</strong>
              <span class="response-time">— {{ formatDate(response.createdAt) }}</span>
            </div>
            <p class="response-message">{{ response.message }}</p>
  
            <ul v-if="response.attachmentUrls?.length" class="response-attachments">
              <li v-for="(path, index) in response.attachmentUrls" :key="index">
                <a :href="path" target="_blank">{{ path }}</a>
              </li>
            </ul>
  
            <p v-if="response.status" class="response-status">
              <em>Status changed to: {{ response.status }}</em>
            </p>
          </div>
        </div>
        <p v-else class="no-responses">No responses yet.</p>
      </div>
  
      <div v-else-if="error" class="error-message">{{ error }}</div>
      <div v-else class="ticket-meta">Loading ticket...</div>
    </div>
  </template>
  
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import { useRoute } from 'vue-router';
  import { getSharedTicketById } from '../../api/sharedApi';
  import UserNavbar from '../../components/UserNavbar.vue';
  
  const route = useRoute();
  const ticket = ref(null);
  const error = ref(null);
  
  const fetchTicket = async () => {
    try {
      const id = route.params.id;
      ticket.value = await getSharedTicketById(id);
      ticket.value.responses = ticket.value.viewResponses;
 // 🔁 For department users
    } catch (err) {
      error.value = err.message || 'Failed to load ticket.';
    }
  };
  
  const formatDate = (date) => {
    return new Date(date).toLocaleString();
  };
  
  onMounted(fetchTicket);
  </script>
  
  <style scoped src="../../assets/css/viewticket.css">
.ticket-view-page {
  padding: 2rem;
  max-width: 900px;
  margin: auto;
  color: #eee;
}

.ticket-card {
  background-color: #2a2a2a;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.3);
  margin-bottom: 2rem;
}

.ticket-card h1 {
  color: #42b983;
  margin-bottom: 1rem;
}

.ticket-description {
  font-size: 1.1rem;
  margin-bottom: 1.5rem;
  color: #ccc;
}

.ticket-meta span {
  display: block;
  margin-bottom: 0.5rem;
  color: #bbb;
}

.ticket-attachments ul,
.response-attachments {
  list-style: none;
  padding-left: 1rem;
}

.ticket-attachments a,
.response-attachments a {
  color: #42b983;
  text-decoration: underline;
}

.response-section {
  background-color: #1f1f1f;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 0 5px rgba(0, 0, 0, 0.2);
}

.response-section h2 {
  color: #42b983;
  margin-bottom: 1.2rem;
}

.ticket-response {
  background-color: #2e2e2e;
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 1.2rem;
  border-left: 4px solid #42b983;
}

.response-header {
  font-weight: bold;
  margin-bottom: 0.5rem;
  color: #eee;
}

.response-time {
  font-weight: normal;
  color: #888;
  margin-left: 0.5rem;
}

.response-message {
  color: #ccc;
  margin-bottom: 0.5rem;
}

.response-status {
  color: #aaa;
  font-style: italic;
}

.no-responses {
  color: #888;
  font-style: italic;
}
</style>
  