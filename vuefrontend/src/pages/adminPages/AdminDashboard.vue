<template>
  <div>
    <AdminNavbar />
    <div class="admin-home-page">
      <h1 class="admin-title">ADMIN PANEL</h1>

      <div class="admin-links-container">
        <RouterLink to="/admin/users" class="admin-link-button">
          Manage Users
        </RouterLink>

        <RouterLink to="/admin/departments" class="admin-link-button">
          Departments & Categories
        </RouterLink>
      </div>
    </div>
  </div>
</template>


<script setup>
import { RouterLink } from 'vue-router';
import AdminNavbar from '../../components/adminComponents/AdminNavbar.vue';
</script>

<style scoped>
.admin-home-page {
  padding: 2rem;
  color: #eee;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.admin-title {
  margin-bottom: 2rem;
  color: whitesmoke;
  font-size: 2rem;
}

.admin-links-container {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  width: 100%;
  max-width: 400px;
}

.admin-link-button {
  background-color: #2a2a2a;
  color: #eee;
  padding: 1rem;
  border-radius: 10px;
  text-align: center;
  text-decoration: none;
  font-weight: bold;
  border: 1px solid #555;
  transition: 0.3s ease;
}

.admin-link-button:hover {
  background-color: #414141;
  color: #42b983;
  border-color: #42b983;
}
</style>
