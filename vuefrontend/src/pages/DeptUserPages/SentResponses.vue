<template>
  <UserNavbar />
  <div class="overlay"></div>

  <div class="ticket-view-page">
    <h1>Sent Responses</h1>

    <div class="filter-bar">
      <select v-model="filters.status">
        <option value="">All Statuses</option>
        <option value="Open">Open</option>
        <option value="Closed">Closed</option>
        <option value="InProgress">In Progress</option>
        <option value="Resolved">Resolved</option>
        <option value="Reopened">Reopened</option>
        <option value="Deleted">Deleted</option>
      </select>

      <select v-model="filters.hasAttachments">
        <option value="">All</option>
        <option value="true">With Attachments</option>
        <option value="false">No Attachments</option>
      </select>

      <input type="date" v-model="filters.fromDate" />
      <input type="date" v-model="filters.toDate" />

      <button @click="applyFilters">Search</button>
    </div>

    <div v-if="responses.length > 0" class="responses-grid">
      <div v-for="response in responses" :key="response.id" class="ticket-response">
        <div class="response-header">
          <strong>Ticket #{{ response.ticketId }}</strong>
          <span class="response-time">{{ formatDate(response.createdAt) }}</span>
        </div>

        <p class="response-message">{{ response.message }}</p>

        <div v-if="response.attachmentUrls?.length" class="response-attachments">
          <h4>📎 Attachments</h4>
          <ul>
            <li v-for="(url, index) in response.attachmentUrls" :key="index">
              <a :href="url" target="_blank">{{ getFileName(url) }}</a>
            </li>
          </ul>
        </div>

        <p class="response-status">
          <em>Status: {{ response.status }}</em>
        </p>
      </div>
    </div>

    <p v-else class="no-responses">No Responses found.</p>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { getMyResponses } from '../../api/sharedApi'
import UserNavbar from '../../components/UserNavbar.vue'

const responses = ref([])

const filters = ref({
  search: '',
  status: '',
  fromDate: '',
  toDate: '',
  hasAttachments: ''
})

const applyFilters = async () => {
  try {
    const params = new URLSearchParams()
    for (const key in filters.value) {
      if (filters.value[key]) {
        params.append(key, filters.value[key])
      }
    }
    responses.value = await getMyResponses(params)
  } catch (err) {
    console.error('❌ Failed to fetch filtered responses:', err)
  }
}

const fetchResponses = async () => {
  try {
    const params = new URLSearchParams()
    for (const key in filters.value) {
      if (filters.value[key]) {
        params.append(key, filters.value[key])
      }
    }
    responses.value = await getMyResponses(params)
  } catch (err) {
    console.error('❌ Failed to fetch responses:', err)
  }
}

const formatDate = (date) => new Date(date).toLocaleDateString()

const getFileName = (url) => {
  try {
    return decodeURIComponent(url.split('/').pop().split('?')[0])
  } catch {
    return 'Attachment'
  }
}

onMounted(fetchResponses)
</script>

<style scoped>
.ticket-view-page {
  padding: 2rem 1rem;
  color: #eee;
  width: 100%;
  box-sizing: border-box;
  font-family: 'Poppins', sans-serif;
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
}

h1 {
  margin-bottom: 1.5rem;
}

.filter-bar {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 1rem;
  margin-bottom: 2rem;
  max-width: 1100px;
  width: 100%;
}

.filter-bar select,
.filter-bar input[type="date"] {
  padding: 10px 14px;
  background-color: #fff;
  color: #333;
  border: none;
  border-radius: 20px;
  font-size: 14px;
  min-width: 180px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

.filter-bar button {
  padding: 10px 18px;
  background-color: #ca0176;
  color: white;
  border: none;
  border-radius: 20px;
  font-size: 14px;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.2s ease;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.2);
}

.filter-bar button:hover {
  color: #ca0176;
  background-color: white;
}

.responses-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
  gap: 3rem;
  justify-content: center;
  max-width: 1100px;
  width: 100%;
}

.ticket-response {
  background-color: white;
  color: black;
  padding: 1.25rem;
  border-radius: 20px;
  width: 100%;
  max-width: 260px;
  min-height: 240px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.response-header {
  display: flex;
  justify-content: space-between;
  font-weight: bold;
}

.response-time {
  font-size: 0.9rem;
}

.response-message {
  margin: 1rem 0;
  font-size: 15px;
  text-align: left;
}

.response-status {
  font-style: italic;
  font-size: 0.9rem;
  color: #555;
  text-align: left;
}

.response-attachments h4 {
  margin-bottom: 0.5rem;
}

.response-attachments a {
  color: #007bff;
  text-decoration: underline;
}

.no-responses {
  margin-top: 2rem;
  color: #aaa;
  font-style: italic;
}

@media (max-width: 768px) {
  .filter-bar {
    flex-direction: column;
    align-items: stretch;
  }

  .filter-bar > * {
    width: 100%;
    max-width: 300px;
  }
}
</style>
