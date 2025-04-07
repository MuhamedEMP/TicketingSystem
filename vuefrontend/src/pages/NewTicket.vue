<!-- src/pages/NewTicket.vue -->
<template>
    <div class="new-ticket-page">
      <h1>Create New Ticket</h1>
      <form @submit.prevent="submitTicket">
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
          <label for="category">Category ID</label>
          <input type="number" v-model="ticket.categoryId" id="category" required />
        </div>
  
        <div class="form-group">
          <label for="department">Department ID</label>
          <input type="number" v-model="ticket.departmentId" id="department" required />
        </div>
  
        <div class="form-group">
          <label for="description">Description</label>
          <textarea v-model="ticket.description" id="description"></textarea>
        </div>
  
        <div class="form-group">
          <label for="attachments">Attachments</label>
          <input type="file" multiple @change="handleFiles" />
        </div>
  
        <button type="submit">Submit Ticket</button>
      </form>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue';
  
  const ticket = ref({
    categoryId: null,
    departmentId: null,
    title: '',
    urgency: 0,
    description: '',
    attachments: [],
  });
  
  const handleFiles = (event) => {
    const files = event.target.files;
    ticket.value.attachments = Array.from(files).map(file => ({
      fileName: file.name,
      fileData: file, // Handle encoding to base64 or FormData on submit if needed
    }));
  };
  
  const submitTicket = async () => {
    console.log(JSON.stringify(ticket.value));
    const token = localStorage.getItem("accessToken");
  
    const response = await fetch('http://localhost:5172/user/newticket', {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(ticket.value),
    });
  
    if (response.ok) {
      alert("Ticket submitted!");
    } else {
      console.error("Failed to submit ticket");
    }
  };
  </script>
  
  <style scoped>
    @import '../assets/css/newticket.css';
  </style>
  