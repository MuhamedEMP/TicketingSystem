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
          <span><strong>Assigned to:</strong> {{ ticket.assignedToName }}</span>
          <span v-if="hasPolicy('DepartmentUserOnly') && !ticket.assignedToId">
            <button class="assign-button" @click="assignToMe">Assign to Me</button>
          </span>

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
  
      <!-- Add Response Form (only for department users) -->
        <div v-if="hasPolicy('DepartmentUserOnly')" class="add-response-form">
        <h3>Add a Response</h3>
        <form @submit.prevent="submitResponse">
            <textarea
            v-model="newResponse.message"
            placeholder="Enter your response..."
            required
            ></textarea>

            <label>Status:</label>
            <select v-model.number="newResponse.status">
            <option disabled value="">Select status </option>
            <option :value="1">Open</option>
            <option :value="2">Closed</option>
            <option :value="3">InProgress</option>
            <option :value="4">Resolved</option>
            <option :value="5">Reopened</option>
            <option :value="6">Deleted</option>
            </select>
<!-- 
            Open = 1,
        Closed = 2,
        InProgress = 3,
        Resolved = 4,
        Reopened = 5,
        Deleted = 6, -->
            <button type="submit">Submit Response</button>
        </form>
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
              <em>Ticket Status: {{ response.status }}</em>
            </p>
            <hr class="white-line" />
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
  import { hasPolicy } from '../../utils/hasPolicy';
  import { postTicketResponse } from '../../api/sharedApi';
  import { assignTicketToMe } from '../../api/sharedApi';


const assignToMe = async () => {
  try {
    const updatedTicket = await assignTicketToMe(ticket.value.id);
    ticket.value = updatedTicket; 
  } catch (err) {
    console.error('❌ Failed to assign ticket:', err);
  }
};

const newResponse = ref({
  ticketId: null, // will be set after ticket is fetched
  message: '',
  status: '', // should be a string version of TicketStatusEnum (e.g. 'Pending')
  attachments: []
});

const submitResponse = async () => {
  if (!newResponse.value.message.trim()) return;
  const payload = {
    ticketId: ticket.value.id,
    message: newResponse.value.message,
    status: newResponse.value.status,
    attachments: [] // will stay empty for now
  };

  console.log('📤 Sending response payload:', payload);
  try {
    await postTicketResponse(payload);
    newResponse.value.message = '';
    newResponse.value.status = '';
    await fetchTicket(); // refresh ticket + responses
  } catch (err) {
    console.error('❌ Failed to post response:', err);
  }
};

  
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
  
  <style scoped>
  .assign-button {
  background-color: #42b983;
  border: none;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  font-weight: bold;
  margin-top: 0.8rem;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.assign-button:hover {
  background-color: #36966e;
}


  .white-line {
  border: none;
  height: 2px;
  background-color: white;
  margin: 1.5rem 0;
  opacity: 0.2; /* optional for soft effect */
} 

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
  color: whitesmoke;
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
  color: whitesmoke;
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

.add-response-form {
  background-color: #2a2a2a;
  padding: 1.5rem;
  border-radius: 12px;
  margin-top: 2rem;
  box-shadow: 0 0 6px rgba(0, 0, 0, 0.3);
}

.add-response-form h3 {
  color: #42b983;
  margin-bottom: 1rem;
  font-size: 1.2rem;
}

.add-response-form textarea {
  width: 100%;
  min-height: 120px;
  margin-bottom: 1rem;
  border-radius: 6px;
  border: 1px solid #444;
  padding: 0.8rem;
  background: #1f1f1f;
  color: #eee;
  resize: vertical;
  font-size: 1rem;
}

.add-response-form select {
  width: 100%;
  padding: 0.6rem;
  border-radius: 6px;
  background-color: #1f1f1f;
  color: #eee;
  border: 1px solid #444;
  margin-bottom: 1rem;
  font-size: 1rem;
}

.add-response-form button {
  padding: 0.6rem 1.2rem;
  background-color: #42b983;
  border: none;
  color: white;
  border-radius: 8px;
  font-weight: bold;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.add-response-form button:hover {
  background-color: #36966e;
}
</style>
  