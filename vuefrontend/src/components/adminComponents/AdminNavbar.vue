<template>
    
  </template>

<script setup>
import { ref } from 'vue';
import { useRouteFlags } from '../../utils/routeUtils';
import router from '../../router';

const { notHome, notMyTickets, notAdmin } = useRouteFlags();

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
@import '../../assets/css/navbar.css';
@import '../../assets/css/custom.css';


.navbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 1.2rem;
  height: 80px;
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