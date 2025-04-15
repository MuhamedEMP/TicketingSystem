<template>
  <div>
    <AdminNavbar />
    <div class="overlay"></div>

    <div class="home-page">
      <h1>Add New Department</h1>

      <div class="department-block">
        <input
          v-model="deptName"
          type="text"
          placeholder="Enter department name"
          class="input-field"
        />
        <button @click="submit" class="button small-button">
          Add Department
        </button>

        <p v-if="message" class="success-msg">{{ message }}</p>
        <p v-if="error" class="error-msg">{{ error }}</p>
      </div>
    </div>
  </div>
</template>

  
  <script setup>
  import { ref } from 'vue';
  import AdminNavbar from '../../components/adminComponents/AdminNavbar.vue';
  import { submitDepartment } from '../../api/adminApi';
  
  const deptName = ref('');
  const message = ref('');
  const error = ref('');
  
  const submit = async () => {
  message.value = ''
  error.value = ''

  if (!deptName.value.trim()) {
    error.value = 'Please enter a department name.'
    return
  }

  try {
    const response = await submitDepartment(deptName.value)
    message.value = response.data
    deptName.value = ''
  } catch (err) {
    if (err.response?.status === 409) {
      // If API sent a specific message → show it
      error.value = err.response?.data || 'Department already exists or cannot be added.'
    } else {
      // Fallback for other errors
      error.value = 'Failed to add department.'
    }
  }
}


  </script>
  <style scoped>
  .home-page {
    padding: 2rem;
    color: #eee;
  }
  
  .department-block {
    background-color: #2a2a2a;
    padding: 1rem;
    border-radius: 10px;
    display: flex;
    flex-direction: column;
    gap: 1rem;
    margin-top: 1rem;
  }
  
  .input-field {
    padding: 0.6rem;
    background-color: #1e1e1e;
    color: #eee;
    border: 1px solid #555;
    border-radius: 5px;
    outline: none;
  }
  
  .input-field:focus {
    border-color: #42b983;
  }
  
  .success-msg {
    color: lightgreen;
  }
  
  .error-msg {
    color: lightcoral;
  }
  
  .button {
    background-color: whitesmoke;
    border: 2px solid #232020;
    border-radius: 25px;
    font-size: 16px;
    font-weight: bold;
    color: #232020;
    cursor: pointer;
    transition: all 0.3s ease;
    text-align: center;
  }
  
  .small-button {
    padding: 8px 16px;
    font-size: 14px;
    border-radius: 20px;
  }
  
  .button:hover {
    background-color: #232020;
    color: whitesmoke;
  }
  </style>
  