<template>
    <div class="error-page">
      <h1>403 - Forbidden</h1>
      <p>You do not have permission to view this page.</p>
      <router-link to="/home">Return Home</router-link>
    </div>
  </template>
  
  <style scoped>
  .error-page {
    text-align: center;
    padding: 2rem;
  }
  </style>
<script setup>
import { onMounted } from 'vue';
import { refreshUserRoles } from '../../utils/userUtils'; 

onMounted(() => {
  refreshUserRoles();
});
</script>
