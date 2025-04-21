<template>
  <nav class="navbar">
    <div class="brand-title">eMedia</div>

    <div class="nav-links button-container">
      <router-link v-if="notHome" to="/home" class="button small-button">Home</router-link>

        <router-link
        v-if="hasPolicy('AdminOnly') || hasPolicy('AdminAndDepartmentUser')"
        to="/admin"
        class="button small-button"
      >
        Admin Panel
      </router-link>


    <router-link
        v-if="notMyTickets && hasPolicy('RegularUserOnly')"
        to="/user/mytickets"
        class="button small-button"
      >
        My Tickets
      </router-link>

      <router-link
        v-if="notMyResponses && hasPolicy('RegularUserOnly')"
        to="/myresponses"
        class="button small-button"
      >
        My Responses
      </router-link>

      <!-- ✅ Show to admins OR department users -->
      <router-link
        v-if="hasPolicy('DepartmentUserOnly') && !isOnSharedTickets"
        to="/sharedtickets"
        class="button small-button"
      >
        My Tickets
      </router-link>

        <!-- ✅ Show to admins OR department users -->
        <router-link
        v-if="hasPolicy('DepartmentUserOnly') && !isOnMyDepartments"
        to="/mydepartments"
        class="button small-button"
      >
        Departments
      </router-link>

      <!-- ✅ Show to department users -->
      <router-link
        v-if="hasPolicy('DepartmentUserOnly') && !isOnSentResponses"
        to="/sentresponses"
        class="button small-button"
      >
        Responses
      </router-link>

      <!-- User icon dropdown -->
      <div class="user-menu" @click="toggleDropdown">
        <img src="../assets/user-icon.png" alt="User" class="user-icon" />

        <div v-if="dropdownOpen" class="dropdown">
          <router-link to="/profile" class="dropdown-item">Profile</router-link>
          <button @click="logout" class="dropdown-item logout">Logout</button>
        </div>
      </div>
    </div>
  </nav>
</template>
<script setup>
import {  ref } from 'vue';
import { useRouteFlags } from '../utils/routeUtils';
import router from '../router';
import { hasPolicy } from '../utils/hasPolicy';
import { useRoute } from 'vue-router';

const route = useRoute();
const { notHome, notMyTickets } = useRouteFlags();

const isOnSharedTickets = route.path === '/sharedtickets';
const isOnSentResponses = route.path === '/sentresponses';
const isOnMyDepartments = route.path === '/mydepartments';
const notMyResponses = route.path != '/myresponses';

const dropdownOpen = ref(false);

const toggleDropdown = () => {
  dropdownOpen.value = !dropdownOpen.value;
};

const logout = () => {
  localStorage.clear();
  router.push('/');
};

</script>



<style scoped>
@import '../assets/css/navbar.css';
@import '../assets/css/custom.css';

.navbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 1.2rem;
  height: 60px;
  background-color: #1f1f1f;
  border-bottom: 1px solid #333;
}

.nav-content {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-menu {
  position: relative;
  display: flex;
  align-items: center;
  margin-left: 0.5rem;
  padding: 0.2rem;
}

.user-icon {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  object-fit: cover;
  border: 1px solid #666;
}

.dropdown {
  position: absolute;
  top: 100%;
  right: 0;
  margin-top: 8px;
  background: #333;
  border: 1px solid #444;
  border-radius: 5px;
  min-width: 140px;
  z-index: 100;
}

.dropdown-item {
  display: block;
  padding: 0.5rem 1rem;
  text-align: left;
  color: #eee;
  background: none;
  text-decoration: none;
  border: none;
  width: 100%;
  cursor: pointer;
}

.dropdown-item:hover {
  background-color: #444;
}

.logout {
  color: #f77;
}

</style>