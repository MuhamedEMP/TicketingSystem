<template>
    <UserNavbar />
    <div class="overlay"></div>
  
    <div class="profile-page">
      <h1>My Profile</h1>
  
      <div v-if="user" class="profile-card">
        <p><strong>Full Name:</strong> {{ user.fullName }}</p>
        <p><strong>First Name:</strong> {{ user.firstName }}</p>
        <p><strong>Last Name:</strong> {{ user.lastName }}</p>
        <p><strong>Email:</strong> {{ user.email }}</p>
  
        <div class="roles">
          <p><strong>Roles:</strong></p>
          <ul>
            <li v-for="role in user.roles" :key="role">{{ role }}</li>
          </ul>
        </div>
      </div>
  
      <p v-else>Loading profile...</p>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import UserNavbar from '../components/UserNavbar.vue';
  import { getUserProfile } from '../api/userApi';
  
  const user = ref(null);
  
  onMounted(async () => {
    try {
      user.value = await getUserProfile();
    } catch (error) {
      console.error('Failed to fetch user profile:', error);
    }
  });
  </script>
  
  <style scoped>
  .profile-page {
    padding: 2rem;
    color: #eee;
  }
  
  .profile-card {
    background-color: #2c2c2c;
    padding: 1.5rem;
    border-radius: 10px;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.6);
    max-width: 500px;
    margin: 0 auto;
  }
  
  .profile-card p {
    margin: 0.5rem 0;
  }
  
  .roles ul {
    padding-left: 1rem;
  }
  
  .roles li {
    list-style-type: disc;
    color: #ccc;
  }
  </style>
  