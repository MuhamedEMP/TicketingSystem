<template>
  
  <UserNavbar />
    <div >
      <h1>ADMIN</h1>
      <RouterLink to="/admin/users">VIEW USERS</RouterLink>
    </div>
  </template>

<script setup>
import { RouterLink } from 'vue-router';
import UserNavbar from '../../components/UserNavbar.vue';
</script>