<template>
    <UserNavbar />
    <div class="overlay"></div>
  
    <div class="ticket-view-page">
      <h1>📨 Sent Responses</h1>
  
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
  
          <ul v-if="response.attachmentUrls?.length" class="response-attachments">
            <li v-for="(path, index) in response.attachmentUrls" :key="index">
              <a :href="path" target="_blank">{{ path }}</a>
            </li>
          </ul>
  
          <p class="response-status">
            <em>Status: {{ response.status }}</em>
          </p>
          <router-link :to="`/sharedtickets/${response.ticketId}`" class="response-link">
            View Ticket
          </router-link>
          <hr class="white-line" />
        </div>
      </div>
  
      <p v-else class="no-responses">You haven't sent any responses yet.</p>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import { getMyResponses } from '../../api/sharedApi'; // you'll create this
  import UserNavbar from '../../components/UserNavbar.vue';
  
  const responses = ref([]);
  
  const formatDate = (date) => new Date(date).toLocaleString();
  
  onMounted(async () => {
    try {
      responses.value = await getMyResponses();
    } catch (err) {
      console.error('Failed to fetch sent responses:', err);
    }
  });
  </script>
  
  <style scoped>
  /* Reuse ticket styles or add custom ones here */
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
    color: #888;
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
  