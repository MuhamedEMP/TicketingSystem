<template>
  <UserNavbar />
  <div class="overlay"></div>

  <div class="new-ticket-page">
    <div class="info-banner">
      <p><strong>Department:</strong> {{ departmentName }}</p>
      <p><strong>Category:</strong> {{ categoryName }}</p>
      <p v-if="categoryDescription" class="category-description">
        {{ categoryDescription }}
      </p>
    </div>

    <form @submit.prevent="submitTicket" class="ticket-form">
      <div class="form-group">
        <label for="title">Title</label>
        <input v-model="ticket.title" id="title" required />
      </div>

      <div class="form-group">
        <label for="urgency">Urgency</label>
        <select v-model.number="ticket.urgency" id="urgency" required>
          <option disabled value="">Select urgency</option>
          <option value="1">Low</option>
          <option value="2">Medium</option>
          <option value="3">High</option>
        </select>
      </div>

      <div class="form-group">
        <label for="description">Description</label>
        <textarea v-model="ticket.description" id="description"></textarea>
      </div>

      <div class="form-group">
        <label for="attachments">Attachments</label>
        <input type="file" multiple @change="handleFiles" />
      </div>

      <ul class="upload-preview">
        <li v-for="(file, index) in rawFiles" :key="index">
          {{ file.name }}
          <button class="delete-btn" @click.prevent="rawFiles.splice(index, 1)">❌</button>
        </li>
      </ul>

      <p v-if="isUploading" class="uploading-message">⏳ Uploading attachments, please wait...</p>
      <button type="submit" class="submit-button" :disabled="isUploading">Submit Ticket</button>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { submitNewTicket } from '../api/userApi';
import UserNavbar from '../components/UserNavbar.vue';
import { getDepartmentById } from '../api/departmentApi';
import { getCategoryById } from '../api/categoryApi';
import { uploadToSharePoint } from '../api/sharepointUploader';

const departmentName = ref('');
const categoryName = ref('');
const categoryDescription = ref('');
const rawFiles = ref([]);
const isUploading = ref(false);

const route = useRoute();
const departmentId = parseInt(route.params.departmentId);
const categoryId = parseInt(route.params.categoryId);

const ticket = ref({
  title: '',
  urgency: 0,
  description: '',
  attachments: [],
});

const handleFiles = (event) => {
  rawFiles.value = [...event.target.files];
};

const resetForm = () => {
  ticket.value.title = '';
  ticket.value.urgency = 0;
  ticket.value.description = '';
  ticket.value.attachments = [];
  rawFiles.value = [];

  const fileInput = document.querySelector('input[type="file"]');
  if (fileInput) fileInput.value = '';
};

const submitTicket = async () => {
  isUploading.value = true;
  try {
    const uploaded = [];
    for (const file of rawFiles.value) {
      const url = await uploadToSharePoint(file);
      uploaded.push({
        Path: url,
        Filename: file.name,
        ContentType: file.type
      });
    }

    const payload = {
      ...ticket.value,
      attachments: uploaded
    };

    await submitNewTicket(departmentId, categoryId, payload);
    alert("Ticket submitted!");
    resetForm();
  } catch (error) {
    console.error("Failed to submit ticket:", error);
  } finally {
    isUploading.value = false;
  }
};

onMounted(async () => {
  try {
    const dept = await getDepartmentById(departmentId);
    const cat = await getCategoryById(categoryId);

    categoryDescription.value = cat.description;
    departmentName.value = dept.name;
    categoryName.value = cat.name;
  } catch (error) {
    console.error('Failed to fetch department or category:', error);
  }
});
</script>

<style scoped>
.uploading-message {
  color: #ca0176;
  font-style: italic;
  font-size: 0.95rem;
  margin-top: -0.8rem;
  margin-bottom: -0.5rem;
}

.new-ticket-page {
  max-width: 700px;
  margin: 2rem auto;
  padding: 2rem;
  background-color: #ffffff; /* white background */
  border-radius: 12px;
  box-shadow: 0 0 12px rgba(0, 0, 0, 0.1);
  color: #333;
  font-family: 'Segoe UI', sans-serif;
}

.info-banner {
  margin-bottom: 2rem;
  padding: 0;
  background: none;
  border: none;
  color: #333;
}

.info-banner p {
  margin: 0.3rem 0;
  font-size: 1rem;
  font-weight: 500;
}

.category-description {
  margin-top: 0.3rem;
  font-style: italic;
  color: #555;
  background: none;
  border: none;
  padding: 0;
}

.ticket-form {
  display: flex;
  flex-direction: column;
  gap: 1.2rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

label {
  margin-bottom: 0.3rem;
  font-weight: bold;
  color: #222;
}

input[type="text"],
input[type="file"],
input,
select,
textarea {
  background-color: #f9f9f9;
  color: #222;
  border: 1px solid #ccc;
  border-radius: 6px;
  padding: 0.6rem;
  font-size: 1rem;
  resize: vertical;
}

textarea {
  min-height: 120px;
}

input:focus,
textarea:focus,
select:focus {
  outline: none;
  border-color: #ca0176;
}

.submit-button {
  background-color: #ca0176;
  color: #fff;
  padding: 0.8rem 1.2rem;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.submit-button:hover {
  background-color: #a60060;
}

.upload-preview {
  margin-top: -0.5rem;
}

.upload-preview li {
  font-size: 0.95rem;
  margin: 4px 0;
  color: #444;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.delete-btn {
  margin-left: 8px;
  background: transparent;
  border: none;
  color: #ca0176;
  cursor: pointer;
  font-size: 1rem;
}
</style>
